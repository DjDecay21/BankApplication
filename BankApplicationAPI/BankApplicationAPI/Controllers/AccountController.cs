using BankApplicationAPI.Entities;
using BankApplicationAPI.Exeptions;
using BankApplicationAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BankApplicationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("createAccount/{id}")]
        public ActionResult<Account> CreateAccount(int id)
        {
            string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            try
            {
                try
                {
                    _accountService.CreateAccount(id, token);
                    return Ok();
                }
                catch (BadRequestException ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }
            catch(BadTokenException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            
        }
        [HttpGet("showAccounts/{id}")]
        public ActionResult<List<AccountDto>> Get([FromRoute] int id)
        {
            string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            try
            {
                try
                {
                    var accounts = _accountService.GetAccountsByUserId(id, token);

                    if (accounts == null || accounts.Count == 0)
                    {
                        return NotFound(new { message = $"The user is not found with {id} id." });
                    }

                    var accountDtos = accounts.Select(a => new AccountDto
                    {
                        AccountNumber = a.AccountNumber,
                        Balance = a.Balance
                    }).ToList();

                    return Ok(accountDtos);
                }
                catch (NotFoundAccount ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }
            catch (BadTokenException ex)
            {
                return BadRequest(new { message = ex.Message, });
            }
        }

    }
}
