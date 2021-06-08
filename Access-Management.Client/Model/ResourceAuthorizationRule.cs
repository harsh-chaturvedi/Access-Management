using System;

namespace Access_Management.Client.Model
{
    public class ResourceAuthorizationRule
    {
        public Guid Id { get; set; }
        public Guid PolicyId { get; set; }
        public string Role { get; set; }
        public string Route { get; set; }
    }
}