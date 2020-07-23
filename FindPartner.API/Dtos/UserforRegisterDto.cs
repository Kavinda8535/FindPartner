using System.ComponentModel.DataAnnotations;

namespace FindPartner.API.Dtos
{
    public class UserforRegisterDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "You must specify passowrd between 4 and 8 charactors. ")]
        public string Password { get; set; }
    }
}