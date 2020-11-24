using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Inmobiliaria.Api
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class InmueblesController : Controller
    {
        private readonly DataContext contexto;

        public InmueblesController(DataContext contexto)
        {
            this.contexto = contexto;
        }

        // GET: api/<controller>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var usuario = User.Identity.Name;
                return Ok(contexto.Inmueble.Include(e => e.Propietario).Where(e => e.Propietario.Email == usuario));
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
                var usuario = User.Identity.Name;
                return Ok(contexto.Inmueble.Include(e => e.Propietario).Where(e => e.Propietario.Email == usuario).Single(e => e.IdInmueble == id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST api/<controller>
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] Inmueble inmueble)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    inmueble.IdPropietario = contexto.Propietario.Single(e => e.Email == User.Identity.Name).IdPropietario;
                    contexto.Inmueble.Add(inmueble);
                    contexto.SaveChanges();
                    return CreatedAtAction(nameof(Get), new { id = inmueble.IdInmueble }, inmueble);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromForm] Inmueble inmueble)
        {
            try
            {
                if (ModelState.IsValid && contexto.Inmueble.AsNoTracking().Include(e => e.Propietario).FirstOrDefault(e => e.IdInmueble == id && e.Propietario.Email == User.Identity.Name) != null)
                {
                    inmueble.IdInmueble = id;
                    contexto.Inmueble.Update(inmueble);
                    contexto.SaveChanges();
                    return Ok(inmueble);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var entidad = contexto.Inmueble.Include(e => e.Propietario).FirstOrDefault(e => e.IdInmueble == id && e.Propietario.Email == User.Identity.Name);
                if (entidad != null)
                {
                    contexto.Inmueble.Remove(entidad);
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
