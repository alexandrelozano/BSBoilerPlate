using System.ComponentModel.DataAnnotations;

namespace BSBoilerPlate.Models
{
    public class User
    {
        public int ID { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string GivenName { get; set; }
        public string? FirstSurname { get; set; }
        public string? SecondSurname { get; set; }
        public DateTime? DOB { get; set; }
        public DateTime? Inactive { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Value must be bigger than {1}")]
        public int RoleID { get; set; }
        public Role Role { get; set; }
        public string? Culture { get; set; }

        public override string ToString()
        {
            return UserName;
        }

        public string FullName
        {
            get { return $"{GivenName} {FirstSurname} {SecondSurname}".Trim(); }
        }
    }
}
