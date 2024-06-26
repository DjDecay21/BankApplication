using BankApplicationAPI.Entities;
using BankApplicationAPI.Exeptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
            var user = _bankDbContext.Users.FirstOrDefault(u => u.Email== dto.Email);
            if (user == null)
            {
                throw new BadLoginExeption("Invalid email or password1");
            }
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if(result == PasswordVerificationResult.Failed)
            {
                throw new BadLoginExeption("Invalid email or password2");
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
            if (dto.PasswordHash != dto.RepeatPasswordHash)
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
                _bankDbContext.Add(newUser);
                _bankDbContext.SaveChanges();

                
            }
            else
            {
                throw new BadRequestExeption("The passwords are different");
            }
            
        }
    }
}
