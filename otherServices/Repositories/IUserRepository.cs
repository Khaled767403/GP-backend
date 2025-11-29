using otherServices.Models;

namespace otherServices.Repositories
{
    public interface IUserRepository:IGenericRepository<User>
    {
        Task<User> AcceptUserAsync(long userId);
        Task<User> RejectUserAsync(long userId);

    }
}
