using TekramApp.ViewModels;

namespace TekramApp.Interfaces
{
    public interface IAddressService
    {
        Task<List<AddressDto>> GetAddressesByCustomerAsync(Guid customerId);
        Task<AddressDto> GetAddressByIdAsync(Guid id);
        Task<AddressDto> CreateAddressAsync(AddressDto dto);
        Task<AddressDto> UpdateAddressAsync(Guid id, AddressDto dto);
        Task<bool> DeleteAddressAsync(Guid id);
        //Task<List<CountryDto>> GetCountriesAsync();
        //Task<List<CityDto>> GetCitiesByCountryAsync(Guid countryId);
    }
}
