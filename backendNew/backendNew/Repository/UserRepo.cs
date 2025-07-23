using System.Data.Entity;
using AutoMapper;
using backendNew.DataAccessLayer;
using backendNew.Model;
using Microsoft.EntityFrameworkCore;

namespace backendNew.Repository
{
    public class UserRepo : IUser
    {
        private readonly AppDbContext appDbContext;


        public UserRepo(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
   
        }

        public async Task<User> CreateUserAsync(User user)
        {
            // Ensure role is valid; default to "User" if not provided
            var allowedRoles = new[] { "User", "Admin", "SubAdmin" };

            if (string.IsNullOrWhiteSpace(user.Role))
            {
                user.Role = "User";
            }
            else if (!allowedRoles.Contains(user.Role))
            {
                throw new ArgumentException("Invalid role specified.");
            }

            appDbContext.Users.Add(user);
            await appDbContext.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteUserAsync(User user)
        {
            var existingUser = await appDbContext.Users.FindAsync(user.Id);

            if (existingUser == null)
                return false;

            appDbContext.Users.Remove(existingUser);
            await appDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            var users = await appDbContext.Users.ToListAsync();

            var safeUsers = users.Select(u => new User
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role
                
            }).ToList();

            return safeUsers;
        }
        public async Task<User> UpdateUserAsync(User user)
        {
            var existingUser = await appDbContext.Users.FindAsync(user.Id);

            if (existingUser == null)
                return null;

            existingUser.Name = user.Name;
            existingUser.Email = user.Email;
            existingUser.Password = user.Password; 
            existingUser.Role = user.Role; 

            await appDbContext.SaveChangesAsync();
            return existingUser;
        }
    }
}
