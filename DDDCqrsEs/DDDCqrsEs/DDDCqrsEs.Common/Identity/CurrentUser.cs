using DDDCqrsEs.Common.Constants;
using System;

namespace DDDCqrsEs.Common.Identity
{
    public class CurrentUser
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public Role? Role { get; set; }
    }
}
