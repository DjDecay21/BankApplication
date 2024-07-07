namespace BankApplicationAPI.Exeptions
{
    public class NotFoundTransfer : Exception
    {
        public NotFoundTransfer(string message) : base(message)
        {

        }

    }
}
