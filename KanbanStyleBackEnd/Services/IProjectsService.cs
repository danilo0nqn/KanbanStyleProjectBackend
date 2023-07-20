using KanbanStyleBackEnd.Models.DataModels;

namespace KanbanStyleBackEnd.Services
{
    public interface IProjectsService
    {
        
        Task<IEnumerable<Project>> GetProjectsByUser(int userId);
        
        
    }
}
