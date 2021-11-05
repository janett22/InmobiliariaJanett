using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using InmobiliariaJanett.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Inmobiliaria.Api
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class InquilinoController : ControllerBase
    {
        private readonly DataContext contexto;
     

        public InquilinoController(DataContext contexto)
        {
            this.contexto = contexto;
            
        }
        //GET api/Inquilino/id
        [HttpGet("{id}")]
        public async Task<IActionResult> obtenerInquilino(int id)
        {
            return Ok(contexto.Contratos
              
                .Include(x => x.Inquilino)
                 .Include(x => x.Inmueble)
                .ThenInclude(x => x.Duenio)
                .Where(x => x.Inmueble.Duenio.Email == User.Identity.Name && x.Id == id).Single());
        }


        //obtener inquilino
        // GET: api/Inquilino
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var usuario = User.Identity.Name;
                var res = await contexto.Inquilinos
                    .Where(x => x.Email == usuario).ToListAsync();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST api/<InquilinoController>
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] Inquilino inquilino)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    contexto.Inquilinos.Add(inquilino);
                    contexto.SaveChanges();
                    return CreatedAtAction(nameof(Get), new { id = inquilino.Id }, inquilino);
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT api/<InquilinoController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromForm] Inquilino inquilino)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    contexto.Inquilinos.Update(inquilino);
                    contexto.SaveChanges();
                    return Ok(inquilino);
                }

                return BadRequest();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InquilinoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        private bool InquilinoExists(int id)
        {
            return contexto.Inquilinos.Any(e => e.Id == id);
        }

        // DELETE api/<InquilinoController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var p = contexto.Inquilinos.Find(id);
                    if (p == null)
                        return NotFound();
                    contexto.Inquilinos.Remove(p);
                    contexto.SaveChanges();
                    return Ok(p);
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
