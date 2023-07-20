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

namespace KanbanStyleBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly KanbanProjectDBContext _context;
        private readonly IProjectsService _projectsService;

        public ProjectsController(KanbanProjectDBContext context, IProjectsService projectsService)
        {
            _context = context;
            _projectsService = projectsService; 
        }

        // GET: api/Projects
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
          if (_context.Projects == null)
          {
              return NotFound();
          }
            return await _context.Projects.ToListAsync();
        }

        // GET: api/Projects/5
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
          if (_context.Projects == null)
          {
              return NotFound();
          }
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            return project;
        }
        
        //Get Methods in ProjectsService
        
        [Route("GetProjectsByUser")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjectsByUser(int userId)
        {
            var projectsByUser = await _projectsService.GetProjectsByUser(userId);
            return projectsByUser.ToList();
        }
        
        

        // PUT: api/Projects/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<IActionResult> PutProject(int id, Project project)
        {
            if (id != project.Id)
            {
                return BadRequest();
            }

            _context.Entry(project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
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

        // POST: api/Projects
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<Project>> PostProject(ProjectDto project, int creatorId)
        {
          if (_context.Projects == null)
          {
              return Problem("Entity set 'KanbanProjectDBContext.Projects'  is null.");
          }
            var creator = _context.Users.Find(creatorId);
            if (creator == null)
            {
                throw new Exception("Bad request, user Id inputed doesn't exist, check id.");
            }
            var newProject = new Project();
            newProject.Id = project.Id;
            newProject.Name = project.Name;
            newProject.Description = project.Description;
            newProject.CreatedDate = DateTime.UtcNow;
            newProject.CreatedBy = creator.Name;

            await _context.Projects.AddAsync(newProject);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProject", new { id = newProject.Id }, newProject);
        }

        // DELETE: api/Projects/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            if (_context.Projects == null)
            {
                return NotFound();
            }
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProjectExists(int id)
        {
            return (_context.Projects?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
