using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class User : IdentityUser
    {
        [MaxLength(70)]
        public string FirstName { get; set; }
        [MaxLength(70)]
        public string LastName { get; set; }
        public string MiddleName { get; set; }

        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public Guid TeamId { get; set; }
        public bool IsLeader { get; set; }
        //public bool IsInvited { get; set; }
    }
}
