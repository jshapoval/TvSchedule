using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using TvSchedule.Data.Data;

namespace TvSchedule.Data.Entities
{
    public class Channel : IDbEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedUtc { get; set; }
        public string UpdateShowsLockSessionId { get; set; }
        public DateTime? UpdateShowsLockExpireUtc { get; set; }
        public DateTime? UpdatedUtc { get; set; }
        public string LockSessionId { get; set; }
        public DateTime? LockExpirationUtc { get; set; }
        public bool IsDeleted { get; set; }
        public int IdFromApi { get; set; }
        public string LastError { get; set; }
        public DateTime? LastShow { get; set; }

    }
}
