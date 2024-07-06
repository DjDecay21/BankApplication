namespace BankApplicationAPI.Exeptions
{
    public class BadTokenException : Exception
    {
        public BadTokenException(string message) : base(message)
        {

        }

    }
}
