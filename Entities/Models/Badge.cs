using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Badge
    {
        public Badge()
        {
            this.UserBadges = new List<UserBadge>();
        }
        public Guid Id { get; set; } = Guid.NewGuid(); // Guid Guid.NewGuid() new Guid("")
        public string Title { get; set; }
        public string Description { get; set; }
        public string FileType { get; set; }
        public Guid TeamId { get; set; }
        public byte[] FileContent { get; set; }
        public List<UserBadge> UserBadges { get; set; }
        public Team Team { get; set; }
    }
}
