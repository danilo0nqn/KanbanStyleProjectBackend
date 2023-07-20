using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KanbanStyleBackEnd.Models.DataModels
{
    public class User : BaseEntity
    {
        [Required, StringLength(70)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string LinkedIn { get; set; } = string.Empty;
        public string GitHub { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        [Required]
        public string UserType { get; set; } = "User";

        [JsonIgnore]
        public ICollection<ProjectUser> ProjectUsers { get; set; }
        [JsonIgnore]
        public ICollection<Assignment> Assignments { get; set; }
    }
}
