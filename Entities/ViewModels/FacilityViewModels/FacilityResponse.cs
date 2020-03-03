using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.FacilityViewModels
{
    public class FacilityResponse: BaseResponseObject
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        //public Category Category { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
