using BankApplicationAPI.Entities;
using BankApplicationAPI.Exeptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace BankApplicationAPI.Services
{
    public interface IUserService
    {
        string Login(LoginUserDto dto);
        void RegisterUser(RegisterUserDto dto);
    }
    public class UserService : IUserService
    {
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly BankDbContext _bankDbContext;
        private readonly AuthenticationSettings _authenticationSettings;

        public UserService(BankDbContext context, IPasswordHasher<User> passwordHasher, AuthenticationSettings authenticationSettings)
        {
            _passwordHasher = passwordHasher;
            _bankDbContext = context;
            _authenticationSettings = authenticationSettings;
        }

        public string Login(LoginUserDto dto)
        {
            dto.Email = dto.Email.ToLower();
            
            var user = _bankDbContext.Users.FirstOrDefault(u => u.Email == dto.Email);
            if (user == null)
            {
                throw new BadLoginException("Invalid email or password1");
            }
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new BadLoginException("Invalid email or password2");
            }
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.NameIdentifier, $"{user.Email}"),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);


            var token = new JwtSecurityToken(_authenticationSettings.JwtIssuer,
                _authenticationSettings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: cred);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        public void RegisterUser(RegisterUserDto dto)
        {

            dto.FirstName = CapitalizeFirstLetter(dto.FirstName);
            dto.LastName = CapitalizeFirstLetter(dto.LastName);
            dto.Email = dto.Email.ToLower();

            ValidateUserRegistration(dto);

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

            _bankDbContext.Add(newUser);
            _bankDbContext.SaveChanges();
        }
        
        private void ValidateUserRegistration(RegisterUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FirstName) ||
                string.IsNullOrWhiteSpace(dto.LastName) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.PasswordHash) ||
                string.IsNullOrWhiteSpace(dto.PhoneNumber))
            {
                throw new BadRequestException("All fields are required.");
            }

            if (!Regex.IsMatch(dto.Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                throw new BadRequestException("Incorrect email.");
            }

            var doubleUser = _bankDbContext.Users.FirstOrDefault(u=>u.Email == dto.Email);
            if (doubleUser != null)
            {
                throw new BadRequestException("There is an account with the given email address ");
            }
            if (dto.PasswordHash != dto.RepeatPasswordHash)
            {
                throw new BadRequestException("The passwords are different.");
            }

            if (!IsPasswordStrong(dto.PasswordHash))
            {
                throw new BadRequestException("Password is not strong enough.");
            }
            if (!IsValidName(dto.FirstName) || !IsValidName(dto.LastName))
            {
                throw new BadRequestException("First name and last name can only contain letters without spaces or special characters.");
            }
        }
        private bool IsPasswordStrong(string password)
        {
            if (password.Length < 8) return false;

            if (!Regex.IsMatch(password, "[A-Z]")) return false;

            if (!Regex.IsMatch(password, "[a-z]")) return false;

            if (!Regex.IsMatch(password, "[0-9]")) return false;

            if (!Regex.IsMatch(password, "[^a-zA-Z0-9]")) return false;

            return true;
        }
        private string CapitalizeFirstLetter(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            return char.ToUpper(text[0]) + text.Substring(1).ToLower();
        }
        private bool IsValidName(string name)
        {
            return Regex.IsMatch(name, @"^[a-zA-Z]+$");
        }
    }

}
