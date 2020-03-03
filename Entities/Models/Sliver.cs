using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Sliver
    {
        public Sliver()
        {
            this.UserSlivers = new List<UserSliver>();
        }
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Description { get; set; }
        public string Rang { get; set; }
        public string FileType { get; set; }
        public byte[] FileContent { get; set; }
        public List<UserSliver> UserSlivers { get; set; }
    }
}
