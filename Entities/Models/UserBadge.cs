using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class UserBadge
    {
        public Guid? BadgeId { get; set; }

        [MaxLength(450)]
        [Required]
        public string UserId { get; set; }
        public Badge Badge { get; set; }
    }
}
