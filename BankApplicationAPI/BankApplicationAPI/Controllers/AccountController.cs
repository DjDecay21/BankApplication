﻿using BankApplicationAPI.Entities;
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
        //[HttpGet("showAccounts/")]
        //public ActionResult<AccountDto> Get([FromRoute] int id)
        //{
        //    try
        //    {
        //        _accountService.
        //    }
        //    return Ok();
        //}
        }
}
