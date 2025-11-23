using MivetOnline.Models.Cita;
using MivetOnline.Models.Usuario;

namespace MivetOnline.Data.Interfaces
{
    public interface ICitaDAO
    {
        // Listar citas (datos técnicos - Backend)
        Task<List<CitaO>> ListarCitasBack();

        // Listar citas (con información completa - Frontend)
        Task<List<Cita>> ListarCitasFront();

        // Listar citas de un cliente específico
        Task<List<CitaCliente>> ListarCitasPorCliente(int idUsuario);

        // Agregar una nueva cita
        Task<bool> AgregarCita(DateTime calendario, int consultorio, long idVeterinario, long idMascota, long idPago);

        // Actualizar una cita existente
        Task<bool> ActualizarCita(long idCita, DateTime calendario, int consultorio, long idVeterinario, long idMascota, long idPago);

        // Eliminar una cita
        Task<bool> EliminarCita(long idCita);

        // Obtener citas por fecha específica
        Task<List<Cita>> ObtenerCitasPorFecha(int dia, int mes, int año);
    }
}