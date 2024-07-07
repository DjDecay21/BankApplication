using BankApplicationAPI.Entities;
using BankApplicationAPI.Exeptions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace BankApplicationAPI.Services
{
    public interface IAccountService
    {
        void CreateAccount(int id, string token);
        List<AccountDto> GetAccountsByUserId(int id, string token);
        List<TransferDto> GetTransferByAccountNumber(string accountNumber, string token);


    }
    public class AccountService : IAccountService
    {
        
        private readonly IAccountService _accountService;
        private readonly BankDbContext _dbContext;
        

        public AccountService(BankDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void CreateAccount(int id, string token)
        {
            if (!VerificationTokenById(id, token))
            {
                throw new BadTokenException(token);
            }
            var user = _dbContext.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null)
            {
                throw new BadRequestException("There is no such user ");
            }
            string newAccountNumber = GenerateAccountNumber();
            var duplicateAccount = _dbContext.Accounts.FirstOrDefault(u => u.AccountNumber == newAccountNumber);
            if (duplicateAccount == null)
            {
                var newAccount = new Account
                {
                    AccountNumber = newAccountNumber,
                    UserId = id,
                    Balance = 0.0m
                };
                _dbContext.Accounts.Add(newAccount);
                _dbContext.SaveChanges();

            }

        }
        static string GenerateAccountNumber()
        {
            Random accountNumber = new Random();
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < 16; i++)
            {
                int digit = accountNumber.Next(0, 10);
                sb.Append(digit);
            }

            return sb.ToString();
        }

        public List<AccountDto> GetAccountsByUserId(int userId, string token)
        {

            if(!VerificationTokenById(userId, token))
            {
                throw new BadTokenException(token);
            }
            var accounts = _dbContext.Accounts
                                     .Where(a => a.UserId == userId)
                                     .Select(account => new AccountDto
                                     {
                                         AccountNumber = account.AccountNumber,
                                         Balance = account.Balance
                                     })
                                     .ToList();
            if (accounts.Count == 0 || accounts == null)
            {
                throw new NotFoundAccount("User does not have an account.");
            }
            return accounts;
        }
        public List<TransferDto> GetTransferByAccountNumber(string accountNumber, string token)
        {
            if(!VerificationTokenByAccountNumber(accountNumber, token))
            {
                throw new BadTokenException(token);
            }
            var transfers = _dbContext.Transfers
                                     .Where(a => a.SourceAccountNumber == accountNumber || a.DestinationAccountNumber==accountNumber)
                                     .Select(transfers => new TransferDto
                                     {
                                         SourceAccountNumber = transfers.SourceAccountNumber,
                                         DestinationAccountNumber = transfers.DestinationAccountNumber,
                                         Amount = transfers.Amount,
                                         Description = transfers.Description,
                                         TransferDate = transfers.TransferDate
                                     })
                                     .ToList();
            if(transfers.Count == 0 || transfers == null)
            {
                throw new NotFoundAccount("Account does not have an transfer.");
            }
            return transfers;
        }
        private bool VerificationTokenById(int userId, string token)
        {
            var (header, payload) = DecodeToken(token);
            var (tokenId, email) = ExtractEmailAndId(payload);

            if (!int.TryParse(tokenId, out int tokenUserId))
            {
                throw new BadTokenException("Id from the token is not a valid integer.");
            }

            if (tokenUserId != userId)
            {
                throw new BadTokenException("UserId from the token does not match the requested userId.");
            }
            return true;

        }
        private bool VerificationTokenByAccountNumber(string numberAccount, string token)
        {
            var checker = _dbContext.Accounts.Where(a => a.AccountNumber == numberAccount)
                .FirstOrDefault();

            var (header, payload) = DecodeToken(token);
            var (tokenId, email) = ExtractEmailAndId(payload);

            if (!int.TryParse(tokenId, out int tokenUserId))
            {
                throw new BadTokenException("Id from the token is not a valid integer.");
            }

            if (checker.UserId != tokenUserId)
            {
                throw new BadTokenException("UserId from the token does not match the requested userId.");
            }
            return true;

        }
        private static (string headerDecoded, string payloadDecoded) DecodeToken(string token)
        {
            string[] parts = token.Split('.');
            if (parts.Length != 3)
            {
                throw new ArgumentException("Wrong token format.");
            }

            string headerEncoded = parts[0];
            string payloadEncoded = parts[1];

            string headerDecoded = Base64UrlDecode(headerEncoded);
            string payloadDecoded = Base64UrlDecode(payloadEncoded);

            return (headerDecoded, payloadDecoded);
        }

        private static string Base64UrlDecode(string input)
        {
            string output = input.Replace('-', '+').Replace('_', '/');
            switch (output.Length % 4)
            {
                case 2: output += "=="; break;
                case 3: output += "="; break;
                case 0: break;
                default: throw new ArgumentException("Invalid Base64Url string.");
            }

            byte[] base64Bytes = Convert.FromBase64String(output);
            return Encoding.UTF8.GetString(base64Bytes);
        }

        public static (string Id, string Email) ExtractEmailAndId(string payload)
        {
            var payloadJson = JsonDocument.Parse(payload);

            string id = payloadJson.RootElement.GetProperty("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")[0].GetString();
            string email = payloadJson.RootElement.GetProperty("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")[1].GetString();

            return (id, email);
        }



    }
}
