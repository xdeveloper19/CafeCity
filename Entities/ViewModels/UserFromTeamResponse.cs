using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels
{
    public class UserFromTeamResponse: BaseResponseObject
    {
        public string UserId { get; set; }
        public string FIO { get; set; }
        public string Rang { get; set; }
        public string Email { get; set; }
        public byte[] FileContent { get; set; }
    }
}
