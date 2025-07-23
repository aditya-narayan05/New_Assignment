using backendNew.DataAccessLayer;
using backendNew.Model;
using Microsoft.EntityFrameworkCore;

namespace backendNew.Repository
{
    public class ItemRepo : IItem
    {
        private readonly AppDbContext _context;

        public ItemRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Item>> GetItemsAsync()
        {
            return await _context.Items.ToListAsync();
        }

        public async Task<Item> AddItemAsync(Item item)
        {
            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null) return false;

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}