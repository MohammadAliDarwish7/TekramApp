using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using TekramApp.Context;
using TekramApp.Interfaces;
using TekramApp.Models;
using TekramApp.ViewModels;
using static TekramApp.Helpers.JWTHelper;

namespace TekramApp.Services.Customer
{
    public class CustomerService : ICustomerService
    {
        private readonly TekramDbContext _context;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IPasswordHasher<Customer> _passwordHasher;

        public CustomerService(TekramDbContext context, IPasswordHasher<Customer> passwordHasher, IJwtTokenService jwtTokenService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
        }
        public async Task<Customer> RegisterFromDashboardAsync(CustomerRegisterDto dto)
        {
            await ValidateCustomerDtoAsync(dto);

            var customer = new Customer
            {
                Name = dto.Name,
                Username = dto.Username,
                Email = dto.Email,
            };
            customer.PasswordHash = _passwordHasher.HashPassword(customer, dto.Password);

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }
        public async Task<Customer> RegisterFromMobileAsync(CustomerRegisterDto dto)
        {
            // Mobile-specific logic (like default status or extra fields)
            return await RegisterFromDashboardAsync(dto);
        }
        public async Task<Customer?> GetCustomerByIdAsync(Guid id)
        {
            return await _context.Customers
                .Include(c => c.Addresses)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _context.Customers.Include(c => c.Addresses).ToListAsync();
        }
        public async Task<bool> UpdateCustomerAsync(Guid id, CustomerRegisterDto dto)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return false;

            customer.Name = dto.Name;
            customer.Username = dto.Username;
            customer.Email = dto.Email;

            if (!string.IsNullOrWhiteSpace(dto.Password))
                customer.PasswordHash = _passwordHasher.HashPassword(customer, dto.Password);

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> AddAddressAsync(Guid customerId, AddressDto addressDto)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null) return false;

            if (addressDto.IsDefault)
            {
                // unset previous default addresses
                var existingDefaults = await _context.Addresses
                    .Where(a => a.CustomerId == customerId && a.IsDefault)
                    .ToListAsync();
                foreach (var addr in existingDefaults)
                    addr.IsDefault = false;
            }

            var address = new Address
            {
                CustomerId = customerId,
                AddressLine = addressDto.AddressLine,
                //City = addressDto.City,
                //Country = addressDto.Country,
                IsDefault = addressDto.IsDefault
            };

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateAddressAsync(Guid customerId, Guid addressId, AddressDto addressDto)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.CustomerId == customerId && a.Id == addressId);
            if (address == null) return false;

            if (addressDto.IsDefault && !address.IsDefault)
            {
                // unset previous default addresses
                var existingDefaults = await _context.Addresses
                    .Where(a => a.CustomerId == customerId && a.IsDefault)
                    .ToListAsync();
                foreach (var addr in existingDefaults)
                    addr.IsDefault = false;
            }

            address.AddressLine = addressDto.AddressLine;
            //address.City = addressDto.City;
            //address.Country = addressDto.Country;
            address.IsDefault = addressDto.IsDefault;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteAddressAsync(Guid customerId, Guid addressId)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.CustomerId == customerId && a.Id == addressId);
            if (address == null) return false;

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<TokenResponseDto?> LoginMobileAsync(CustomerLoginDto dto)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Username == dto.UsernameOrEmail || c.Email == dto.UsernameOrEmail);

            if (customer == null) return null;

            var result = _passwordHasher.VerifyHashedPassword(customer, customer.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed) return null;

            // Generate access token (short-lived)
            var accessToken = _jwtTokenService.GenerateToken(customer, TimeSpan.FromHours(1));

            // Generate refresh token (long-lived)
            var refreshToken = new RefreshToken
            {
                CustomerId = customer.Id,
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.UtcNow.AddDays(30)
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return new TokenResponseDto
            {
                AccessToken = accessToken.Token,
                AccessTokenExpires = accessToken.Expires,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpires = refreshToken.Expires
            };
        }
        public async Task<TokenResponseDto?> RefreshTokenAsync(string refreshToken)
        {
            var existingToken = await _context.RefreshTokens
                .Include(rt => rt.Customer)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsRevoked);

            if (existingToken == null || existingToken.Expires < DateTime.UtcNow)
                return null;

            // Generate new access token
            var accessToken = _jwtTokenService.GenerateToken(existingToken.Customer, TimeSpan.FromHours(1));

            return new TokenResponseDto
            {
                AccessToken = accessToken.Token,
                AccessTokenExpires = accessToken.Expires,
                RefreshToken = existingToken.Token,
                RefreshTokenExpires = existingToken.Expires
            };
        }
        private async Task ValidateCustomerDtoAsync(CustomerRegisterDto dto)
        {
            if (!new EmailAddressAttribute().IsValid(dto.Email))
                throw new ArgumentException("Invalid email format");

            if (dto.Password.Length < 8)
                throw new ArgumentException("Password must be at least 8 characters");

            if (!Regex.IsMatch(dto.Password, @"[A-Z]"))
                throw new ArgumentException("Password must contain an uppercase letter");

            if (!Regex.IsMatch(dto.Password, @"[a-z]"))
                throw new ArgumentException("Password must contain a lowercase letter");

            if (!Regex.IsMatch(dto.Password, @"[0-9]"))
                throw new ArgumentException("Password must contain a digit");

            if (await _context.Customers.AnyAsync(c => c.Email == dto.Email))
                throw new ArgumentException("Email already exists");

            if (await _context.Customers.AnyAsync(c => c.Username == dto.Username))
                throw new ArgumentException("Username already exists");
        }
    }
}
