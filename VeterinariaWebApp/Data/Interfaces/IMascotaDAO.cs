using VeterinariaWebApp.Models.Mascota;

namespace VeterinariaWebApp.Data.Interfaces
{
    public interface IMascotaDAO
    {
        // Agregar una nueva mascota (Cliente registra su mascota)
        Task<bool> AgregarMascota(string nombre, string especie, string raza, DateTime fechaNac, long idUsuario);

        //Listar mascotas de un cliente específico (por ID de usuario)
        Task<List<Mascota>> ListarMascotasPorCliente(long idUsuario);

        // Actualizar datos de una mascota
        Task<bool> ActualizarMascota(long idMascota, string nombre, string especie, string raza, DateTime fechaNac);

        // Eliminar una mascota
        Task<string> EliminarMascota(long idMascota);
    }
}
