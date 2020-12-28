using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Facility
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string PlaceId { get; set; }
        public string Name { get; set; }
        public string WorkSchedule { get; set; }
        public double Rate { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Description { get; set; }
        public List<UserFacility> UserFacilities { get; set; }
        public List<MediaData> Media { get; set; }
        public GeoData Geo { get; set; }
    }
}
