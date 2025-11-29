using otherServices.Models;
using otherServices.Models.DTOs;

namespace otherServices.Services
{
    public interface ITenantService
    {

        Task<string> SubmitProposalAsync(long PostId,SubmitProposalDto dto);
        Task<bool> EditProposalAsync(long proposalId, ProposalEditDto updatedProposal);
        Task<bool> DeleteProposalAsync(long proposalId);
        Task<bool> cancelSave(long userId,long postId);
        Task<IEnumerable<PostDTo>> GetPosts();
        Task<List<SavedPostDto>> GetMySavedPosts(long tenantId);
        Task<bool> Save_Post(long userId, long postId);

        Task<IEnumerable<ProposalDto>> GetTenantProposalsAsync(long tenantId);


    }
}
