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
using Microsoft.Build.Framework;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace KanbanStyleBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentsController : ControllerBase
    {
        private readonly KanbanProjectDBContext _context;
        //Service
        private readonly IAssignmentsService _assignmentsService;
        public AssignmentsController(KanbanProjectDBContext context, IAssignmentsService assignmentsService)
        {
            _context = context;
            _assignmentsService = assignmentsService;   
        }

        // GET: api/Assignments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Assignment>>> GetAssignments()
        {
          if (_context.Assignments == null)
          {
              return NotFound();
          }
            return await _context.Assignments.ToListAsync();
        }

        // GET: api/Assignments/5
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<Assignment>> GetAssignment(int id)
        {
          if (_context.Assignments == null)
          {
              return NotFound();
          }
            var assignment = await _context.Assignments.FindAsync(id);

            if (assignment == null)
            {
                return NotFound();
            }

            return assignment;
        }

        //Link to all Get AssignmentService methods
        
        [Route("GetUrgentAssignmentsByUser")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<IEnumerable<Assignment>>> GetUrgentAssignmentsByUser(int userId)
        {
            var urgentAssignmentsByUser = await _assignmentsService.GetUrgentAssignmentsByUser(userId);
            return urgentAssignmentsByUser.ToList();
        }

        [Route("GetAssignmentsByUser")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<IEnumerable<Assignment>>> GetAssignmentsByUser(int userId)
        {
            var assignmentsByUser = await _assignmentsService.GetAssignmentsByUser(userId);
            return assignmentsByUser.ToList();
        }
        
        [Route("GetAssignmentsByStage")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<IEnumerable<Assignment>>> GetAssignmentsByStage(AssignmentStage stageEnum)
        {
            var assignmentsByStage = await _assignmentsService.GetAssignmentsByStage(stageEnum);
            return assignmentsByStage.ToList();
        }
        [Route("GetAssignmentsByProject")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<IEnumerable<Assignment>>> GetAssignmentsByProject(string projectName)
        {
            var assignmentsByProject = await _assignmentsService.GetAssignmentsByProject(projectName);
            return assignmentsByProject.ToList();
        }
        [Route("AssignAssignmentToUser")]
        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<IActionResult> AssignAssignmentToUser(int assignmentId, int userId)
        {
            await _assignmentsService.AssignAssignmentToUser(assignmentId, userId);
            return NoContent();
        }
        [Route("ChangeAssignmentPriority")]
        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<IActionResult> ChangeAssignmentPriority(int assignmentId, int userId, Priority priority)
        {
            await _assignmentsService.ChangeAssignmentPriority(assignmentId, userId, priority);
            return NoContent();
        }
        [Route("ChangeAssignmentStage")]
        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<IActionResult> ChangeAssignmentStage(int assignmentId, int userId, AssignmentStage assignmentStage)
        {
            await _assignmentsService.ChangeAssignmentStage(assignmentId, userId, assignmentStage);
            return NoContent();
        }



        // PUT: api/Assignments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<IActionResult> PutAssignment(int assignmentId, int userId, AssignmentDto assignment)
        {
            if (assignment.Id != 0)
            {
                if (assignmentId != assignment.Id)
                {
                    return BadRequest();
                }
            }
            var assignmentToModify = await _context.Assignments.FindAsync(assignmentId);

            if (assignmentToModify == null)
            {
                NotFound();
            }
            assignmentToModify.Stage = assignment.Stage;
            assignmentToModify.UpdatedBy = _context.Users.Find(userId).Name;
            assignmentToModify.Description = assignment.Description;
            if (assignmentToModify.BeingDoneById != null && assignmentToModify.BeingDoneById != 0) 
            {
                assignmentToModify.BeingDoneById = assignment.BeingDoneById;
            }
            assignmentToModify.CreatedBy = assignment.CreatedBy;
            assignmentToModify.CreatedDate = assignment.CreatedDate;
            assignmentToModify.Name = assignment.Name;
            assignmentToModify.Priority = assignment.Priority;
            assignmentToModify.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

          
            return NoContent();
        }

        // POST: api/Assignments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<ActionResult<Assignment>> PostAssignment(AssignmentDto assignment)
        {
          if (_context.Assignments == null)
          {
              return Problem("Entity set 'KanbanProjectDBContext.Assignments'  is null.");
          }
            
            var project = await _context.Projects.FindAsync(assignment.ProjectId);

            if (project == null || assignment.ProjectId == 0)
            {
                throw new ArgumentException("Project id not found or not inputed. Please check project ID");
            }

            var newAssignment = new Assignment
            {
                CreatedDate = DateTime.Now,
                Id = assignment.Id,
                Name = assignment.Name,
                Description = assignment.Description,
                Priority = assignment.Priority,
                ProjectId = assignment.ProjectId,
                Stage = assignment.Stage,
                Project = project,
            };


            _context.Assignments.Add(newAssignment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAssignment", new { id = newAssignment.Id }, newAssignment);
        }

        // DELETE: api/Assignments/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator, User")]
        public async Task<IActionResult> DeleteAssignment(int id)
        {
            if (_context.Assignments == null)
            {
                return NotFound();
            }
            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }

            _context.Assignments.Remove(assignment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AssignmentExists(int id)
        {
            return (_context.Assignments?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
