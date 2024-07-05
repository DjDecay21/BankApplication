using BankApplicationAPI.Entities;
using BankApplicationAPI.Exeptions;
using BankApplicationAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            try
            {
                _accountService.CreateAccount(id);
                return Ok();
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("showAccounts/{id}")]
        public ActionResult<List<AccountDto>> Get([FromRoute] int id)
        {
            try
            {
                var accounts = _accountService.GetAccountsByUserId(id);

                if (accounts == null || accounts.Count == 0)
                {
                    return NotFound(new { message = $"The user is not found with {id} id." });
                }

                // Mapowanie Account na AccountDto jeśli jest taka potrzeba
                var accountDtos = accounts.Select(a => new AccountDto
                {
                    AccountNumber = a.AccountNumber,
                    Balance = a.Balance
                    // Dodaj inne właściwości, jeśli są w AccountDto
                }).ToList();

                return Ok(accountDtos);
            }
            catch (NotFoundAccount ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
