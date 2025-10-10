using FoodOrder.Domain.Entities.Identity;
using FoodOrder.Domain.Interfaces;
using FoodOrder.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Infrastructure.Repositories
{
    internal class UserRepository : Repository<AppUser>, IUserRepository
    {
        private readonly UserManager<AppUser> _userManager;

        public UserRepository(FoodOrderDbContext context, UserManager<AppUser> userManager) : base(context)
        {
            _userManager = userManager;
        }

        public async Task<AppUser?> GetByIdAsync(int id)
        {
            return await _dbSet
               .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<(IEnumerable<AppUser> users, int totalCount)> GetCustomersAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            string? email = null,
            string? phoneNumber = null)
        {
            // Lấy tất cả users có role "Customer"
            var usersInRole = await _userManager.GetUsersInRoleAsync("Customer");
            var userIds = usersInRole.Select(u => u.Id).ToList();

            var query = _dbSet.Where(u => userIds.Contains(u.Id));

            // Áp dụng search filters
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(u =>
                    (u.Email != null && u.Email.Contains(searchTerm)) ||
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(searchTerm)) ||
                    (u.FullName != null && u.FullName.Contains(searchTerm)));
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                query = query.Where(u => u.Email != null && u.Email.Contains(email));
            }

            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                query = query.Where(u => u.PhoneNumber != null && u.PhoneNumber.Contains(phoneNumber));
            }

            var totalCount = await query.CountAsync();

            var users = await query
                .OrderBy(u => u.FullName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, totalCount);
        }

        public async Task<(IEnumerable<AppUser> users, int totalCount)> GetStaffAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            string? email = null,
            string? phoneNumber = null)
        {
            // Lấy tất cả users có role "Staff"
            var usersInRole = await _userManager.GetUsersInRoleAsync("Staff");
            var userIds = usersInRole.Select(u => u.Id).ToList();

            var query = _dbSet.Where(u => userIds.Contains(u.Id));

            // Áp dụng search filters
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(u =>
                    (u.Email != null && u.Email.Contains(searchTerm)) ||
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(searchTerm)) ||
                    (u.FullName != null && u.FullName.Contains(searchTerm)));
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                query = query.Where(u => u.Email != null && u.Email.Contains(email));
            }

            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                query = query.Where(u => u.PhoneNumber != null && u.PhoneNumber.Contains(phoneNumber));
            }

            var totalCount = await query.CountAsync();

            var users = await query
                .OrderBy(u => u.FullName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, totalCount);
        }
    }
}
