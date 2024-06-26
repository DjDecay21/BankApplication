namespace BankApplicationAPI.Exeptions
{
    public class BadLoginExeption : Exception
    {
        public BadLoginExeption(string message) : base(message)
        {

        }
    }
}
