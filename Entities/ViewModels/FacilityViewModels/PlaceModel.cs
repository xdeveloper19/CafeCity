using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.FacilityViewModels
{
    public class PlaceModel
    {
        public double Rating { get; set; }
        public string Path { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string Descriptoin { get; set; }
        public string Id { get; internal set; }
    }
}
