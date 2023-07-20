namespace KanbanStyleBackEnd.Models.DataModels
{
    public class ProjectUserDto: BaseEntity
    {
        public int ProjectId { get; set; }
        public int UserId { get; set; }
    }
}
