using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain
{
    public class ActivityAttendee
    {
        public string? UserId { get; set; }
        public UserData UserData { get; set; } = null!;
        public Guid ActivityId { get; set; }
        public Activity Activity { get; set; } = null!;
        public bool IsHost { get; set; }
        public DateTime DateJoined { get; set; } = DateTime.UtcNow;
    }
}