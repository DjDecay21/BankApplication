namespace BankApplicationAPI.Entities
{
    public class TransferDto
    {
        public string SourceAccountNumber { get; set; }
        public string DestinationAccountNumber { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public DateTime TransferDate { get; set; }
    }
}
