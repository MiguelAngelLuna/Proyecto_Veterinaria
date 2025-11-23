using Microsoft.AspNetCore.Mvc;
using MivetOnline.Data.Interfaces;
using MivetOnline.Models.Mascota;
using MivetOnline.Models.Usuario;

namespace MivetOnline.Controllers
{
    public class MascotaController : Controller
    {
        private readonly IMascotaDAO _mascotaDAO;

        public MascotaController(IMascotaDAO mascotaDAO)
        {
            _mascotaDAO = mascotaDAO;
        }

        // GET: Mascota (Listar mascotas del cliente)
        public async Task<IActionResult> Index()
        {
            if (!EstaAutenticado())
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                var idUsuario = HttpContext.Session.GetInt32("IdUsuario");
                var mascotas = await _mascotaDAO.ListarMascotasPorCliente(idUsuario.Value);
                return View(mascotas);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al cargar las mascotas: {ex.Message}";
                return View(new List<Mascota>());
            }
        }

        // GET: Mascota/Crear
        public IActionResult Crear()
        {
            if (!EstaAutenticado())
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        // POST: Mascota/Agregar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Agregar(MascotaO mascota)
        {
            if (!EstaAutenticado())
            {
                return RedirectToAction("Index", "Login");
            }

            if (!ModelState.IsValid)
            {
                return View("Crear", mascota);
            }

            try
            {
                var idUsuario = HttpContext.Session.GetInt32("IdUsuario");

                var resultado = await _mascotaDAO.AgregarMascota(
                    mascota.nom_mas,
                    mascota.esp_mas,
                    mascota.raz_mas,
                    mascota.fna_mas,
                    idUsuario.Value
                );

                if (resultado)
                {
                    TempData["SuccessMessage"] = "¡Mascota registrada exitosamente!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "No se pudo registrar la mascota. Verifique que sea un cliente válido.";
                    return View("Crear", mascota);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al agregar mascota: {ex.Message}";
                return View("Crear", mascota);
            }
        }

        // GET: Mascota/Editar/5
        public async Task<IActionResult> Editar(long id)
        {
            if (!EstaAutenticado())
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                var idUsuario = HttpContext.Session.GetInt32("IdUsuario");
                var mascotas = await _mascotaDAO.ListarMascotasPorCliente(idUsuario.Value);
                var mascota = mascotas.FirstOrDefault(m => m.IdMascota == id);

                if (mascota == null)
                {
                    TempData["Error"] = "Mascota no encontrada";
                    return RedirectToAction("Index");
                }

                // Convertir Mascota a MascotaO para el formulario
                var mascotaO = new MascotaO
                {
                    ide_mas = mascota.IdMascota,
                    nom_mas = mascota.Nombre ?? string.Empty,
                    esp_mas = mascota.Especie ?? string.Empty,
                    raz_mas = mascota.Raza ?? string.Empty,
                    fna_mas = mascota.FechaNacimiento
                };

                return View(mascotaO);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al cargar mascota: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // POST: Mascota/Actualizar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Actualizar(MascotaO mascota)
        {
            if (!EstaAutenticado())
            {
                return RedirectToAction("Index", "Login");
            }

            if (!ModelState.IsValid)
            {
                return View("Editar", mascota);
            }

            try
            {
                var resultado = await _mascotaDAO.ActualizarMascota(
                    mascota.ide_mas,
                    mascota.nom_mas,
                    mascota.esp_mas,
                    mascota.raz_mas,
                    mascota.fna_mas
                );

                if (resultado)
                {
                    TempData["SuccessMessage"] = "Mascota actualizada correctamente";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "No se pudo actualizar la mascota";
                    return View("Editar", mascota);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al actualizar: {ex.Message}";
                return View("Editar", mascota);
            }
        }

        // GET: Mascota/Eliminar/5
        public async Task<IActionResult> Eliminar(long id)
        {
            if (!EstaAutenticado())
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                var idUsuario = HttpContext.Session.GetInt32("IdUsuario");
                var mascotas = await _mascotaDAO.ListarMascotasPorCliente(idUsuario.Value);
                var mascota = mascotas.FirstOrDefault(m => m.IdMascota == id);

                if (mascota == null)
                {
                    TempData["Error"] = "Mascota no encontrada";
                    return RedirectToAction("Index");
                }

                return View(mascota);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al cargar mascota: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // POST: Mascota/ConfirmarEliminar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarEliminar(long id)
        {
            if (!EstaAutenticado())
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                var mensaje = await _mascotaDAO.EliminarMascota(id);

                if (mensaje.Contains("correctamente"))
                {
                    TempData["SuccessMessage"] = mensaje;
                }
                else
                {
                    TempData["Error"] = mensaje;
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar mascota: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // GET: Mascota/Detalle/5
        public async Task<IActionResult> Detalle(long id)
        {
            if (!EstaAutenticado())
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                var idUsuario = HttpContext.Session.GetInt32("IdUsuario");
                var mascotas = await _mascotaDAO.ListarMascotasPorCliente(idUsuario.Value);
                var mascota = mascotas.FirstOrDefault(m => m.IdMascota == id);

                if (mascota == null)
                {
                    TempData["Error"] = "Mascota no encontrada";
                    return RedirectToAction("Index");
                }

                return View(mascota);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al cargar mascota: {ex.Message}";
                return RedirectToAction("Index");
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