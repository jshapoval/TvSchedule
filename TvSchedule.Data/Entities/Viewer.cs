using System;
using System.Collections.Generic;
using System.Text;
using TvSchedule.Data.Data;

namespace TvSchedule.Data.Entities
{
    public class Viewer : IDbEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DOB { get; set; }
        public string Description { get; set; }
        public string DescriptionForTest { get; set; }
        public DateTime CreatedUtc { get; set; }
        public string UpdateShowsLockSessionId { get; set; }
        public DateTime UpdateShowsLockExpireUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
        public string LockSessionId { get; set; }
        public DateTime? LockExpirationUtc { get; set; }
    }
}
