using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KanbanStyleBackEnd.DataAccess;
using KanbanStyleBackEnd.Models.DataModels;
using KanbanStyleBackEnd.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using KanbanStyleBackEnd.Models.DTOs;

namespace KanbanStyleBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly KanbanProjectDBContext _context;
        private readonly IUsersService _usersService;

        public UsersController(KanbanProjectDBContext context, IUsersService usersService)
        {
            _context = context;
            _usersService = usersService;   
        }

        // GET: api/Users
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _usersService.GetAllUsers();
            if (users == null)
            {
                return NotFound();
            }
            return users;
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            if (_context.Users == null)
            {
              return NotFound();
            }
            var user = await _usersService.GetUserById(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpGet("User/{email}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<User>> GetUserByEmail(string email)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var userByEmail = await _usersService.FindUserByEmail(email);
            if (userByEmail == null)
            {
                throw new Exception("There isn't a user with this mail");
            }
            return userByEmail;
        }
        [Route("GetUsersByProject")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersByProject(int projectId)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var users = await _usersService.GetUsersByProject(projectId);
            if (users == null)
            {
                throw new Exception("There aren't users doing this project currently.");
            }
            return users;
        }

        [Route("PostUserAndUserLogin")]
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<User>> PostUserAndUserLogin(UserDto user)
        {
            await _usersService.CreateUserAndUserLogin(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }
        [Route("ChangeStatusMessage")]
        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<IActionResult> ChangeStatusMessage([FromBody] StatusMessageDto statusMessage, int userId)
        {
            await _usersService.ChangeStatusMessage(statusMessage.Message, userId);
            return NoContent();
        }


        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<IActionResult> PutUser(int id, UserDto user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }
            var userToModify = await _context.Users.FindAsync(id);
            if (userToModify == null)
            {
                NotFound();
            }

            userToModify.Name = user.Name;
            userToModify.Email = user.Email;
            userToModify.Password = user.Password;
            userToModify.Status = user.Status;
            userToModify.Avatar = user.Avatar;
            userToModify.UpdatedAt = DateTime.UtcNow;
            userToModify.GitHub = user.GitHub;
            userToModify.LinkedIn = user.LinkedIn;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<ActionResult<User>> PostUser(User user)
        {
          if (_context.Users == null)
          {
              return Problem("Entity set 'KanbanProjectDBContext.Users'  is null.");
          }
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            await _usersService.DeleteUser(user.Id);

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
