using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TekramApp.Attribute;
using TekramApp.Interfaces;
using TekramApp.Models;

namespace TekramApp.Controllers.Shops
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShopsController : ControllerBase
    {
        private readonly IShopService _shopService;

        public ShopsController(IShopService shopService)
        {
            _shopService = shopService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var shops = await _shopService.GetAllShopsAsync();
            return Ok(shops);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var shop = await _shopService.GetShopByIdAsync(id);
            if (shop == null) return NotFound();
            return Ok(shop);
        }

        [HttpPost]
        [UsePermissions("ManageShops")]
        public async Task<IActionResult> Create([FromBody] Shop shop)
        {
            var created = await _shopService.CreateShopAsync(shop);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [UsePermissions("ManageShops")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Shop shop)
        {
            var updated = await _shopService.UpdateShopAsync(id, shop);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [UsePermissions("ManageShops")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _shopService.DeleteShopAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
