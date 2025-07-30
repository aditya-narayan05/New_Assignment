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
            return await _context.Items
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Item> AddItemAsync(Item item)
        {
            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<bool> DeleteItemAsync(int id)
        {       //removed findasync()
            var item = new Item { Id = id }; // no need to fetch full entity , places a minimal placeholder for deletion
            _context.Items.Attach(item);     // attach a stub
            _context.Items.Remove(item);  //Stub Delete Just tells EF: “Hey, pretend this entity exists and remove it by key.”

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false; // item might not exist
            }
        }
    }
}