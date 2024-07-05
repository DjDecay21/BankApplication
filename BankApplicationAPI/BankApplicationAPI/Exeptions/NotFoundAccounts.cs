namespace BankApplicationAPI.Exeptions
{
    public class NotFoundAccount : Exception
    {
        public NotFoundAccount(string message) : base(message)
        {

        }

    }
}
