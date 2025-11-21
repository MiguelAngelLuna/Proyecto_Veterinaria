using System.ComponentModel;

namespace MivetOnline.Models.Usuario
{
    public class Usuario
    {
        [DisplayName("Nombre")]
        public string? NombreUsuario { get; set; }

        [DisplayName("Apellido")]
        public string? ApellidoUsuario { get; set; }

        [DisplayName("Fecha Nac.")]
        public DateTime FechaNacimiento { get; set; }

        [DisplayName("Tipo Doc.")]
        public string? TipoDocumento { get; set; }

        [DisplayName("Número Doc.")]
        public string? NumeroDocumento { get; set; }

        [DisplayName("Rol")]
        public string? Rol { get; set; }
    }
}
