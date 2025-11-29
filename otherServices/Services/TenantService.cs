using CommentAPI.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using otherServices.Models;
using otherServices.Models.DTOs;
using otherServices.Repositories;

namespace otherServices.Services
{
    public class TenantService : ITenantService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IUserRepository _userRepository;
        private readonly IProposalRepository _proposalRepository;
        private readonly IPostRepository _postRepository;
        private readonly ISavedPostRepository _savedPostRepository;


        public TenantService( IWebHostEnvironment env, IProposalRepository proposalRepository, IPostRepository postRepository, ISavedPostRepository savedPostRepository , IUserRepository userRepository)
        {
            _env = env;
            _proposalRepository = proposalRepository;
            _postRepository = postRepository;
            _savedPostRepository = savedPostRepository;
            _userRepository = userRepository;
        }
        public async Task<bool> Save_Post(long userId, long postId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var post = await _postRepository.GetByIdAsync(postId);
            if (user == null || post == null)
                return false;

            var exists = (await _savedPostRepository.FindAsync(sp => sp.TenantId == userId && sp.PostId == postId)).Any();
            if (exists) return false;

            await _savedPostRepository.AddAsync(new SavedPost
            {
                TenantId = userId,
                PostId = postId,
            });
            await _savedPostRepository.SaveChangesAsync();

            return true;
        }
        public async Task<string> SubmitProposalAsync(long PostId, SubmitProposalDto form)
        {
            if (form.File == null || form.File.Length == 0)
                throw new ArgumentException("File is required");

            string uploadPath = Path.Combine(_env.ContentRootPath, "../Media");
            Directory.CreateDirectory(uploadPath);

            string uniqueFileName = Guid.NewGuid() + Path.GetExtension(form.File.FileName);
            string filePath = Path.Combine(uploadPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await form.File.CopyToAsync(stream);
            }

            var proposal = new Proposal
            {
                PostId = PostId,
                TenantId = form.TenantId,
                Phone = form.Phone,
                StartRentalDate = form.StartRentalDate,
                EndRentalDate = form.EndRentalDate,
                FilePath = filePath,
                RentalStatus = "Pending"
            };

            await _proposalRepository.AddAsync(proposal);
            await _proposalRepository.SaveChangesAsync();

            return "Proposal submitted successfully!";
        }


        public async Task<bool> DeleteProposalAsync(long proposalId)
        {
            var proposal = await _proposalRepository.GetByIdAsync(proposalId);
            if (proposal == null) return false;

            if (!string.IsNullOrEmpty(proposal.FilePath) && File.Exists(proposal.FilePath))
                File.Delete(proposal.FilePath);

            _proposalRepository.Remove(proposal);
            await _proposalRepository.SaveChangesAsync();
            return true;
        }


        public async Task<bool> EditProposalAsync(long proposalId, ProposalEditDto updated)
        {
            var proposal = await _proposalRepository.GetByIdAsync(proposalId);
            if (proposal == null) return false;

            proposal.Phone = updated.Phone;
            proposal.StartRentalDate = updated.StartRentalDate;
            proposal.EndRentalDate = updated.EndRentalDate;

            if (updated.File != null && updated.File.Length > 0)
            {
                if (!string.IsNullOrEmpty(proposal.FilePath) && File.Exists(proposal.FilePath))
                    File.Delete(proposal.FilePath);

                string uploadsDir = Path.Combine(_env.ContentRootPath, "../Media");
                Directory.CreateDirectory(uploadsDir);

                string newFileName = $"{Guid.NewGuid()}_{updated.File.FileName}";
                string filePath = Path.Combine(uploadsDir, newFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await updated.File.CopyToAsync(stream);
                }

                proposal.FilePath = filePath;
            }

            await _proposalRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PostDTo>> GetPosts()
        {
            var posts = await _postRepository.NestedFind(p => p.FlagWaitingPost == 0, p => p.Landlord);

            return posts.Select(p => new PostDTo
            {
                PostId = p.PostId,
                Title = p.Title,
                Description = p.Description,
                Price = p.Price,
                Location = p.Location,
                RentalStatus = p.RentalStatus,
                DatePost = p.DatePost,
                FlagWaitingPost = p.FlagWaitingPost,
                UserId = p.Landlord?.UserId ?? 0,
                UserName = p.Landlord?.UserName,
                Email = p.Landlord?.Email,
                ImagePath = p.ImagePath,
                FileBase64 = (!string.IsNullOrEmpty(p.ImagePath) && File.Exists(p.ImagePath))
                    ? Convert.ToBase64String(File.ReadAllBytes(p.ImagePath))
                    : null
            });
        }



        public async Task<List<SavedPostDto>> GetMySavedPosts(long tenantId)
        {
            var savedPosts = await _savedPostRepository.NestedFind(
                sp => sp.TenantId == tenantId,
                sp => sp.Post,
                sp => sp.Post.Landlord,
                sp => sp.Post.Comments);

            if (!savedPosts.Any()) throw new KeyNotFoundException("No saved posts found.");

            var result = new List<SavedPostDto>();

            foreach (var sp in savedPosts)
            {
                var post = sp.Post;
                string base64File = null;

                if (!string.IsNullOrEmpty(post.ImagePath) && File.Exists(post.ImagePath))
                {
                    byte[] fileBytes = await File.ReadAllBytesAsync(post.ImagePath);
                    base64File = Convert.ToBase64String(fileBytes);
                }

                result.Add(new SavedPostDto
                {
                    PostId = post.PostId,
                    Title = post.Title,
                    Description = post.Description,
                    Price = post.Price,
                    Location = post.Location,
                    DatePost = post.DatePost,
                    RentalStatus = post.RentalStatus,
                    FlagWaitingPost = post.FlagWaitingPost,
                    ImagePath = post.ImagePath,
                    FileBase64 = base64File,

                    landlordId = post.Landlord.UserId,
                    landlordUserName = post.Landlord.UserName,

                    Comments = post.Comments.Select(c => new PostsCommentsDto
                    {
                        CommentId = c.CommentId,
                        PostId = c.PostId,
                        Comment_Written = c.Description,
                        DateComment = c.DateComment
                    }).ToList()
                });
            }

            return result;
        }



        public async Task<bool> cancelSave(long userId, long postId)
        {
            var post = (await _savedPostRepository.FindAsync(sp => sp.TenantId == userId && sp.PostId == postId)).FirstOrDefault();
            if (post == null) return false;

            _savedPostRepository.Remove(post);
            await _savedPostRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProposalDto>> GetTenantProposalsAsync(long tenantId)
        {
            var proposals = await _proposalRepository.NestedFind(
                p => p.TenantId == tenantId,
                p => p.Post,
                p => p.Post.Landlord);

            return await Task.WhenAll(proposals.Select(async proposal =>
            {
                string base64File = null;
                if (!string.IsNullOrEmpty(proposal.FilePath) && File.Exists(proposal.FilePath))
                {
                    byte[] fileBytes = await File.ReadAllBytesAsync(proposal.FilePath);
                    base64File = Convert.ToBase64String(fileBytes);
                }

                return new ProposalDto
                {
                    ProposalId = proposal.ProposalId,
                    PostId = proposal.PostId,
                    TenantId = proposal.TenantId,
                    Phone = proposal.Phone,
                    StartRentalDate = proposal.StartRentalDate,
                    EndRentalDate = proposal.EndRentalDate,
                    RentalStatus = proposal.RentalStatus,
                    FileName = Path.GetFileName(proposal.FilePath),
                    FileBase64 = base64File,
                    LandlordId = proposal.Post.Landlord.UserId,
                    LandlordName = proposal.Post.Landlord.UserName
                };
            }));
        }

    }
}
