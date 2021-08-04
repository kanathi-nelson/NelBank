using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NelBank.Viewmodels
{
    public class AccountTransferViewmodel
    {
          public string Bank { get; set; }
          public decimal Amount { get; set; }
          public string AccountNo { get; set; }
          public int? UserId { get; set; }
          public int AccountId { get; set; }
          public string FromAccount { get; set; }
    }
}
