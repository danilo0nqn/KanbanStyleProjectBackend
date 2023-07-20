using KanbanStyleBackEnd.DataAccess;
using KanbanStyleBackEnd.Models.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KanbanStyleBackEnd.Services
{
    public class AssignmentsService : IAssignmentsService
    {
        private readonly KanbanProjectDBContext _dbContext;
        private readonly IUsersService _usersService;
        private readonly IProjectsService _projectsService;
        public AssignmentsService(KanbanProjectDBContext dbContext, IUsersService usersService, IProjectsService projectsService)
        {
            _dbContext = dbContext;
            _usersService = usersService;
            _projectsService = projectsService;
        }

        public async Task<IActionResult> AssignAssignmentToUser(int assignmentId, int userId)
        {
            if (_dbContext.Users.Find(userId) == null)
            {
                throw new Exception("User not found");
            }
            if (_dbContext.Assignments.Find(assignmentId) == null)
            {
                throw new Exception("Assignment not found");
            }
            var assignmentToAssign = _dbContext.Assignments.Find(assignmentId);
            assignmentToAssign.BeingDoneById = userId;
            if(assignmentToAssign.Stage == AssignmentStage.Pending)
            {
                assignmentToAssign.Stage = AssignmentStage.Ongoing;
            }
            var userToBeAssigned = _dbContext.Users.Find(userId);
            assignmentToAssign.BeingDoneBy = userToBeAssigned;
            await _dbContext.SaveChangesAsync();

            return new OkResult();
        }

        public async Task<IActionResult> ChangeAssignmentPriority(int assignmentId, int userId, Priority priority)
        {
            if (_dbContext.Users.Find(userId) == null)
            {
                throw new Exception("User not found");
            }
            if (_dbContext.Assignments.Find(assignmentId) == null)
            {
                throw new Exception("Assignment not found");
            }
            var assignmentToChange = _dbContext.Assignments.Find(assignmentId);
            assignmentToChange.Priority = priority;
            assignmentToChange.UpdatedBy = _dbContext.Users.Find(userId).Email;
            assignmentToChange.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return new OkResult();
        }

        public async Task<IActionResult> ChangeAssignmentStage(int assignmentId, int userId, AssignmentStage assignmentStage)
        {
            if (_dbContext.Users.Find(userId) == null)
            {
                throw new Exception("User not found");
            }
            if (_dbContext.Assignments.Find(assignmentId) == null)
            {
                throw new Exception("Assignment not found");
            }
            var assignmentToChange = _dbContext.Assignments.Find(assignmentId);
            assignmentToChange.Stage = assignmentStage;
            assignmentToChange.UpdatedBy = _dbContext.Users.Find(userId).Email;
            assignmentToChange.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return new OkResult();
        }

        public Task<IActionResult> CreateNewAssignment(Assignment assignment, int userId, int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Assignment>> GetAssignmentsByProject(string projectName)
        {
            var assignmentsByProject = await _dbContext.Assignments
                .Where(assignment => assignment.Project.Name == projectName)
                .ToArrayAsync();

            if(!assignmentsByProject.Any())
            {
                throw new Exception("There is no assignments in that project");
            }
            return assignmentsByProject;
        }

        public async Task<IEnumerable<Assignment>> GetAssignmentsByStage(AssignmentStage stageEnum)
        {
            var assignmentsByStage = await _dbContext.Assignments
                .Where(assignment => assignment.Stage == stageEnum)
                .ToArrayAsync();
            if(!assignmentsByStage.Any())
            {
                throw new Exception("There aren't assignments in "+ stageEnum + "stage");
            }
            return assignmentsByStage;
        }

        public async Task<IEnumerable<Assignment>> GetAssignmentsByUser(int userId)
        {
            var findUserByEmail = _dbContext.Users.Find(userId);
            if(findUserByEmail == null)
            {
                throw new Exception("Wrong userId sent. Check for correct UserId int parameter");
            };

            var assignmentsByUser = await _dbContext.Assignments
                .Where(assignment => assignment.BeingDoneBy.Id == userId)
                .ToArrayAsync();
            if(!assignmentsByUser.Any())
            {
                throw new Exception("The user " + findUserByEmail.Name + "has not assignments pending");
            }
            return assignmentsByUser;
        }

        public async Task<IEnumerable<Assignment>> GetUrgentAssignmentsByUser(int userId)
        {
            var findUserByEmail = _dbContext.Users.Find(userId);
            var urgentAssignmentsByUser = await _dbContext.Assignments
                .Where(assignment => assignment.BeingDoneBy.Id == userId && assignment.Priority == Priority.Urgent)
                .ToArrayAsync();
            if(!urgentAssignmentsByUser.Any())
            {
                throw new Exception(findUserByEmail.Name + "has not urgent assignments");
            }
            return urgentAssignmentsByUser;
        }

        
    }
}
