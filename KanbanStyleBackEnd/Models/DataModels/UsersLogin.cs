using System.ComponentModel.DataAnnotations;

namespace KanbanStyleBackEnd.Models.DataModels
{
    public class UsersLogin
    {
        [Required, Key]
        public int id { get; set; }
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
