using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using otherServices.Models;
using otherServices.Models.DTOs;
using WebAPIDotNet.DTOs;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.Extensions.Hosting;
using otherServices.Repositories;

namespace WebAPIDotNet.Services
{
    public class LandlordService : ILandlordService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IUserRepository _userRepository;
        private readonly IPostRepository _postRepository;
        private readonly IProposalRepository _proposalRepository;
        public LandlordService( IWebHostEnvironment env, IUserRepository userRepository, IPostRepository postRepository, IProposalRepository proposalRepository)
        {
            _env = env;
            _userRepository = userRepository;
            _postRepository = postRepository;
            _proposalRepository = proposalRepository;
        }

        public async Task<PostDTo> Create_Post(long landlordId, CreatePostDTO postDto)
        {
            var landlord = await _userRepository.GetByIdAsync(landlordId);
            if (landlord == null) throw new KeyNotFoundException("Landlord not found");

            if (postDto.Image == null || postDto.Image.Length == 0)
                throw new ArgumentException("File is required");

            string uploadPath = Path.Combine(_env.ContentRootPath, "../Media");
            Directory.CreateDirectory(uploadPath);

            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(postDto.Image.FileName);
            string imagePath = Path.Combine(uploadPath, uniqueFileName);

            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await postDto.Image.CopyToAsync(stream);
            }

            var post = new Post
            {
                Title = postDto.Title,
                Description = postDto.Description,
                Price = postDto.Price,
                Location = postDto.Location,
                RentalStatus = "Available",
                LandlordId = landlordId,
                DatePost = DateTime.Now,
                FlagWaitingPost = 1,
                ImagePath = imagePath
            };

            await _postRepository.AddAsync(post);
            await _postRepository.SaveChangesAsync();

            return MapToDTO(post, landlord);
        }



        public async Task<PostDTo> Get_Post_By_Id(long id)
        {
            var posts = await _postRepository.NestedFind(p => p.PostId == id, p => p.Landlord);
            var post = posts.FirstOrDefault();
            if (post == null) throw new KeyNotFoundException("Post not found");

            return MapToDTO(post, post.Landlord);
        }

        public async Task<List<PostDTo>> Get_Posts_By_LandlordId(long landlordId)
        {
            var posts = await _postRepository.NestedFind(p => p.LandlordId == landlordId, p => p.Landlord);
            return posts.Select(p => MapToDTO(p, p.Landlord)).ToList();
        }

        public async Task<bool> Delete_Post(long postId)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null) return false;

            _postRepository.Remove(post);
            await _postRepository.SaveChangesAsync();
            return true;
        }

        public async Task<PostDTo> Update_Post(long postId, UpdatePostDTO updateDto)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null) throw new KeyNotFoundException("Post not found");

            if (updateDto.Title != null) post.Title = updateDto.Title;
            if (updateDto.Description != null) post.Description = updateDto.Description;
            if (updateDto.Price.HasValue) post.Price = updateDto.Price.Value;
            if (updateDto.Location != null) post.Location = updateDto.Location;
            if (updateDto.RentalStatus != null) post.RentalStatus = updateDto.RentalStatus;

            if (updateDto.File != null && updateDto.File.Length > 0)
            {
                if (!string.IsNullOrEmpty(post.ImagePath) && File.Exists(post.ImagePath))
                    File.Delete(post.ImagePath);

                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "../Media");
                if (!Directory.Exists(uploadsDir)) Directory.CreateDirectory(uploadsDir);

                var fileName = $"{Guid.NewGuid()}_{updateDto.File.FileName}";
                var filePath = Path.Combine(uploadsDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await updateDto.File.CopyToAsync(stream);
                }

                post.ImagePath = filePath;
            }

            _postRepository.Update(post);
            await _postRepository.SaveChangesAsync();

            var landlord = await _userRepository.GetByIdAsync(post.LandlordId);
            return MapToDTO(post, landlord);
        }

        private PostDTo MapToDTO(Post post, User landlord)
        {
            string base64File = null;

            if (!string.IsNullOrEmpty(post.ImagePath) && File.Exists(post.ImagePath))
            {
                byte[] fileBytes = File.ReadAllBytes(post.ImagePath);
                base64File = Convert.ToBase64String(fileBytes);
            }

            return new PostDTo
            {
                PostId = post.PostId,
                Title = post.Title,
                Description = post.Description,
                Price = post.Price,
                Location = post.Location,
                RentalStatus = post.RentalStatus,
                DatePost = post.DatePost,
                FlagWaitingPost = post.FlagWaitingPost,
                ImagePath = post.ImagePath,
                FileBase64 = base64File,
                UserId = landlord.UserId,
                UserName = landlord.UserName,
                Email = landlord.Email
            };
        }





        public async Task<Proposal> AcceptProposal(long proposalId)
        {
            var proposals = await _proposalRepository.NestedFind(p => p.ProposalId == proposalId, p => p.Post);
            var proposal = proposals.FirstOrDefault();
            if (proposal == null) throw new KeyNotFoundException("Proposal not found");

            proposal.RentalStatus = "Approved";
            if (proposal.Post != null)
                proposal.Post.RentalStatus = "Rental";

            await _proposalRepository.SaveChangesAsync();
            return proposal;
        }

        public async Task<Proposal> RejectProposal(long proposalId)
        {
            var proposal = await _proposalRepository.GetByIdAsync(proposalId);
            if (proposal == null) throw new KeyNotFoundException("Proposal not found");

            proposal.RentalStatus = "Rejected";
            await _proposalRepository.SaveChangesAsync();
            return proposal;
        }

        public async Task<IEnumerable<ProposalDto>> GetLandlordProposalsAsync(long landlordId)
        {
            var landlordPosts = await _postRepository.FindAsync(p => p.LandlordId == landlordId);
            var postIds = landlordPosts.Select(p => p.PostId).ToList();

            var proposals = await _proposalRepository.NestedFind(
                p => postIds.Contains(p.PostId),
                p => p.User);

            var result = new List<ProposalDto>();

            foreach (var proposal in proposals)
            {
                string base64File = null;
                if (!string.IsNullOrEmpty(proposal.FilePath) && File.Exists(proposal.FilePath))
                {
                    byte[] fileBytes = await File.ReadAllBytesAsync(proposal.FilePath);
                    base64File = Convert.ToBase64String(fileBytes);
                }

                result.Add(new ProposalDto
                {
                    ProposalId = proposal.ProposalId,
                    PostId = proposal.PostId,
                    TenantId = proposal.TenantId,
                    TenantName = proposal.User.UserName,
                    Phone = proposal.Phone,
                    StartRentalDate = proposal.StartRentalDate,
                    EndRentalDate = proposal.EndRentalDate,
                    RentalStatus = proposal.RentalStatus,
                    FileName = Path.GetFileName(proposal.FilePath),
                    FileBase64 = base64File
                });
            }

            return result;
        }

    }
}