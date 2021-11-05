using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
    public class ContratoController : ControllerBase
    {
        private readonly DataContext contexto;

        public ContratoController(DataContext contexto)
        {
            this.contexto = contexto;
        }



        // GET: api/Contrato
        [HttpGet]
        public async Task<IActionResult> Get()
        {
       
            var usuario = User.Identity.Name;
            var res = await contexto.Contratos
                .Include(x => x.Inquilino)
                .Include(x => x.Inmueble)
                .ThenInclude(x => x.Duenio)
                .Where(c => c.Inmueble.Duenio.Email == usuario && (c.FechaFin >= DateTime.Now)).ToListAsync();
                 return Ok(res);
        }



        // GET: api/Contrato/5
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetDetalleInquilino(int Id)
        {
            return Ok(contexto.Contratos
                .Include(x => x.Inquilino)
                .Include(x => x.Inmueble)
                .ThenInclude(x => x.Duenio)
                .Where(x => x.Inmueble.Duenio.Email == User.Identity.Name && x.InquilinoId == Id).Single());
        }



    }        


}
