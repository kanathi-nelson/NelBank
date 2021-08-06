using System;
using System.Collections.Generic;
using System.Text;

namespace NelBank.Viewmodels
{
    public class AccountLoginResp
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public int? UserId { get; set; }
        public int AccountId { get; set; }
        public string AccountNo { get; set; }
        public string AccountType { get; set; }
        public string AccountBalance { get; set; }
        public string FromAccount { get; set; }
        public string Username { get; set; }
    }
}
