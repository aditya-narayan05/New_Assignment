using backendNew.Model;

namespace backendNew.Repository
{
    public interface IUser
    {
        Task<List<User>> GetUsersAsync();

        Task<User> CreateUserAsync(User user);

        Task<User> UpdateUserAsync(User user);

        Task<bool> DeleteUserAsync(User user);
    }
}
