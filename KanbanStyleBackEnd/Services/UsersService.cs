using KanbanStyleBackEnd.Controllers;
using KanbanStyleBackEnd.DataAccess;
using KanbanStyleBackEnd.Models.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace KanbanStyleBackEnd.Services
{
    public class UsersService : IUsersService
    {
        private readonly KanbanProjectDBContext _context;
        public UsersService(KanbanProjectDBContext context)
        {
            _context = context;
        }

        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _context.Users.ToArrayAsync();
            return users;

        }

        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _context.Users.SingleOrDefaultAsync(user => user.Id == id);
            return user;
        }

        public async Task<ActionResult<User>> FindUserByEmail(string email)
        {
            var findUserByEmail = await _context.Users
                .Where(user => user.Email == email)
                .FirstOrDefaultAsync();
            if (findUserByEmail == null)
            {
                throw new Exception("There is no user with this email: " + email);
            }
            return findUserByEmail;
        }

        public async Task<ActionResult<IEnumerable<User>>> GetUsersByProject(int projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            var users = await _context.ProjectUsers
                .Where(project => project.ProjectId == projectId)
                .Select(users => users.User)
                .ToArrayAsync();
            if (users == null)
            {
                throw new Exception("The project " + project.Name + " doesn't have users");
            }

            return users;
        }

        public async Task<IActionResult> CreateUserAndUserLogin(UserDto userDto)
        {
            var findUserByEmail = await _context.Users
                .Where(userInUsers=>userInUsers.Email == userDto.Email)
                .FirstOrDefaultAsync();

            if(findUserByEmail == null)
            {
                var newUser = new User
                {
                    Name = userDto.Name,
                    LastName = userDto.LastName,
                    Email = userDto.Email,
                    Password = userDto.Password,
                    Status = userDto.Status,
                    Avatar = userDto.Avatar,
                    CreatedBy = "From Website",
                    CreatedDate = DateTime.UtcNow,
                    GitHub = userDto.GitHub,
                    LinkedIn = userDto.LinkedIn,
                    UserType = userDto.UserType,
                };
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                var newUserCreated = await FindUserByEmail(newUser.Email);
                var newUserLogin = new UsersLogin
                {
                    Email = userDto.Email,
                    Password = userDto.Password,
                };

                _context.UsersLogins.Add(newUserLogin);
                await _context.SaveChangesAsync();

                return new OkResult();
            }
            throw new Exception("There already is a user with this mail: " +  userDto.Email);
        }


        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            
            if (user.ProjectUsers == null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return new OkResult();
            }
            var projectUser = _context.ProjectUsers
                .Where(pu => pu.UserId == user.Id)
                .FirstOrDefault();
            var project = _context.Projects
                .Where(p => p.Id == projectUser.ProjectId)
                .FirstOrDefault();
            var assignments = _context.Assignments
                .Where(a => a.BeingDoneBy.Id == user.Id)
                .ToArray();

            foreach (Assignment assignment in assignments)
            {
                assignment.BeingDoneBy = null;
            }
            project.ProjectUsers = null;
            _context.ProjectUsers.Remove(projectUser);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            
            return new OkResult();
        }

        public async Task<IActionResult> ChangeStatusMessage(string message, int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if(user == null)
            {
                throw new Exception("User not found or disconnected");
            }
            user.Status = message;
            await _context.SaveChangesAsync();
            return new OkResult();
        }
    }
}
