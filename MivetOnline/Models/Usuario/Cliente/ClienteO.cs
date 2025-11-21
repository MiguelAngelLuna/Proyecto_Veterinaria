using System.ComponentModel;

namespace MivetOnline.Models.Usuario
{
    public class ClienteO : UsuarioO
    {
        [DisplayName("ID CLIENTE")]
        public long ide_cli { get; set; }

        [DisplayName("ID USUARIO")]
        public long ide_usr { get; set; }
    }
}
