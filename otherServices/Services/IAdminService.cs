using otherServices.Models;
using otherServices.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPIDotNet.DTOs;

namespace otherServices.Services
{
    public interface IAdminService
    {
        Task<Post> AcceptPost(long postId);
        Task<Post> RejectPost(long postId);
        
        Task<User> AcceptUser(long UserId);
        Task<User> RejectUser(long UserId);

        Task<IEnumerable<PostDTo>> GetWaitingPosts();

        Task<IEnumerable<UserDto>> GetWaitingLandlord();
        Task<IEnumerable<User>> GetLandlordStatus(long userid);

        Task<IEnumerable<User>> GetUsers();




    }
}