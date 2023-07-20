using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KanbanStyleBackEnd.Models.DataModels
{
    public class UserDto : BaseEntity
    {
        [StringLength(70)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string LinkedIn { get; set; } = string.Empty;
        public string GitHub { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public string UserType { get; set; } = "User";
    }
}
