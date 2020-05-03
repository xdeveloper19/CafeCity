using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class MediaData
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid FacilityId { get; set; }
        public string Path { get; set; }
        public string Type { get; set; }
    }
}
