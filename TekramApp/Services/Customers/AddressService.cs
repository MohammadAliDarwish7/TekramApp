using System;
using Microsoft.EntityFrameworkCore;
using TekramApp.Context;
using TekramApp.Interfaces;
using TekramApp.Models;
using TekramApp.ViewModels;

namespace TekramApp.Services.Customers
{
    public class AddressService 
    {
        private readonly TekramDbContext _context;

        public AddressService(TekramDbContext context)
        {
            _context = context;
        }

        public async Task<List<AddressDto>> GetAddressesByCustomerAsync(Guid customerId)
        {
            return await _context.Addresses
                .Where(a => a.CustomerId == customerId)
                .Select(a => new AddressDto
                {
                    Id = a.Id,
                    CustomerId = a.CustomerId,
                    AddressLine = a.AddressLine,
                    CountryId = a.CountryId,
                    CityId = a.CityId,
                    IsDefault = a.IsDefault
                })
                .ToListAsync();
        }

        public async Task<AddressDto> GetAddressByIdAsync(Guid id)
        {
            var a = await _context.Addresses.FindAsync(id);
            if (a == null) return null;
            return new AddressDto
            {
                Id = a.Id,
                CustomerId = a.CustomerId,
                AddressLine = a.AddressLine,
                CountryId = a.CountryId,
                CityId = a.CityId,
                IsDefault = a.IsDefault
            };
        }

        public async Task<AddressDto> CreateAddressAsync(AddressDto dto)
        {
            // Check if customer has any default address
            bool hasDefault = await _context.Addresses
                .AnyAsync(a => a.CustomerId == dto.CustomerId && a.IsDefault);

            // If no default exists, force this one to be default
            if (!hasDefault)
                dto.IsDefault = true;

            // If the new address is set as default, unset other defaults
            if (dto.IsDefault)
            {
                var others = _context.Addresses
                    .Where(a => a.CustomerId == dto.CustomerId && a.IsDefault);
                foreach (var o in others)
                    o.IsDefault = false;
            }

            var address = new Address
            {
                CustomerId = dto.CustomerId,
                AddressLine = dto.AddressLine,
                CountryId = dto.CountryId,
                CityId = dto.CityId,
                IsDefault = dto.IsDefault
            };

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
            dto.Id = address.Id;
            return dto;
        }

        public async Task<AddressDto> UpdateAddressAsync(Guid id, AddressDto dto)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null) return null;

            address.AddressLine = dto.AddressLine;
            address.CountryId = dto.CountryId;
            address.CityId = dto.CityId;

            // Handle default logic
            if (dto.IsDefault)
            {
                // Unset other default addresses for this customer
                var others = _context.Addresses
                    .Where(a => a.CustomerId == address.CustomerId && a.Id != id && a.IsDefault);
                foreach (var o in others)
                    o.IsDefault = false;
                address.IsDefault = true;
            }
            else
            {
                // Ensure at least one default remains
                bool hasOtherDefault = await _context.Addresses
                    .AnyAsync(a => a.CustomerId == address.CustomerId && a.Id != id && a.IsDefault);
                if (!hasOtherDefault)
                    address.IsDefault = true; // cannot unset if it's the only default
                else
                    address.IsDefault = false;
            }

            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<bool> DeleteAddressAsync(Guid id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null) return false;

            // Check if this is the only default
            bool isDefault = address.IsDefault;
            var otherAddresses = await _context.Addresses
                .Where(a => a.CustomerId == address.CustomerId && a.Id != id)
                .ToListAsync();

            if (isDefault && otherAddresses.Count > 0)
                otherAddresses[0].IsDefault = true; // make another default

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<List<CountryDto>> GetCountriesAsync()
        {
            return await _context.Countries
                .Select(c => new CountryDto { Id = c.Id, Name = c.Name })
                .ToListAsync();
        }

        public async Task<List<CityDto>> GetCitiesByCountryAsync(Guid countryId)
        {
            return await _context.Cities
                .Where(c => c.CountryId == countryId)
                .Select(c => new CityDto { Id = c.Id, Name = c.Name })
                .ToListAsync();
        }
    }

}
