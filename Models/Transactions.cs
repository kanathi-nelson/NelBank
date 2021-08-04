using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace NelBank.Models
{
    public class Transactions
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("TransactionTypes")]
        public int? TransactionType { get; set; }
        
        [ForeignKey("Accounts")]
        public int? Account{ get; set; }

           [ForeignKey("User")]
        public int? UserId{ get; set; }

        public string ItemType { get; set; }

        public decimal? Amount { get; set; }
        public string DebitedAccount { get; set; }
        public string Bank { get; set; }
        public string CreditedAccount { get; set; }
        public bool Debit { get; set; }
        public bool IsInternal { get; set; }

        [Column(TypeName = "datetime2")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? TransactionDate { get; set; }

        [Column(TypeName = "datetime2")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:MM}")]
        public DateTime? TransactionTime { get; set; }

        public TransactionTypes TransactionTypes { get; set; }
        public Accounts Accounts { get; set; }
        public ApplicationUser User { get; set; }
    }
}
