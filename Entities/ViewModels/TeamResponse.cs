using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels
{
    public class TeamResponse: BaseResponseObject
    {
        public Guid TeamId { get; set; }
        public string Name { get; set; }
        public string Leader { get; set; }
    }
}
