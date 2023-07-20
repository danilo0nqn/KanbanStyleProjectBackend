using KanbanStyleBackEnd.Models.DataModels;
using Microsoft.AspNetCore.Mvc;

namespace KanbanStyleBackEnd.Services
{
    public interface IAssignmentsService
    {
        Task<IEnumerable<Assignment>> GetAssignmentsByUser(int userId);
        Task<IEnumerable<Assignment>> GetUrgentAssignmentsByUser(int userId);
        Task<IEnumerable<Assignment>> GetAssignmentsByProject(string projectName);
        Task<IEnumerable<Assignment>> GetAssignmentsByStage(AssignmentStage stageEnum);
        Task<IActionResult> CreateNewAssignment(Assignment assignment, int userId, int projectId);
        Task<IActionResult> AssignAssignmentToUser(int assignmentId, int userId);
        Task<IActionResult> ChangeAssignmentPriority(int assignmentId, int userId, Priority priority);
        Task<IActionResult> ChangeAssignmentStage(int assignmentId, int userId, AssignmentStage assignmentStage);
    }
}
