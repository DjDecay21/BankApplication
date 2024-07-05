using BankApplicationAPI.Entities;
using BankApplicationAPI.Exeptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace BankApplicationAPI.Services
{
    public interface IAccountService
    {
        //Added veryfication JWT
        void CreateAccount(int id);
        List<Account> GetAccountsByUserId(int id);
    }
    public class AccountService : IAccountService
    {
        
        private readonly IAccountService _accountService;
        private readonly BankDbContext _dbContext;
        

        public AccountService(BankDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void CreateAccount(int id)
        {
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

        public List<Account> GetAccountsByUserId(int userId)
        {
            // Pobranie wszystkich kont przypisanych do określonego userId
            var accounts = _dbContext.Accounts
                                     .Where(a => a.UserId == userId)
                                     .Select(account => new Account
                                     {
                                         AccountNumber = account.AccountNumber,
                                         Balance = account.Balance
                                     })
                                     .ToList();

            // Logowanie lub inna akcja w przypadku pustej listy
            if (accounts.Count == 0)
            {
                Console.WriteLine($"Nie znaleziono kont dla użytkownika o ID: {userId}");
            }

            return accounts;
        }


    }
}
