using MivetOnline.Models.Pago;

namespace MivetOnline.Data.Interfaces
{
    public interface IPagoDAO
    {
        // Listar métodos de pago disponibles (Yape, Efectivo, etc.)
        Task<List<PayOpts>> ListarPaymentOptions();

        // Agregar un nuevo pago y devolver su ID
        Task<long?> AgregarPago(DateTime hora, decimal monto, long tipoPago, long idUsuario);

        // Obtener pago por ID (datos técnicos - Backend)
        Task<PagoO?> ObtenerPagoPorId(long id);

        // Obtener pago por ID con información completa (Frontend)
        Task<Pago?> ObtenerPagoPorIdFront(long id);
    }
}
