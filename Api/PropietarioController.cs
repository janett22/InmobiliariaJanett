using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using InmobiliariaJanett.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Inmobiliaria.Api
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class PropietarioController : ControllerBase
    {
        private readonly DataContext contexto;
        private readonly IConfiguration config;
        public PropietarioController(DataContext contexto, IConfiguration config)
        {
            this.contexto = contexto;
            this.config = config;
        } 

        //obtener un propietario
        [HttpGet]
        public async Task<ActionResult<Propietario>> Get()
        {
            try
            {
                var usuario = User.Identity.Name;

                return await contexto.Propietarios.SingleOrDefaultAsync(x => x.Email == usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        //obtenr por id
        // GET api/<PropietarioController>/5
            [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (id <= 0)
                return NotFound();
            var res =  contexto.Propietarios.FirstOrDefault(x => x.Id == id);
            if (res != null)
                return Ok(res);
            else
                return NotFound();


        }


        //crear un propietario
        // POST api/Propietario
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Propietario propietario)
        {
            try
            {

                if (ModelState.IsValid)
                {

                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                      password: propietario.clave,
                      salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                      prf: KeyDerivationPrf.HMACSHA1,
                      iterationCount: 1000,
                      numBytesRequested: 256 / 8));

                    propietario.clave = hashed;



                    contexto.Propietarios.Add(propietario);
                    contexto.SaveChanges();
                    return CreatedAtAction(nameof(Get), new { id = propietario.Id }, propietario);
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }



        }


        //actualizar un propietario
        // PUT api/<PropietarioController>/5
        [HttpPut()]
        public async Task<IActionResult> Put([FromBody] Propietario propietario)
        {
            if (ModelState.IsValid)
            {

                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                       password: propietario.clave,
                       salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                       prf: KeyDerivationPrf.HMACSHA1,
                       iterationCount: 1000,
                       numBytesRequested: 256 / 8));

                propietario.clave = hashed;

                contexto.Propietarios.Update(propietario);
                 await contexto.SaveChangesAsync();
                return Ok(propietario);
            }

            return BadRequest();
        }
     

     
        //login para ingresar
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm]LoginView loginView)
        {
            try
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: loginView.Clave,
                    salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));
                var p = contexto.Propietarios.FirstOrDefault(x => x.Email == loginView.Usuario);
                if (p == null || p.clave != hashed)
                {
                    return BadRequest("Nombre de usuario o clave incorrecta");
                }
                else
                {
                    var key = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(config["TokenAuthentication:SecretKey"]));
                    var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, p.Email),
                        new Claim("FullName", p.Nombre + " " + p.Apellido),
                        new Claim(ClaimTypes.Role, "Propietario"),
                    };

                    var token = new JwtSecurityToken(
                        issuer: config["TokenAuthentication:Issuer"],
                        audience: config["TokenAuthentication:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(600),
                        signingCredentials: credenciales
                    );
                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
