using Microsoft.AspNetCore.Mvc;
using VeterinariaWebApp.Data.Interfaces;
using VeterinariaWebApp.Models.Usuario;
using VeterinariaWebApp.Models.Usuario.Cliente;

namespace VeterinariaWebApp.Controllers
{
    public class ClienteController : Controller
    {
        private readonly IClienteDAO _clienteDAO;
        private readonly IUsuarioDAO _usuarioDAO;
        private readonly IMascotaDAO _mascotaDAO;

        public ClienteController(IClienteDAO clienteDAO, IUsuarioDAO usuarioDAO, IMascotaDAO mascotaDAO)
        {
            _clienteDAO = clienteDAO;
            _usuarioDAO = usuarioDAO;
            _mascotaDAO = mascotaDAO;
        }

        // GET: Cliente (Dashboard)
        public async Task<IActionResult> Index()
        {
            if (!EstaAutenticado())
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                var idUsuario = HttpContext.Session.GetInt32("IdUsuario");
                var correo = HttpContext.Session.GetString("Correo");

                ViewBag.Correo = correo;
                ViewBag.IdUsuario = idUsuario;

                // Cargar mascotas del cliente
                var mascotas = await _mascotaDAO.ListarMascotasPorCliente(idUsuario.Value);
                ViewBag.TotalMascotas = mascotas.Count;

                // Cargar citas del cliente
                var citas = await _clienteDAO.ListarCitasPorCliente(idUsuario.Value);
                ViewBag.TotalCitas = citas.Count;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al cargar el dashboard: {ex.Message}";
                return View();
            }
        }

        // GET: Cliente/MisCitas
        public async Task<IActionResult> MisCitas()
        {
            if (!EstaAutenticado())
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                var idUsuario = HttpContext.Session.GetInt32("IdUsuario");
                var citas = await _clienteDAO.ListarCitasPorCliente(idUsuario.Value);
                return View(citas);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al cargar las citas: {ex.Message}";
                return View(new List<CitaCliente>());
            }
        }

        // GET: Cliente/MiPerfil
        public async Task<IActionResult> MiPerfil()
        {
            if (!EstaAutenticado())
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                var correo = HttpContext.Session.GetString("Correo");
                ViewBag.Correo = correo;

                // Aquí puedes cargar más datos del perfil si es necesario
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al cargar el perfil: {ex.Message}";
                return View();
            }
        }

        // GET: Cliente/Editar
        public async Task<IActionResult> Editar()
        {
            if (!EstaAutenticado())
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                var idUsuario = HttpContext.Session.GetInt32("IdUsuario");

                // Obtener datos del cliente
                var clientes = await _clienteDAO.ListarClientesBack();
                var cliente = clientes.FirstOrDefault(c => c.ide_usr == idUsuario);

                if (cliente == null)
                {
                    TempData["Error"] = "No se encontraron datos del cliente";
                    return RedirectToAction("Index");
                }

                // Cargar tipos de documento
                var documentos = await _usuarioDAO.ListarDocumentos();
                ViewBag.Documentos = documentos;

                return View(cliente);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al cargar datos: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // POST: Cliente/Actualizar
   
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Actualizar(ClienteO cliente)
        {
            if (!EstaAutenticado())
            {
                return RedirectToAction("Index", "Login");
            }

            if (!ModelState.IsValid)
            {
                var documentos = await _usuarioDAO.ListarDocumentos();
                ViewBag.Documentos = documentos;
                return View("Editar", cliente);
            }

            try
            {
                var usuario = new UsuarioO
                {
                    cor_usr = cliente.cor_usr,
                    pwd_usr = cliente.pwd_usr,
                    nom_usr = cliente.nom_usr,
                    ape_usr = cliente.ape_usr,
                    num_doc = cliente.num_doc,
                    fna_usr = cliente.fna_usr,
                    ide_doc = cliente.ide_doc,
                    ide_rol = cliente.ide_rol
                };

                var resultado = await _clienteDAO.ActualizarCliente(cliente.ide_cli, usuario);

                if (resultado)
                {
                
                    HttpContext.Session.SetString("Correo", cliente.cor_usr);

                    TempData["SuccessMessage"] = "Perfil actualizado correctamente";
                    return RedirectToAction("MiPerfil");
                }
                else
                {
                    ViewBag.Error = "No se pudo actualizar el perfil";
                    var documentos = await _usuarioDAO.ListarDocumentos();
                    ViewBag.Documentos = documentos;
                    return View("Editar", cliente);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al actualizar: {ex.Message}";
                var documentos = await _usuarioDAO.ListarDocumentos();
                ViewBag.Documentos = documentos;
                return View("Editar", cliente);
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