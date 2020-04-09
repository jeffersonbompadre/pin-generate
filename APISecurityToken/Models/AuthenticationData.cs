using System.Collections.Generic;

namespace APISecurityToken.Models
{
    public class AuthenticationData
    {
        public string PINData { get; set; }
        public string UserName { get; set; }
        public ICollection<string> Roles { get; set; }
    }
}
