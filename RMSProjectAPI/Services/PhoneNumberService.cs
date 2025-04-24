// IPhoneNumberService.cs
using Microsoft.EntityFrameworkCore;
using RMSProjectAPI.Database;
using RMSProjectAPI.Database.Entity;
using RMSProjectAPI.Model;

namespace RMSProjectAPI.Services
{
    public interface IPhoneNumberService
    {
        Task<PhoneNumber> AddPhoneNumberAsync(Guid userId, PhoneNumberDto phoneNumberDto);
        Task<List<PhoneNumber>> GetUserPhoneNumbersAsync(Guid userId);
    }
}

// PhoneNumberService.cs
namespace RMSProjectAPI.Services
{
    public class PhoneNumberService : IPhoneNumberService
    {
        private readonly AppDbContext _context;

        public PhoneNumberService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PhoneNumber> AddPhoneNumberAsync(Guid userId, PhoneNumberDto phoneNumberDto)
        {
            var phoneNumber = new PhoneNumber
            {
                Id = Guid.NewGuid(),
                Number = phoneNumberDto.Number,
                UserId = userId
            };

            _context.PhoneNumbers.Add(phoneNumber);
            await _context.SaveChangesAsync();

            return phoneNumber;
        }

        public async Task<List<PhoneNumber>> GetUserPhoneNumbersAsync(Guid userId)
        {
            return await _context.PhoneNumbers
                .Where(pn => pn.UserId == userId)
                .ToListAsync();
        }
    }
}