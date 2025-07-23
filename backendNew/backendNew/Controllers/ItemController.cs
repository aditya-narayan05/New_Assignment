using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backendNew.Model;
using backendNew.Repository;

namespace backendNew.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly IItem _itemRepo;

        public ItemController(IItem itemRepo)
        {
            _itemRepo = itemRepo;
        }

        [HttpGet]
        [Authorize(Roles = "User,Admin,SubAdmin")]
        public async Task<IActionResult> GetItems()
        {
            var items = await _itemRepo.GetItemsAsync();
            return Ok(items);
        }

        [HttpPost]
        [Authorize(Roles = "SubAdmin,Admin")]
        public async Task<IActionResult> AddItem([FromBody] Item item)
        {
            var newItem = await _itemRepo.AddItemAsync(item);
            return CreatedAtAction(nameof(GetItems), new { id = newItem.Id }, newItem);
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var result = await _itemRepo.DeleteItemAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}