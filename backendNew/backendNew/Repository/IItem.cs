using backendNew.Model;

namespace backendNew.Repository
{
    public interface IItem
    {
        Task<List<Item>> GetItemsAsync();
        Task<Item> AddItemAsync(Item item);
        Task<bool> DeleteItemAsync(int id);
    }
}