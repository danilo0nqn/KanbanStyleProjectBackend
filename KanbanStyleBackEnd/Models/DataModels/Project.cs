using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KanbanStyleBackEnd.Models.DataModels
{
    public class Project : BaseEntity
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [JsonIgnore]
        public ICollection<ProjectUser> ProjectUsers { get; set; }

        [JsonIgnore]
        public ICollection<Assignment> Assignments { get; set; }
    }
}
