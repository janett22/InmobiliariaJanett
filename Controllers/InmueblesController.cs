using InmobiliariaJanett.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaJanett.Controllers
{
    public class InmueblesController : Controller
    {


        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment environment;
        private readonly RepositorioPropietario repositorioPropietario;
        private readonly IRepositorioInmueble  repositorio;

        public InmueblesController(IConfiguration configuration, IWebHostEnvironment environment, IRepositorioInmueble repositorio)
        {

            this.repositorioPropietario = new RepositorioPropietario(configuration);
            this.environment = environment;
            this.repositorio = repositorio;
            this.configuration = configuration;
        }



        // GET: InmueblesController
        public ActionResult Index()
        {
            var Inmuebles = repositorio.ObtenerTodos();

            ViewData["Title"] = "INMUEBLES";

            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            return View(Inmuebles);
        }
        [Authorize]
        public ActionResult IndexPorPropietario(int id)
        {
            TempData["IdPro"] = id;

            TempData["pro"] = id;

          
            var lista = repositorio.BuscarPorPropietario(id);
           
            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];

            return View(lista);
        }
    
        // GET: InmueblesController/Details/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Details(int id)
        {
            var entidad = repositorio.ObtenerPorId(id);
            return View(entidad);
        }

        // GET: InmueblesController/Create
        [Authorize(Policy = "Administrador")]
        public ActionResult Create()
        {
            try
            {
                ViewBag.IdPro = TempData["IdPro"];
                TempData["Pro"] = TempData["IdPro"];
                ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
                ViewBag.Usos = Inmueble.ObtenerUsos();
                ViewBag.Tipos = Inmueble.ObtenerTipos();
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();

            }

        }

        // POST: InmueblesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Create(Inmueble entidad)
        {
            ViewBag.IdPro = TempData["Pro"];
            int id = ViewBag.IdPro;

            try
            {
                if (ModelState.IsValid)
                {
                    repositorio.Alta(entidad);
                    TempData["Id"] = entidad.Id;
                    TempData["Mensaje"] = "Datos guardados correctamente";

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Inmueble = repositorio.ObtenerTodos();
                    ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
                    ViewBag.Usos = Inmueble.ObtenerUsos();
                    ViewBag.Tipos = Inmueble.ObtenerTipos();

                    return View(entidad);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
        }

        // GET: InmueblesController/Edit/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id)
        {
            var entidad = repositorio.ObtenerPorId(id);
            ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            ViewBag.Usos = Inmueble.ObtenerUsos();
            ViewBag.Tipos = Inmueble.ObtenerTipos();
            return View(entidad);
        }
 
    // POST: InmueblesController/Edit/5
    [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id, Inmueble entidad)
        {
            try
            {
                entidad.Id = id;
                repositorio.Modificacion(entidad);
                TempData["Mensaje"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
             catch (Exception ex)
            {
                ViewBag.Propietarios = repositorioPropietario.ObtenerTodos();
                ViewBag.Usos = Inmueble.ObtenerUsos();
                ViewBag.Tipos = Inmueble.ObtenerTipos();
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
        }


        // GET: InmueblesController/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            var entidad = repositorio.ObtenerPorId(id);
            TempData["IdPro"] = entidad.IdPropietario;
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(entidad);
        }


        // POST: InmueblesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id, Inmueble entidad)
        {
            try
            {

                ViewBag.IdPro = TempData["IdPro"];
                entidad = repositorio.ObtenerPorId(id);
                int idPro = ViewBag.IdPro;

                repositorio.Baja(id);

                TempData["Mensaje"] = "Eliminación realizada correctamente";
                return RedirectToAction("PorPropietario", new { id = idPro });
            }
            catch (Exception ex)
            {
                ViewBag.Error = " No se puede eliminar el Inmueble, ya que posee Contratos asociados";


                return View(entidad);
            }
        }

        [Authorize]
        public ActionResult InmueblesDisponibles()
        {
            try
            {
                IList<Inmueble> lista = repositorio.BuscarDisponibles();
                ViewData["Title"] = "INMUEBLES DISPONIBLES";
                return View(nameof(Index), lista);

            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult BuscarInmueblesPorFecha(BusquedaPorFechas busqueda)
        {
            try
            {
                var lista = repositorio.BuscarInmueblesDisponibles(busqueda.FechaInicio, busqueda.FechaFin);
                ViewData["Title"] = "INMUEBLES DISPONIBLES";
                ViewData["Title2"] = "Periodo: " + busqueda.FechaInicio.ToShortDateString() + "-" + busqueda.FechaFin.ToShortDateString();

                if (TempData.ContainsKey("Id"))
                    ViewBag.Id = TempData["Id"];
                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];


                return View(nameof(Index), lista);

            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrace = ex.StackTrace;
                return View();
               
            }
            
        }





    }
}
