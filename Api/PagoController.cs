using System;
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
    public class PagoController : ControllerBase
    {
        private readonly DataContext contexto;

        public PagoController(DataContext contexto)
        {
            this.contexto = contexto;
        }


        
        
        //obtener todos los pagos
        // GET: api/<PagoController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var usuario = User.Identity.Name;
                var res = contexto.Pagos.Include(e => e.Contrato)
                                       .Where(e => e.Contrato.Inmueble.Duenio.Email == usuario)
                                       .Select(x => new { x.NroPago, x.Fecha, x.Importe });

                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        

        //obtener pagos de un contrato
        // GET api/<PagosController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pago>> Get(int id)
        {
            try
            {
                return Ok(contexto.Pagos.Where(x => x.IdContrato == id)
                    .Select(x => new { x.Id, x.IdContrato, x.NroPago, x.Fecha, x.Importe }));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    } }

