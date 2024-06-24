namespace BankApplicationAPI.Entities
{
    public class RegisterUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public string RepeatPasswordHash { get; set; }
        public DateTime DateOfBirth { get; set; }

        public string PhoneNumber { get; set; }
    }
}
