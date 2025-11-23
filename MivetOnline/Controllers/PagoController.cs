using Microsoft.AspNetCore.Mvc;
using MivetOnline.Data.Interfaces;


namespace MivetOnline.Controllers
{
    public class PagoController : Controller
    {
        private readonly IPagoDAO _pagoDAO;
        private readonly IClienteDAO _clienteDAO;

        public PagoController(IPagoDAO pagoDAO, IClienteDAO clienteDAO)
        {
            _pagoDAO = pagoDAO;
            _clienteDAO = clienteDAO;
        }

        // GET: Pago/PagosCliente
        public async Task<IActionResult> PagosCliente()
        {
            if (!EstaAutenticado())
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                var idUsuario = HttpContext.Session.GetInt32("IdUsuario");

                // Obtener las citas del cliente (que contienen información de pagos)
                var citas = await _clienteDAO.ListarCitasPorCliente(idUsuario.Value);

                return View(citas);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al cargar los pagos: {ex.Message}";
                return View(new List<MivetOnline.Models.Usuario.CitaCliente>());
            }
        }

        // GET: Pago/Detalle/5
        public async Task<IActionResult> Detalle(long id)
        {
            if (!EstaAutenticado())
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                var pago = await _pagoDAO.ObtenerPagoPorIdFront(id);

                if (pago == null)
                {
                    TempData["Error"] = "Pago no encontrado";
                    return RedirectToAction("PagosCliente");
                }

                return View(pago);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al cargar el pago: {ex.Message}";
                return RedirectToAction("PagosCliente");
            }
        }

        // Método auxiliar para verificar autenticación
        private bool EstaAutenticado()
        {
            var idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            var rol = HttpContext.Session.GetString("Rol");
            return idUsuario != null && rol == "Cliente";
        }
    }
}