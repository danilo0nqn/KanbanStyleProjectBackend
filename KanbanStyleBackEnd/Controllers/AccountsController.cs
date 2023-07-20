using KanbanStyleBackEnd.DataAccess;
using KanbanStyleBackEnd.Helpers;
using KanbanStyleBackEnd.Models.DataModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KanbanStyleBackEnd.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly KanbanProjectDBContext _kanbanProjectDBContext;
        public AccountsController(JwtSettings jwtSettings, KanbanProjectDBContext kanbanProjectDBContext)
        {
            _jwtSettings = jwtSettings;
            _kanbanProjectDBContext = kanbanProjectDBContext;
        }


        [HttpPost]
        public IActionResult GetToken(UsersLoginDto usersLogin)
        {
            try
            {
                var Token = new UserTokens();
                var Login = _kanbanProjectDBContext.UsersLogins.FirstOrDefault(x => x.Email == usersLogin.Email);
                if(usersLogin == null)
                {
                    throw new Exception("User doesn't exist");
                };
                var user = _kanbanProjectDBContext.Users.FirstOrDefault(user => user.Email == usersLogin.Email);

                if (user != null && user.Password == usersLogin.Password)
                {
                    
                    Token = JwtHelpers.GenTokenKey(new UserTokens()
                    {
                        EmailId = user.Email,
                        Id = user.Id,
                        UserType = user.UserType,
                        GuidId = Guid.NewGuid(),

                    }, _jwtSettings);
                } else
                {
                    return BadRequest("Wrong Password");
                }
                return Ok(Token);
            }catch (Exception ex)
            {
                throw new Exception("GetToken Error", ex);
            }
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<IActionResult> GetUserList()
        {
            return Ok(await _kanbanProjectDBContext.UsersLogins.ToListAsync());
        }


    }
}
