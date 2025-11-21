using System.ComponentModel;

namespace MivetOnline.Models.Usuario.Cliente
{
    public class Cliente : Usuario
    {
        [DisplayName("ID")]
        public long IdCliente { get; set; }
    }
}
