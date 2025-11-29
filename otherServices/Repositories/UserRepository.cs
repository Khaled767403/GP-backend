using otherServices.Models;

namespace otherServices.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly AppDbContext2 _context;

        public UserRepository(AppDbContext2 context):base(context)
        {
            _context = context;
        }

        public async Task<User> AcceptUserAsync(long userId)
        {
            var User = await GetByIdAsync(userId);
            if (User == null)
                throw new KeyNotFoundException("User not found");

            User.FlagWaitingUser = 0;
            await SaveChangesAsync();
            return User;
        }

        public async Task<User> RejectUserAsync(long userId)
        {
            var User = await GetByIdAsync(userId);
            if (User == null)
                throw new KeyNotFoundException("User not found");

            User.FlagWaitingUser = 2;
            await SaveChangesAsync();
            return User;
        }
    }
}
