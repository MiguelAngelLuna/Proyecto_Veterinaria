using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VeterinariaWebApp.Models;

namespace VeterinariaWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        public IActionResult Index()
        {

            var rol = HttpContext.Session.GetString("Rol");

            if (rol == "Cliente")
            {
                return RedirectToAction("Index", "Cliente");
            }


            return View();
        }


        public IActionResult Nosotros()
        {
            return View();
        }


        public IActionResult Servicios()
        {
            return View();
        }


        public IActionResult Contacto()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EnviarContacto(string nombre, string email, string mensaje)
        {
            try
            {

                TempData["SuccessMessage"] = "Mensaje enviado correctamente. Nos pondremos en contacto pronto.";
                return RedirectToAction("Contacto");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al enviar el mensaje: {ex.Message}";
                return View("Contacto");
            }
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}