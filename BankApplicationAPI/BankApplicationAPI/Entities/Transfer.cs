namespace BankApplicationAPI.Entities
{
    public class Transfer
    {
        public int TransferId { get; set; }
        public string SourceAccountNumber { get; set; }
        public string DestinationAccountNumber { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public DateTime TransferDate { get; set; }

        //Navi
        public virtual Account SourceAccount { get; set; }
        public virtual Account DestinationAccount { get; set; }


    }
}
