using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KanbanStyleBackEnd.Models.DataModels
{
    public class AssignmentDto : BaseEntity
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        public int BeingDoneById { get; set; }
        [Required]
        public int ProjectId { get; set; }
        public Priority Priority { get; set; } = Priority.Unknown;
        public AssignmentStage Stage { get; set; } = AssignmentStage.Pending;
        public string DescriptionURL { get; set; } = string.Empty;
        public string DesignURL { get; set; } = string.Empty;

    }
}