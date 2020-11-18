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
    //[ApiController]
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
        /*[HttpGet]
        public async Task<ActionResult<IEnumerable<Propietario>>> GetPropietarios()
        {
            return await contexto.Propietario.ToListAsync();
        }

        // GET: api/Propietarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Propietario>> GetPropietario(int id)
        {
            var propietario = await contexto.Propietario.FindAsync(id);

            if (propietario == null)
            {
                return NotFound();
            }

            return propietario;
        }*/

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

        // PUT: api/Propietarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPropietario(int id, Propietario propietario)
        {
            if (id != propietario.IdPropietario)
            {
                return BadRequest();
            }

            contexto.Entry(propietario).State = EntityState.Modified;

            try
            {
                await contexto.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PropietarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Propietarios
        [HttpPost]
        public async Task<ActionResult<Propietario>> PostPropietario(Propietario propietario)
        {
            contexto.Propietario.Add(propietario);
            await contexto.SaveChangesAsync();

            return CreatedAtAction("GetPropietario", new { id = propietario.IdPropietario }, propietario);
        }

        // DELETE: api/Propietarios/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Propietario>> DeletePropietario(int id)
        {
            var propietario = await contexto.Propietario.FindAsync(id);
            if (propietario == null)
            {
                return NotFound();
            }

            contexto.Propietario.Remove(propietario);
            await contexto.SaveChangesAsync();

            return propietario;
        }

        private bool PropietarioExists(int id)
        {
            return contexto.Propietario.Any(e => e.IdPropietario == id);
        }
    }
}
