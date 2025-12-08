using Microsoft.AspNetCore.Mvc;
using VeterinariaWebApp.Data.Interfaces;
using VeterinariaWebApp.Models.Usuario;

namespace VeterinariaWebApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUsuarioDAO _usuarioDAO;
        private readonly IClienteDAO _clienteDAO;

        public LoginController(IUsuarioDAO usuarioDAO, IClienteDAO clienteDAO)
        {
            _usuarioDAO = usuarioDAO;
            _clienteDAO = clienteDAO;
        }

        // GET: Login
        public IActionResult Index()
        {
            // Si ya está logueado, redirigir al dashboard
            if (HttpContext.Session.GetString("Rol") != null)
            {
                return RedirectToAction("Index", "Cliente");
            }
            return View();
        }

        // POST: Login/Ingresar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ingresar(string correo, string contraseña)
        {
            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contraseña))
            {
                ViewBag.Error = "Por favor ingrese correo y contraseña";
                return View("Index");
            }

            try
            {
                // Verificar credenciales
                var rol = await _usuarioDAO.VerificarLogin(correo, contraseña);

                if (rol != null && rol == "Cliente")
                {
                    // Obtener ID del usuario
                    var idUsuario = await _usuarioDAO.ObtenerIdUsuario(correo);

                    if (idUsuario.HasValue)
                    {
                        // Guardar datos en sesión
                        HttpContext.Session.SetInt32("IdUsuario", (int)idUsuario.Value);
                        HttpContext.Session.SetString("Rol", rol);
                        HttpContext.Session.SetString("Correo", correo);

                        // Redirigir al dashboard del cliente
                        return RedirectToAction("Index", "Cliente");
                    }
                }

                ViewBag.Error = "Credenciales incorrectas o usuario no autorizado";
                return View("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al iniciar sesión: {ex.Message}";
                return View("Index");
            }
        }

        // GET: Login/Registro
        public async Task<IActionResult> Registro()
        {
            // Cargar tipos de documento para el combo
            var documentos = await _usuarioDAO.ListarDocumentos();
            ViewBag.Documentos = documentos;
            return View();
        }

        // POST: Login/Registrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(UsuarioO usuario)
        {
            if (!ModelState.IsValid)
            {
                var documentos = await _usuarioDAO.ListarDocumentos();
                ViewBag.Documentos = documentos;
                return View("Registro", usuario);
            }

            try
            {
                // El rol de Cliente es 1
                usuario.ide_rol = 1;

                // Registrar el cliente
                var resultado = await _clienteDAO.AgregarCliente(usuario);

                if (resultado)
                {
                    TempData["SuccessMessage"] = "¡Registro exitoso! Inicia sesión con tus credenciales.";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "No se pudo registrar el cliente. Intente nuevamente.";
                    var documentos = await _usuarioDAO.ListarDocumentos();
                    ViewBag.Documentos = documentos;
                    return View("Registro", usuario);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al registrar: {ex.Message}";
                var documentos = await _usuarioDAO.ListarDocumentos();
                ViewBag.Documentos = documentos;
                return View("Registro", usuario);
            }
        }

        // GET: Login/CerrarSesion
        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        // Método auxiliar para verificar si el usuario está autenticado
        private bool EstaAutenticado()
        {
            return HttpContext.Session.GetInt32("IdUsuario") != null;
        }
    }
}