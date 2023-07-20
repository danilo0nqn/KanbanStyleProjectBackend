using System.Text.Json.Serialization;

namespace KanbanStyleBackEnd.Models.DataModels
{
    public class ProjectUser
    {
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        [JsonIgnore]
        public Project Project { get; set; }
        [JsonIgnore]
        public User User { get; set; }
    }
}
