using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InmobiliariaJanett.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Inmobiliaria.Api
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class InmuebleController : ControllerBase
    {
        private readonly DataContext contexto;

        public InmuebleController(DataContext contexto)
        {
            this.contexto = contexto;
        }
      

        //obtener inmueble
        // GET: api/<InmuebleController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var usuario = User.Identity.Name;
                var res = await contexto.Inmuebles
                    .Include(x => x.Duenio)
                    .Where(x => x.Duenio.Email == usuario).ToListAsync();
                    
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Inmuebles/Alquilados                     // Ok alquilados vigentes
        [HttpGet("Alquilados")]
        public async Task<IActionResult> GetMisInmueblesAlquilados()
        {
            try
            {
                var usuario = User.Identity.Name;
                var hoy = DateTime.Now;
                var query = from inmueb in contexto.Inmuebles
                            join cont in contexto.Contratos
                                on inmueb.Id equals cont.InmuebleId
                            //join prop in contexto.Propietarios
                            //    on inmueb.Duenio.IdPropietario equals usuario
                            where cont.FechaInicio <= hoy && cont.FechaFin >= hoy && usuario == inmueb.Duenio.Email
                            select inmueb;



                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET api/<InmuebleController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var usuario = User.Identity.Name;
                return Ok(contexto.Inmuebles
                    .Include(e => e.Duenio)
                    .Where(e => e.Duenio.Email == usuario).Single(e => e.Id == id));
            
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        ///api/Inmueble/2
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id)
        {
            try
            {
                var usuario = User.Identity.Name;
                var inmueble = contexto.Inmuebles.Include(x => x.Duenio).FirstOrDefault(x => x.Id == id && x.Duenio.Email == usuario);

                if (inmueble != null) {
                    inmueble.Estado = !inmueble.Estado;
                    contexto.Inmuebles.Update(inmueble);
                    await contexto.SaveChangesAsync();
                    return Ok(inmueble);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        // POST api/<InmuebleController>
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] Inmueble entidad)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    entidad.IdPropietario = contexto.Propietarios.Single(e => e.Email == User.Identity.Name).Id;
                    contexto.Inmuebles.Add(entidad);
                    contexto.SaveChanges();
                    return CreatedAtAction(nameof(Get), new { id = entidad.Id }, entidad);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    

        // DELETE api/<InmuebleController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var entidad = contexto.Inmuebles.Include(e => e.Duenio).FirstOrDefault(e => e.Id == id && e.Duenio.Email == User.Identity.Name);
                if (entidad != null)
                {
                    contexto.Inmuebles.Remove(entidad);
                    contexto.SaveChanges();
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        

    }
}

