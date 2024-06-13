namespace BankApplicationAPI.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHasz { get; set; }

        //Navi
        public virtual List<Account> Accounts { get; set; }


    }
}
