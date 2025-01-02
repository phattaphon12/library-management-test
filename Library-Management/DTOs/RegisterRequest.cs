namespace Library_Management.DTOs
{
    using System.ComponentModel.DataAnnotations;
    public class RegisterRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; }
        [Required]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string Phone { get; set; }
        public string Role { get; set; } = "User"; // ค่าเริ่มต้นเป็น User
    }
}