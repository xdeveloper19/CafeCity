

using System;

namespace Entities.ViewModels.ManageViewModels
{
    public class IndexViewModel: BaseResponseObject
    {
        public string MiddleName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool BrowserRemembered { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public bool IsLeader { get; set; }
        public Guid TeamId { get; set; }
    }
}
