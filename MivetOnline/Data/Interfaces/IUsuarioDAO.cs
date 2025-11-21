using MivetOnline.Models.Usuario;

namespace MivetOnline.Data.Interfaces
{
    public interface IUsuarioDAO
    {
       
        Task<string?> VerificarLogin(string correo, string contraseña);

        // Obtener ID de usuario por correo 
        Task<long?> ObtenerIdUsuario(string correo);

        // Listar tipos de documento 
        Task<List<UserDoc>> ListarDocumentos();
    }
}
