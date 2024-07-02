using BankApplicationAPI.Entities;
using BankApplicationAPI.Exeptions;
using BankApplicationAPI.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankApplicationAPI.Controllers
{

    [Route ("user/(controller]/")]
    public class UserController : ControllerBase
    {

        public interface IAccountService
        {
            void RegisterUser(RegisterUserDto dto);
            string GenerateJwt(LoginUserDto dto);
        }
        private readonly IUserService _accountService;


        private readonly BankDbContext _dbContext;
        public UserController(IUserService accountService, BankDbContext dbContext)
        {
            _accountService = accountService;
            _dbContext = dbContext;
        }
        [HttpPost("/register")]
        public ActionResult RegisterUser([FromBody] RegisterUserDto dto)
        {
            try
            {
                _accountService.RegisterUser(dto);
                return Ok();
            }
            catch (BadRequestException ex)
            {

                return BadRequest(new { message = ex.Message });
            }

        }
        [HttpPost("/login")]
        public ActionResult Login([FromBody] LoginUserDto dto)
        {
            try
            {
                string token = _accountService.Login(dto);
                return Ok(new { token });
            }
            catch (BadLoginExeption ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }
    }
}
