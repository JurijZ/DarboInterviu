using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities
{
    public class ApplicationUserMap
    {
        [Key]
        public string Id { get; set; }
        public string ApplicationId { get; set; }
        public string UserId { get; set; }
    }
}