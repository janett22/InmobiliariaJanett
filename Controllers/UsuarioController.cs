using InmobiliariaJanett.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InmobiliariaJanett.Controllers
{
  
        public class UsuarioController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment environment;
        private readonly IRepositorioUsuario repositorio;
        public UsuarioController(IConfiguration configuration, IWebHostEnvironment environment, IRepositorioUsuario repositorio)
        {
            this.configuration = configuration;
            this.environment = environment;
            this.repositorio = repositorio;
        }
        // GET: Usuario
        [Authorize(Policy = "Administrador")]
        public ActionResult Index()
        
        {
            try
            {
                IList<Usuario> lista = repositorio.ObtenerTodos();
                ViewBag.usuarios = lista;
                return View(lista);
            }
            catch (Exception ex)
            {

                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View();
            }
           
        }

        // GET: Usuario/Details/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Details(int id)
        {
            var e = repositorio.ObtenerPorId(id);
            return View(e);
        }
        // GET: Usuario/Create
        [Authorize(Policy = "Administrador")]
        public ActionResult Create()
        {
            try
            {
                ViewBag.Roles = Usuario.ObtenerRoles();
                return View();


            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrace = ex.StackTrace;
                return View();
                           }
           }

        // POST: Usuario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Create(Usuario u)
        {
            if (!ModelState.IsValid)
                return View();
            try
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: u.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));
                u.Clave = hashed;
                u.Rol = User.IsInRole("Administrador") ? u.Rol : (int)enRoles.Empleado;
                var nbreRnd = Guid.NewGuid();//posible nombre aleatorio
                int res = repositorio.Alta(u);
                if (u.AvatarFile != null && u.Id > 0)
                {
                    string wwwPath = environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "Uploads");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    //Path.GetFileName(u.AvatarFile.FileName);//este nombre se puede repetir
                    string fileName = "avatar_" + u.Id + Path.GetExtension(u.AvatarFile.FileName);
                    string pathCompleto = Path.Combine(path, fileName);
                    u.Avatar = Path.Combine("/Uploads/", fileName);
                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        u.AvatarFile.CopyTo(stream);
                    }
                    repositorio.usuarioConFoto(u);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrace = ex.StackTrace;
                ViewBag.Roles = Usuario.ObtenerRoles();
                return View();
            }
        }

        [Authorize]
        public ActionResult Perfil()
        {
            ViewData["Title"] = "Mi perfil";
            var u = repositorio.ObtenerPorEmail(User.Identity.Name);
            ViewBag.Roles = Usuario.ObtenerRoles();
            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View("Edit", u);
        }
        // GET: Usuario/Edit/5  
        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id)
        {
            ViewData["Title"] = "Editar usuario";
            var u = repositorio.ObtenerPorId(id);
            ViewBag.Roles = Usuario.ObtenerRoles();
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(u);
        }

        // POST: Usuario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(int id, Usuario u)
        {
            var vista = "Edit";
            try
            {
                u.Id = id;
                if (!User.IsInRole("Administrador"))
                {
                    vista = "Perfil";
                    var usuarioActual = repositorio.ObtenerPorEmail(User.Identity.Name);
                    if (usuarioActual.Id != id)//si no es admin, solo puede modificarse él mismo
                        return RedirectToAction(nameof(Index), "Home");
                    else
                    {
                        u.Rol = usuarioActual.Rol;
                        repositorio.Modificacion(u);
                        TempData["Mensaje"] = "Datos guardados correctamente";
                        if (TempData.ContainsKey("Mensaje"))
                            ViewBag.Mensaje = TempData["Mensaje"];

                        return RedirectToAction("Perfil", new { id = id });
                    }
                }
                else
                {
                    repositorio.Modificacion(u);
                    TempData["Mensaje"] = "Datos guardados correctamente";

                }

                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];

                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                ViewBag.Roles = Usuario.ObtenerRoles();
                return View(vista, u);
            }
        }

        // GET: Usuario/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            var u = repositorio.ObtenerPorId(id);
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(u);
        }

        // POST: Usuario/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id, Usuario entidad)
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

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginView login)
        {
            try
            {
                if (ModelState.IsValid)
                {
                   string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: login.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));

                    var e = repositorio.ObtenerPorEmail(login.Usuario);
                   if (e == null  || e.Clave != hashed)
                    {
                        ModelState.AddModelError("", "El email o la clave no son correctos");
                        return View();
                    }

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, e.Email),
                        new Claim("FullName", e.Nombre + " " + e.Apellido),
                        new Claim(ClaimTypes.Role, e.RolNombre),
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction(nameof(Index), "Home");
                }
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        [AllowAnonymous]
        // GET: Usuario/Login/
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        // GET: Usuario/Login/
        public ActionResult LoginModal()
        {
            return PartialView("_LoginModal", new LoginView());
        }

        // GET: Usuario/Logout
        [Route("salir", Name = "logout")]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

       
    }
}
