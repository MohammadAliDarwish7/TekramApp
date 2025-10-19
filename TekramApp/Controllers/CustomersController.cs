using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TekramApp.Attribute;
using TekramApp.Interfaces;
using TekramApp.ViewModels;

namespace TekramApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpPost("register/dashboard")]
        [UsePermissions("Dashboard.RegisterCustomer")]
        public async Task<IActionResult> RegisterFromDashboard(CustomerRegisterDto dto)
        {
            try
            {
                var customer = await _customerService.RegisterFromDashboardAsync(dto);
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("register/mobile")]
        public async Task<IActionResult> RegisterFromMobile(CustomerRegisterDto dto)
        {
            try
            {
                var customer = await _customerService.RegisterFromMobileAsync(dto);
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null) return NotFound();
            return Ok(customer);
        }

        [HttpGet]
        [UsePermissions("Dashboard.ViewCustomers")]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }

        [HttpPut("{id}")]
        [UsePermissions("Dashboard.EditCustomer")]
        public async Task<IActionResult> Update(Guid id, CustomerRegisterDto dto)
        {
            var result = await _customerService.UpdateCustomerAsync(id, dto);
            if (!result) return NotFound();
            return Ok();
        }

        [HttpPost("addresses")]
        public async Task<IActionResult> AddAddress(AddressDto addressDto)
        {
            var result = await _customerService.AddAddressAsync(addressDto);
            if (!result) return BadRequest();
            return Ok();
        }

        [HttpGet("addresses/{customerId}")]
        public async Task<IActionResult> GetCustomerAddresses(Guid customerId)
        {
            var result = await _customerService.GetCustomerAddresses(customerId);
            if (result.Count == 0) return NotFound();
            return Ok(result);
        }

        [HttpPut("{customerId}/addresses/{addressId}")]
        public async Task<IActionResult> UpdateAddress(Guid customerId, Guid addressId, AddressDto addressDto)
        {
            var result = await _customerService.UpdateAddressAsync(customerId, addressId, addressDto);
            if (!result) return NotFound();
            return Ok();
        }

        [HttpDelete("{customerId}/addresses/{addressId}")]
        public async Task<IActionResult> DeleteAddress(Guid customerId, Guid addressId)
        {
            var result = await _customerService.DeleteAddressAsync(customerId, addressId);
            if (!result) return NotFound();
            return Ok();
        }

        [HttpPost("mobile/login")]
        public async Task<IActionResult> MobileLogin(CustomerLoginDto dto)
        {
            var tokenResponse = await _customerService.LoginMobileAsync(dto);
            if (tokenResponse == null) return Unauthorized("Invalid credentials");

            return Ok(tokenResponse);
        }

        [HttpPost("mobile/refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var tokenResponse = await _customerService.RefreshTokenAsync(refreshToken);
            if (tokenResponse == null) return Unauthorized("Invalid or expired refresh token");

            return Ok(tokenResponse);
        }
    }
}
