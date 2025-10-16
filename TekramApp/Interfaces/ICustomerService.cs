using TekramApp.Models;
using TekramApp.ViewModels;

namespace TekramApp.Interfaces
{
    public interface ICustomerService
    {
        // Registration
        Task<Customer> RegisterFromDashboardAsync(CustomerRegisterDto dto);
        Task<Customer> RegisterFromMobileAsync(CustomerRegisterDto dto);

        // Get
        Task<Customer?> GetCustomerByIdAsync(Guid id);
        Task<IEnumerable<Customer>> GetAllCustomersAsync();

        // Update
        Task<bool> UpdateCustomerAsync(Guid id, CustomerRegisterDto dto);

        // Address Management
        Task<bool> AddAddressAsync(Guid customerId, AddressDto addressDto);
        Task<bool> UpdateAddressAsync(Guid customerId, Guid addressId, AddressDto addressDto);
        Task<bool> DeleteAddressAsync(Guid customerId, Guid addressId);

        Task<TokenResponseDto?> LoginMobileAsync(CustomerLoginDto dto);
        Task<TokenResponseDto?> RefreshTokenAsync(string refreshToken);
    }
}
