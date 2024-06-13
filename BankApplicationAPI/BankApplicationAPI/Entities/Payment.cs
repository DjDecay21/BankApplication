namespace BankApplicationAPI.Entities
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public string AccountNumber { get; set; }
        public double Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Description { get; set; }

        //Navi
        public virtual Account Account { get; set; }
    }
}
