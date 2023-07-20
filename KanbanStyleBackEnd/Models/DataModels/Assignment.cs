using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KanbanStyleBackEnd.Models.DataModels
{
    public enum Priority
    {
        Unknown,
        Minor, 
        Medium,
        Major,
        Urgent
    }
    public enum AssignmentStage
    {
        Pending,
        Ongoing,
        ForReview,
        Completed
    }
    public class Assignment : BaseEntity
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [JsonIgnore]
        public User? BeingDoneBy { get; set; } 
        public int? BeingDoneById { get; set; }
        [JsonIgnore]
        public Project Project { get; set; }
        [Required]
        public int ProjectId { get; set; }
        public string DescriptionURL { get; set; } = string.Empty;
        public string DesignURL { get; set; } = string.Empty;
        public Priority Priority { get; set; } = Priority.Unknown;
        public  AssignmentStage Stage { get; set; } = AssignmentStage.Pending;
        
    }
}
