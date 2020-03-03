using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.TeamManageViewModels
{
    public class EditTeamViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string City { get; set; }
        public DateTime Date { get; set; }
        public string Url { get; set; }
        public string Section { get; set; }
    }
}
