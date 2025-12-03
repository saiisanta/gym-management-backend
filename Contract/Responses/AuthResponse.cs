namespace Contract.Responses
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public string Role { get; set; }
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string TelNumber { get; set; }
        public string Dni { get; set; }
        public string Genero { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string Direccion { get; set; }
        public string Estado { get; set; }
        public int? Plan { get; set; }
        public int? SucursalId { get; set; }
        public string Image { get; set; }
    }
}