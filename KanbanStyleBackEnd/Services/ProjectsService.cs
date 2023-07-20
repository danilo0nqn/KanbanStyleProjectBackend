using KanbanStyleBackEnd.DataAccess;
using KanbanStyleBackEnd.Models.DataModels;
using Microsoft.EntityFrameworkCore;

namespace KanbanStyleBackEnd.Services
{
    public class ProjectsService : IProjectsService
    {
        private readonly KanbanProjectDBContext _dbContext;
        private readonly IUsersService _usersService;
        public ProjectsService(KanbanProjectDBContext dbContext, IUsersService usersService)
        {
            _dbContext = dbContext;
            _usersService = usersService;
        }

        public async Task<IEnumerable<Project>> GetProjectsByUser(int userId)
        {
            var findUserByEmail = _dbContext.Users.Find(userId);

            var projects = await _dbContext.ProjectUsers
                .Where(pu => pu.UserId == findUserByEmail.Id)
                .Select(pu => pu.Project)
                .ToArrayAsync();

            if (projects == null)
            {
                throw new Exception("The user " + findUserByEmail.Name + " has not active projects currently");
            }
            return projects;

        }
    }    
}
