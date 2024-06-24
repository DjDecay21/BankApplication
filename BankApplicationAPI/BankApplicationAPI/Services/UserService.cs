using BankApplicationAPI.Entities;
using BankApplicationAPI.Exeptions;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace BankApplicationAPI.Services
{
    public interface IUserService
    {
        void RegisterUser(RegisterUserDto dto);
    }
    public class UserService : IUserService
    {
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly BankDbContext _flightsDbContext;
        public UserService(BankDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _passwordHasher = passwordHasher;
            _flightsDbContext = context;
        }
        public void RegisterUser(RegisterUserDto dto)
        {
            if (dto.PasswordHash == dto.RepeatPasswordHash)
            {
                if (string.IsNullOrWhiteSpace(dto.FirstName) ||
                string.IsNullOrWhiteSpace(dto.LastName) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.PhoneNumber))
                {
                    throw new BadRequestExeption("All fields are required.");
                }
                if (!Regex.IsMatch(dto.Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                {
                    throw new BadRequestExeption("Incorrect email");
                }
                var newUser = new User()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    PasswordHash = dto.PasswordHash,
                    DateOfBirth = dto.DateOfBirth,
                    PhoneNumber = dto.PhoneNumber


                };
                var hashedPassword = _passwordHasher.HashPassword(newUser, dto.PasswordHash);
                newUser.PasswordHash = hashedPassword;
                _flightsDbContext.Add(newUser);
                _flightsDbContext.SaveChanges();

                
            }
            else
            {
                throw new BadRequestExeption("The passwords are different");
            }
            
        }
    }
}
