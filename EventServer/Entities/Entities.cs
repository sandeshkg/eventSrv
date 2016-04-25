using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EventServer.Entities
{
    public class Client
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string Secret { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public ApplicationTypes ApplicationType { get; set; }
        public bool Active { get; set; }
        public int RefreshTokenLifeTime { get; set; }
        [MaxLength(100)]
        public string AllowedOrigin { get; set; }
    }

    public class RefreshToken
    {
        [Key]
        public string Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Subject { get; set; }
        [Required]
        [MaxLength(50)]
        public string ClientId { get; set; }
        public DateTime IssuedUtc { get; set; }
        public DateTime ExpiresUtc { get; set; }
        [Required]
        public string ProtectedTicket { get; set; }
    }

    public enum ApplicationTypes
    {
        JavaScript = 0,
        NativeConfidential = 1
    }

    public class EventDetails
    {
        public int id { get; set; }
        public string title { get; set; }
        public string venue { get; set; }
        public string iconImageURL { get; set; }
        //public string LastUpdatedUser { get; set; }
        //public string LastUpdatedOn { get; set; }
        public string description { get; set; }
        public DateTime startTime { get; set; }
        public string type { get; set; }
        public string duration { get; set; }
    }
    public class SpecEventDetails
    {
        public int id { get; set; }
        public string lastUpdateOn { get; set; }
    }
}