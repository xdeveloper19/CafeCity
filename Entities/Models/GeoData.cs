using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class GeoData
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid FacilityId { get; set; }
        public Facility Place { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Address { get; set; }

    }
}
