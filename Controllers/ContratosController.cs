using InmobiliariaJanett.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaJanett.Controllers
{
    public class ContratosController : Controller
    {
        private readonly RepositorioInmuebles repositorioInmuebles;
        private readonly RepositorioInquilino repositorioInquilino;
        private readonly RepositorioPropietario repositorioPropietario;
        private readonly RepositorioContrato repositorio;
        private readonly IConfiguration configuration;

        public ContratosController(IConfiguration configuration)
        {

            this.repositorio = new RepositorioContrato(configuration);
            this.repositorioInmuebles = new RepositorioInmuebles(configuration);
            this.repositorioInquilino = new RepositorioInquilino(configuration);
            this.repositorioPropietario = new RepositorioPropietario(configuration);

            this.configuration = configuration;
        }

        // GET: ContratosController
        public ActionResult Index()
        {

            var Con = repositorio.ObtenerTodos();
            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            return View(Con);

        }
        

        [Authorize]
        public ActionResult IndexPorInmueble(int id)
        {
            try
            {
                TempData["IdInmueble"] = id;

                ViewData["Title"] = "CONTRATOS DE ALQUILER";
                IList<Contrato> lista = repositorioInmuebles.BuscarPorContrato(id);
                if (TempData.ContainsKey("Id"))
                    ViewBag.Id = TempData["Id"];
                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];

                return View(lista);

            }
            catch (Exception ex)
            {

                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return Redirect(nameof(Index));
            }

        }


        // GET: ContratosController/Details/5
        public ActionResult Details(int id)
        {
            var c = repositorio.ObtenerPorId(id);
            return View(c);
        }

        // GET: ContratosController/Create
        public ActionResult Create()
        {
           ViewBag.inmuebles= repositorioInmuebles.ObtenerTodos();
            ViewBag.inquilinos = repositorioInquilino.ObtenerTodos();
            return View();
        }

        // POST: ContratosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Contrato contrato)
        {
            try
            {
                repositorio.Alta(contrato);
                TempData["Id"] = "Se creo el Contrato";
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                ViewBag.Inmuebles = repositorioInmuebles.ObtenerTodos();
                ViewBag.inquilinos = repositorioInquilino.ObtenerTodos();
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(contrato);
            }
        }

        // GET: ContratosController/Edit/5
        public ActionResult Edit(int id)
        {
            var entidad = repositorio.ObtenerPorId(id);
            ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos();
            ViewBag.Inmuebles = repositorioInmuebles.ObtenerTodos();
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(entidad);
        }

        // POST: ContratosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Contrato entidad)
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
                ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos();
                ViewBag.Inmuebles = repositorioInmuebles.ObtenerTodos();
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
        }

        // GET: ContratosController/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            var entidad = repositorio.ObtenerPorId(id);
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(entidad);
        }

        // POST: ContratosController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Contrato entidad)
        {
            try
            {
                repositorio.Baja(id);
                TempData["Mensaje"] = "Eliminación realizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
        }

        [Authorize]
        public ActionResult BuscarVigentes(BusquedaPorFechas busqueda)
        {
            try
            {
                IList<Contrato> entidad = repositorio.ContratosVigentes(busqueda.FechaInicio, busqueda.FechaFin);

                if (entidad != null)
                {
                    ViewData["Title"] = "CONTRATOS VIGENTES";
                    return View(nameof(Index), entidad);

                }
                else
                {
                    TempData["Mensaje"] = "El inmueble se encuentra ocupado en las fechas seleccionadas";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return RedirectToAction(nameof(Index));
            }

        }


    }
}
