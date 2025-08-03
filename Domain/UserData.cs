using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain
{
    public class UserData
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string? Bio { get; set; }
        public string? ImageUrl { get; set; }
        public ICollection<ActivityAttendee> Activities { get; set; } = [];
    }
}