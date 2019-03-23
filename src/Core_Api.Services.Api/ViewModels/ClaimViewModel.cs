using System.Collections.Generic;

namespace Core_Api.Services.Api.ViewModels
{
    public class UserClaimsViewModel
    {
        public string aspnetuserid { get; set; }
        public List<ClaimViewModel> claims { get; set; }
    }
    public class ClaimViewModel
    {
        public string type { get; set; }
        public string value { get; set; }
    }
}