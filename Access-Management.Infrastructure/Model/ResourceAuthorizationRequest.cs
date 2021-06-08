using System;
using System.ComponentModel.DataAnnotations;

namespace Access_Management.Infrastructure.Model
{
    public class ResourceAuthorizationRequest
    {
        [Required]
        public string ApplicationDomain { get; set; }

        public AccessSource AccessSource { get; set; }
        [Required]
        public string Route { get; set; }
    }
}