using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KanbanStyleBackEnd.DataAccess;
using KanbanStyleBackEnd.Models.DataModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace KanbanStyleBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectUsersController : ControllerBase
    {
        private readonly KanbanProjectDBContext _context;

        public ProjectUsersController(KanbanProjectDBContext context)
        {
            _context = context;
        }

        // GET: api/ProjectUsers
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<IEnumerable<ProjectUser>>> GetProjectUsers()
        {
          if (_context.ProjectUsers == null)
          {
              return NotFound();
          }
            return await _context.ProjectUsers.ToListAsync();
        }

        // GET: api/ProjectUsers/5
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<ProjectUser>> GetProjectUser(int id)
        {
          if (_context.ProjectUsers == null)
          {
              return NotFound();
          }
            var projectUser = await _context.ProjectUsers.FindAsync(id);

            if (projectUser == null)
            {
                return NotFound();
            }

            return projectUser;
        }

        // PUT: api/ProjectUsers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<IActionResult> PutProjectUser(int id, ProjectUser projectUser)
        {
            if (id != projectUser.ProjectId)
            {
                return BadRequest();
            }

            _context.Entry(projectUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectUserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ProjectUsers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Route("LinkUserToProject")]
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<ProjectUser>> LinkUserToProject(ProjectUserDto projectUser)
        {
          if (_context.ProjectUsers == null)
          {
              return Problem("Entity set 'KanbanProjectDBContext.ProjectUsers'  is null.");
          }
          //TODO: Send to Service!
            var project = await _context.Projects
                .Where(p => p.Id == projectUser.ProjectId)
                .FirstOrDefaultAsync();
            if (project == null)
            {
                throw new Exception("Project not found");
            }
            var user = await _context.Users
                .Where(u => u.Id == projectUser.UserId)
                .FirstOrDefaultAsync();
            if (user == null)
            {
                throw new Exception("User Not Found");
            }
            var newProjectUser = new ProjectUser
            {
                Project = project,
                UserId = projectUser.UserId,
                ProjectId = projectUser.ProjectId,
                User = user
            };

            _context.ProjectUsers.Add(newProjectUser);
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ProjectUserExists(projectUser.ProjectId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetProjectUser", new { id = projectUser.ProjectId }, newProjectUser);
        }

        // DELETE: api/ProjectUsers/5
        [HttpDelete]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<IActionResult> DeleteProjectUser(int projectId, int userId)
        {
            if (_context.ProjectUsers == null)
            {
                return NotFound();
            }
            var projectUser = await _context.ProjectUsers.FindAsync(projectId, userId);
            if (projectUser == null)
            {
                return NotFound();
            }

            _context.ProjectUsers.Remove(projectUser);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProjectUserExists(int id)
        {
            return (_context.ProjectUsers?.Any(e => e.ProjectId == id)).GetValueOrDefault();
        }
    }
}
