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
        public ActionResult<List<AccountDto>> ShowUserAccounts(int id)
        {
            string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                try
                {
                    var accounts = _accountService.GetAccountsByUserId(id, token);
                    return Ok(accounts);
                }
                catch (NotFoundAccount ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
        }

        [HttpGet("transfer")]
        public ActionResult<List<TransferDto>> ShowAcountTransfer(string numberAccount)
        {
            string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            try
            {   
                var transfers = _accountService.GetTransferByAccountNumber(numberAccount, token);

                if (transfers == null || transfers.Count == 0)
                {
                    return NotFound(new { message = "Account has no transactions " });
                }
                return Ok(transfers);
            }

            
            catch (NotFoundAccount ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
    }

}


