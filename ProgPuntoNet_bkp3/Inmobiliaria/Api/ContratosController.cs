using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Inmobiliaria.Api
{
    [Route("api/[controller]")]
    //[ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ContratosController : ControllerBase
    {
        private readonly DataContext contexto;

        public ContratosController(DataContext context)
        {
            contexto = context;
        }

        // GET: api/Contratos
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var usuario = User.Identity.Name;
                var contratos = contexto.Contrato.Include(e => e.Inmueble).ThenInclude(x => x.Propietario).Where(e => e.Inmueble.Propietario.Email == usuario);

                return Ok(contratos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Contratos/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var usuario = User.Identity.Name;
                var contrato = contexto.Contrato.Include(e => e.Inmueble).ThenInclude(x => x.Propietario).Where(e => e.Inmueble.Propietario.Email == usuario).Single(e => e.IdContrato == id);

                /*if (contrato == null)
                {
                    return NotFound();
                }*/

                return Ok(contrato);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT: api/Contratos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromForm] Contrato contrato)
        {
            try
            {
                if (ModelState.IsValid && contexto.Contrato.AsNoTracking().Include(x => x.Inmueble).ThenInclude(e => e.Propietario).FirstOrDefault(e => e.IdInmueble == id && e.Inmueble.Propietario.Email == User.Identity.Name) != null)
                {
                    contrato.IdContrato = id;
                    contexto.Contrato.Update(contrato);
                    contexto.SaveChanges();
                    return Ok(contrato);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST: api/Contratos
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] Contrato contrato)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //contrato.IdPropietario = contexto.Propietario.Single(e => e.Email == User.Identity.Name).IdPropietario;
                    contexto.Contrato.Add(contrato);
                    contexto.SaveChanges();
                    return CreatedAtAction(nameof(Get), new { id = contrato.IdContrato }, contrato);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // DELETE: api/Contratos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var entidad = contexto.Contrato.Include(x => x.Inmueble).ThenInclude(e => e.Propietario).FirstOrDefault(e => e.IdInmueble == id && e.Inmueble.Propietario.Email == User.Identity.Name);
                if (entidad != null)
                {
                    contexto.Contrato.Remove(entidad);
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

        private bool ContratoExists(int id)
        {
            return contexto.Contrato.Any(e => e.IdContrato == id);
        }
    }
}
