using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public record UserLoginDto
    {
        [Required(ErrorMessage = "Username is Required")]
        public string? Username { get; set; }
        [Required(ErrorMessage = "Password is Required")]
        public string? Password { get; set; }
    }
}