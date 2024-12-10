namespace Schemas
{
    public class LoginRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public required string Login { get; set; } // Ajout du modificateur 'required'
        public required string Password { get; set; } // Ajout du modificateur 'required'
    }
}
