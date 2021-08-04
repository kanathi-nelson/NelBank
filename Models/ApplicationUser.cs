using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NelBank.Models
{


    public class ApplicationUser : IdentityUser<int>
    {

       public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime? DateofBirth { get; set; }

     
        public string Address { get; set; } 
        public string Address2 { get; set; } 
        public string PhoneNo { get; set; }   
      
       
        public string ConfirmationToken { get; set; }
        public string ResetToken { get; set; }


        [Column(TypeName = "datetime2")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime CreatedDate { get; set; }

        [Column(TypeName = "datetime2")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:MM}")]
        public DateTime CreatedTime { get; set; } 

        
        [Column(TypeName = "datetime2")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? ModifiedDate { get; set; }

        [Column(TypeName = "datetime2")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:MM}")]
        public DateTime? ModifiedTime { get; set; }


        public bool Deleted { get; set; }

        public bool Blocked { get; set; }

        [Column(TypeName = "datetime2")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? BlockedDate { get; set; }

        [Column(TypeName = "datetime2")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:MM}")]
        public DateTime? BlockedTime { get; set; }

        public int? BlockedById { get; set; }
      
        public int? ModifiedBy { get; set; }

        [ForeignKey("ApplicationRole")]
        public int? RoleId { get; set; }

        
        public virtual ICollection<Transactions> Transactions { get; set; }
        public virtual ICollection<Accounts> Accounts { get; set; }
        public virtual ApplicationRole ApplicationRole { get; set; }

    }


}
