using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter Name.")]
        public string Name { get; set; }

        //[StringLength(10, ErrorMessage = "Four-character password is recommended")]
        [Required(ErrorMessage = "Please enter Password.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter Email.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Range(1, 10, ErrorMessage = "Please select  Role.")]
        public int RoleId { get; set; }

        [Display(Name="Role Type")]
        public string RoleName { get; set; }

        public bool Active { get; set; }
    }
}
