using Entities.ViewModels.AccountViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.FacilityViewModels
{
    public class AddPlaceViewModel
    {
        public double Rating { get; set; }
        public double Longitude { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public string PhotoData { get; set; }
        public string PhotoId { get; set; }
        public double Latitude { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public string UserId { get; set; }
    }
}
