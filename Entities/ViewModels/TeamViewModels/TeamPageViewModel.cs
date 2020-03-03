using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.TeamViewModels
{
    public class TeamPageViewModel: BaseResponseObject
    {
        public Guid TeamId { get; set; }
        public string LeaderId { get; set; }
        public string LeaderName { get; set; }
        public string TeamName { get; set; }
        public string Description { get; set; }
        public string City { get; set; }
        public DateTime Date { get; set; }
        public string Url { get; set; }
        public string Section { get; set; }
        public bool IsLeader { get; set; }
        public byte[] Symbol { get; set; }
       
    }
}
