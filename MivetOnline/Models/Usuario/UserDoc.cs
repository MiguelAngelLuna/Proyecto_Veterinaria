using System.ComponentModel;

namespace MivetOnline.Models.Usuario
{
    public class UserDoc
    {
        [DisplayName("ID")]
        public long ide_doc { get; set; }

        [DisplayName("Tipo de Documento")]
        public string nom_doc { get; set; } = string.Empty;
    }
}
