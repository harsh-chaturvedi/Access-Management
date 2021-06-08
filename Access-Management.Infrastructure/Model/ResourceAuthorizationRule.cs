using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Access_Management.Infrastructure.Model
{
    public class ResourceAuthorizationRule
    {
        public Guid Id { get; set; }
        [Required]
        public Guid PolicyId { get; set; }
        [Required]
        public string Role { get; set; }
        [Required]
        public string Route { get; set; }
        [ForeignKey("PolicyId")]
        public virtual ResourceAuthorizationPolicy AuthorizationPolicy { get; set; }
    }
}