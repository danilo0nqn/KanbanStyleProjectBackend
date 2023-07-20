using KanbanStyleBackEnd.Models.DataModels;
using Microsoft.AspNetCore.Mvc;

namespace KanbanStyleBackEnd.Services
{
    public interface IUsersService
    {
        Task<ActionResult<IEnumerable<User>>> GetAllUsers();
        Task<ActionResult<User>> GetUserById(int id);
        Task<ActionResult<User>> FindUserByEmail(string email);
        Task<ActionResult<IEnumerable<User>>> GetUsersByProject (int projectId);

        Task<IActionResult> CreateUserAndUserLogin(UserDto userDto);
        Task<IActionResult> ChangeStatusMessage(string message, int userId);
        Task<IActionResult> DeleteUser(int id);

    }
}
