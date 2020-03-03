using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Team
    {
        public Team()
        {
            this.Badges = new List<Badge>();
        }
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid BadgeId { get; set; }
        public string Leader { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string City { get; set; }
        public DateTime Date { get; set; }
        public string Url { get; set; }
        public string Section { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public List<Badge> Badges { get; set; }
    }
}
