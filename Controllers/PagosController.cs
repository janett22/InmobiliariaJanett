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
    public class PagosController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment environment;
        private readonly IRepositorioPago repositorio;
        private readonly IRepositorioContrato repositorioCont;
        public PagosController(IConfiguration configuration, IWebHostEnvironment environment, IRepositorioPago repositorio, IRepositorioContrato repositorioCont)
        {
            this.configuration = configuration;
            this.environment = environment;
            this.repositorio = repositorio;
            this.repositorioCont = repositorioCont;
        }

        // GET: PagosController
        public ActionResult Index()
        {
            try
            {

                var lista = repositorio.ObtenerTodos();
                if (TempData.ContainsKey("Id"))
                    ViewBag.Id = TempData["Id"];
                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];
                return View(lista);
            }
            catch (Exception ex)
            {
                throw;

            }

        }

        // GET: PagosController/Details/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Details(int id)
        {
            var p = repositorio.ObtenerPorId(id);
            return View(p);
        }

        // GET: PagosController/Create

        public ActionResult Create()
        {
            try
            {
                IList<Contrato> lista = repositorioCont.ObtenerTodos();
                ViewBag.Contrato = lista;
                return View();

       
            }
            catch (Exception ex)
            {
                throw;
            }
            
            
        }

        // POST: PagosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Create(Pago entidad)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    repositorio.Alta(entidad);
                    TempData["Id"] = entidad.Id;
                    ViewBag.Contrato = entidad;

                    IList<Contrato> lista = repositorioCont.ObtenerTodos();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Contrato = repositorioCont.ObtenerTodos();
                    return View(entidad);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrace = ex.StackTrace;
                return View(entidad);
            }
        }
        
        // GET: PagosController/Edit/5
            public ActionResult Edit(int id)
        {

            var entidad = repositorio.ObtenerPorId(id);
            ViewBag.Contratos  = repositorioCont.ObtenerTodos();
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(entidad);

        }

        // POST: PagosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id, Pago entidad)
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
                ViewBag.Contrato = repositorioCont.ObtenerTodos();
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }

        }

        // GET: PagosController/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            try
            {
                var entidad = repositorio.ObtenerPorId(id);
                TempData["IdCont"] = entidad.IdContrato;
                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];
                if (TempData.ContainsKey("Error"))
                    ViewBag.Error = TempData["Error"];
                return View(entidad);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        // POST: PagosController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id, Pago entidad)
        {

            try
            {
                ViewBag.IdCont = TempData["IdCont"];
                int idCont = ViewBag.IdCont;
                entidad = repositorio.ObtenerPorId(id);
                repositorio.Baja(id);

                TempData["Mensaje"] = "Eliminación realizada correctamente";
                return RedirectToAction("PorContrato", new { id = idCont });
            }
            catch (Exception ex)
            {
                ViewBag.Error = " No se puede eliminar el Pago, ya que posee un Contrato asociado";
                return View(entidad);
            }

        }


        public ActionResult PorContrato(int id)
        {
            try
            {

                TempData["ContId"] = id;
                TempData["IdPago"] = id;
                var lista = repositorio.BuscarPorContrato(id);
                ViewBag.PorContrato = lista;
                IList<Contrato> contratos = repositorioCont.ObtenerTodos();
                ViewBag.Pagos = lista;
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
                return View();
   
            }
        }


        public ActionResult pagar(int id)
        {
            TempData["ContId"] = id;
            TempData["IdPago"] = id;
            var lista = repositorio.BuscarPorContrato(id);
            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];

            return View(lista);
        }


    }

}
