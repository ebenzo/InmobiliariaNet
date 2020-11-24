using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Inmobiliaria.Api
{
    [Route("api/[controller]")]
    //[ApiController] si descomento esto el postman me da error, deberia buscar como se hace la llamada desde pm cuando lo activo
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PropietariosController : Controller
    {
        private readonly DataContext contexto;
        private readonly IConfiguration config;

        public PropietariosController(DataContext context, IConfiguration config)
        {
            contexto = context;
            this.config = config;
        }

        // GET: api/Propietarios
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var usuario = User.Identity.Name;
                return Ok(contexto.Propietario.SingleOrDefault(x => x.Email == usuario));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                return Ok(contexto.Propietario.SingleOrDefault(x => x.IdPropietario == id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET api/<controller>/5
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginView loginView)
        {
            try
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: loginView.Clave,
                    salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));
                var p = contexto.Propietario.FirstOrDefault(x => x.Email == loginView.Email);
                if (p == null || p.Clave != hashed)
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
                        expires: DateTime.Now.AddMinutes(60),
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

        // PUT: api/Propietarios/5 --> el Put es el Update de una entidad --> id es el id del propietario a modificar
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromForm] Propietario propietario)
        {
            try
            {
                // comentar si es costoso
                if (!PropietarioExists(id)) 
                {
                    return NotFound();
                }

                if (ModelState.IsValid && contexto.Propietario.AsNoTracking().FirstOrDefault(e => e.IdPropietario == id && e.Email == User.Identity.Name) != null)
                {
                    propietario.IdPropietario = id;
                    contexto.Propietario.Update(propietario);
                    contexto.SaveChanges();
                    return Ok(propietario);
                }
                return BadRequest(); //return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST: api/Propietarios --> el Post es el alta de una nueva entidad
        //authorization de admin??
        [HttpPost]
        public async Task<IActionResult> Post([FromForm]Propietario propietario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    contexto.Propietario.Add(propietario);
                    await contexto.SaveChangesAsync();

                    return CreatedAtAction("Get", new { id = propietario.IdPropietario }, propietario);
                }
                return BadRequest();//return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // DELETE: api/Propietarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // comentar si es costoso o innecesario
            if (!PropietarioExists(id))
            {
                return NotFound();
            }

            var entidad = contexto.Propietario.FirstOrDefault(e => e.IdPropietario == id && e.Email == User.Identity.Name);
            if (entidad != null)
            {
                contexto.Propietario.Remove(entidad);
                contexto.SaveChanges();
                return Ok();
            }

            return BadRequest();//return NoContent();
        }

        private bool PropietarioExists(int id)
        {
            return contexto.Propietario.Any(e => e.IdPropietario == id);
        }
    }
}
