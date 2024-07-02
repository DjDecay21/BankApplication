using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankApplicationAPI.Entities
{
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string AccountNumber { get; set; }
        public int UserId { get; set; }
        public decimal Balance { get; set; }

        //Navi
        public User User { get; set; }
        public List<Transfer>TransfersOut { get; set; }
        public List<Transfer> TransfersIn { get; set; }

        public List<Payment> Payments { get; set; }

    }
}
