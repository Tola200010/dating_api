using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public record UserRegisterDto
    {
        [Required(ErrorMessage = "Username is Required")]
        public string? Username { get; set; }
        [Required]public string? KnownAs { get; set; }
         [Required]public string? Gender { get; set; }
         [Required]public string? City { get; set; }
         [Required]public string? Country { get; set; } 
         [Required]public DateTime DateOfBirth { get; set; }
        [Required(ErrorMessage = "Password is Required")]
        [StringLength(8,MinimumLength =4)]
        public string? Password { get; set; }
    }
}