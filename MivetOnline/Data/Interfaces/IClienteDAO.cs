using MivetOnline.Models.Usuario;
using MivetOnline.Models.Usuario.Cliente;

namespace MivetOnline.Data.Interfaces
{
    public interface IClienteDAO
    {
        // Agregar un nuevo cliente (Registro - Frontend)
        Task<bool> AgregarCliente(UsuarioO usuario);

        // Listar clientes para tabla frontend 
        Task<List<Cliente>> ListarClientesFront();

        // Listar clientes con datos completos 
        Task<List<ClienteO>> ListarClientesBack();

        // Actualizar datos del cliente
        Task<bool> ActualizarCliente(long id, UsuarioO usuario);

        // Buscar cliente por ID
        Task<Cliente?> BuscarCliente(long id);

        // Eliminar cliente
        Task<bool> EliminarCliente(long id);


        // Listar citas de un cliente específico (por ID de usuario)
        Task<List<CitaCliente>> ListarCitasPorCliente(long ideUsuario);
    }
}
