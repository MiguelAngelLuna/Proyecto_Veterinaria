using Microsoft.AspNetCore.Mvc;
using MivetOnline.Data.Interfaces;
using MivetOnline.Models.Cita;
using MivetOnline.Models.Usuario;

namespace MivetOnline.Controllers
{
    public class CitaController : Controller
    {
        private readonly ICitaDAO _citaDAO;
        private readonly IClienteDAO _clienteDAO;
        private readonly IMascotaDAO _mascotaDAO;
        private readonly IPagoDAO _pagoDAO;

        public CitaController(ICitaDAO citaDAO, IClienteDAO clienteDAO, IMascotaDAO mascotaDAO, IPagoDAO pagoDAO)
        {
            _citaDAO = citaDAO;
            _clienteDAO = clienteDAO;
            _mascotaDAO = mascotaDAO;
            _pagoDAO = pagoDAO;
        }

        // GET: Cita (Listar citas del cliente)
        public async Task<IActionResult> Index()
        {
            if (!EstaAutenticado())
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                // Redirigir a la vista de citas del cliente en ClienteController
                return RedirectToAction("MisCitas", "Cliente");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al cargar las citas: {ex.Message}";
                return View();
            }
        }

        // GET: Cita/Agendar
        public async Task<IActionResult> Agendar()
        {
            if (!EstaAutenticado())
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                var idUsuario = HttpContext.Session.GetInt32("IdUsuario");

                // Cargar mascotas del cliente para el dropdown
                var mascotas = await _mascotaDAO.ListarMascotasPorCliente(idUsuario.Value);
                ViewBag.Mascotas = mascotas;

                // Cargar métodos de pago
                var metodosAgo = await _pagoDAO.ListarPaymentOptions();
                ViewBag.MetodosPago = metodosAgo;

                // Nota: Los veterinarios se cargarían aquí si estuvieran habilitados
                // Por ahora, el cliente solo puede seleccionar su mascota y método de pago

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al cargar el formulario: {ex.Message}";
                return View();
            }
        }

        // POST: Cita/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(CitaO cita, long metodoPago, decimal montoPago)
        {
            if (!EstaAutenticado())
            {
                return RedirectToAction("Index", "Login");
            }

            if (!ModelState.IsValid)
            {
                var idUsuario = HttpContext.Session.GetInt32("IdUsuario");
                var mascotas = await _mascotaDAO.ListarMascotasPorCliente(idUsuario.Value);
                ViewBag.Mascotas = mascotas;
                var metodosPago = await _pagoDAO.ListarPaymentOptions();
                ViewBag.MetodosPago = metodosPago;
                return View("Agendar", cita);
            }

            try
            {
                var idUsuario = HttpContext.Session.GetInt32("IdUsuario");

                // 1. Primero crear el pago
                var idPago = await _pagoDAO.AgregarPago(
                    DateTime.Now,
                    montoPago,
                    metodoPago,
                    idUsuario.Value
                );

                if (!idPago.HasValue)
                {
                    ViewBag.Error = "Error al procesar el pago";
                    var mascotas = await _mascotaDAO.ListarMascotasPorCliente(idUsuario.Value);
                    ViewBag.Mascotas = mascotas;
                    var metodosPago = await _pagoDAO.ListarPaymentOptions();
                    ViewBag.MetodosPago = metodosPago;
                    return View("Agendar", cita);
                }

                // 2. Crear la cita con el ID del pago
                var resultado = await _citaDAO.AgregarCita(
                    cita.CalendarioCita,
                    (int)cita.Consultorio,
                    cita.IdVeterinario,
                    cita.IdMascota,
                    idPago.Value
                );

                if (resultado)
                {
                    TempData["SuccessMessage"] = "¡Cita agendada exitosamente!";
                    return RedirectToAction("MisCitas", "Cliente");
                }
                else
                {
                    ViewBag.Error = "No se pudo agendar la cita";
                    var mascotas = await _mascotaDAO.ListarMascotasPorCliente(idUsuario.Value);
                    ViewBag.Mascotas = mascotas;
                    var metodosPago = await _pagoDAO.ListarPaymentOptions();
                    ViewBag.MetodosPago = metodosPago;
                    return View("Agendar", cita);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al crear la cita: {ex.Message}";
                var idUsuario = HttpContext.Session.GetInt32("IdUsuario");
                var mascotas = await _mascotaDAO.ListarMascotasPorCliente(idUsuario.Value);
                ViewBag.Mascotas = mascotas;
                var metodosPago = await _pagoDAO.ListarPaymentOptions();
                ViewBag.MetodosPago = metodosPago;
                return View("Agendar", cita);
            }
        }

        // GET: Cita/Detalle/5
        public async Task<IActionResult> Detalle(long id)
        {
            if (!EstaAutenticado())
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                var idUsuario = HttpContext.Session.GetInt32("IdUsuario");
                var citas = await _clienteDAO.ListarCitasPorCliente(idUsuario.Value);
                var cita = citas.FirstOrDefault(c => c.ide_cit == id);

                if (cita == null)
                {
                    TempData["Error"] = "Cita no encontrada";
                    return RedirectToAction("MisCitas", "Cliente");
                }

                return View(cita);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al cargar la cita: {ex.Message}";
                return RedirectToAction("MisCitas", "Cliente");
            }
        }

        // GET: Cita/Cancelar/5
        public async Task<IActionResult> Cancelar(long id)
        {
            if (!EstaAutenticado())
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                var idUsuario = HttpContext.Session.GetInt32("IdUsuario");
                var citas = await _clienteDAO.ListarCitasPorCliente(idUsuario.Value);
                var cita = citas.FirstOrDefault(c => c.ide_cit == id);

                if (cita == null)
                {
                    TempData["Error"] = "Cita no encontrada";
                    return RedirectToAction("MisCitas", "Cliente");
                }

                return View(cita);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al cargar la cita: {ex.Message}";
                return RedirectToAction("MisCitas", "Cliente");
            }
        }

        // POST: Cita/ConfirmarCancelacion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarCancelacion(long id)
        {
            if (!EstaAutenticado())
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                var resultado = await _citaDAO.EliminarCita(id);

                if (resultado)
                {
                    TempData["SuccessMessage"] = "Cita cancelada correctamente";
                }
                else
                {
                    TempData["Error"] = "No se pudo cancelar la cita";
                }

                return RedirectToAction("MisCitas", "Cliente");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cancelar la cita: {ex.Message}";
                return RedirectToAction("MisCitas", "Cliente");
            }
        }

        // GET: Cita/BuscarPorFecha
        public async Task<IActionResult> BuscarPorFecha(DateTime? fecha)
        {
            if (!EstaAutenticado())
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                if (!fecha.HasValue)
                {
                    ViewBag.Message = "Seleccione una fecha para buscar";
                    return View(new List<Cita>());
                }

                var citas = await _citaDAO.ObtenerCitasPorFecha(
                    fecha.Value.Day,
                    fecha.Value.Month,
                    fecha.Value.Year
                );

                ViewBag.FechaBusqueda = fecha.Value.ToString("dd/MM/yyyy");
                return View(citas);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al buscar citas: {ex.Message}";
                return View(new List<Cita>());
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