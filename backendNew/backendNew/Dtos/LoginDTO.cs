using System.ComponentModel.DataAnnotations;

namespace backendNew.Dtos
{
    public class LoginDTO
    {
        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
