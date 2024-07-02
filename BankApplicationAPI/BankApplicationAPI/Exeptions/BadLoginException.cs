namespace BankApplicationAPI.Exeptions
{
    public class BadLoginException : Exception
    {
        public BadLoginException(string message) : base(message)
        {

        }
    }
}
