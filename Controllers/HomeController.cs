using InmobiliariaJanett.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InmobiliariaJanett.Controllers
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
            return View();
        }

        [Authorize(Policy = "Administrador")]
        public ActionResult Admin()
        {
            return View();
        }

        public ActionResult Restringido()
        {
            return View();
        }

        public IActionResult Fecha(int anio, int mes, int dia)
        {
            DateTime dt = new DateTime(anio, mes, dia);
            ViewBag.Fecha = dt;
            return View();
        }

        public IActionResult Ruta(string valor)
        {
            ViewBag.Valor = valor;
            return View();
        }

       
    }
}
