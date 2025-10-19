using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TekramApp.Interfaces;

namespace TekramApp.Controllers.Address
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public LocationsController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var countries = await _customerService.GetAllCountriesAsync();
            var cities = await _customerService.GetAllCitiesAsync();

            return Ok(new
            {
                Countries = countries,
                Cities = cities
            });
        }
    }
}
