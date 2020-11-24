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
    public class InquilinosController : ControllerBase
    {
        private readonly DataContext contexto;

        public InquilinosController(DataContext context)
        {
            contexto = context;
        }

        // GET: api/Inquilinos
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var usuario = User.Identity.Name;
                var inquilinos = contexto.Contrato.Include(e => e.Inmueble).ThenInclude(x => x.Propietario)
                    .Where(e => e.Inmueble.Propietario.Email == usuario).Select(x => new { Inquilino = x.Inquilino }).Distinct();
                //var inquilinos = contexto.Inquilinos.Include(x => x.Contratos).ThenInclude(e => e.Inmueble).ThenInclude(x => x.Propietario)
                //    .Where(e => e.Contratos.Inmueble.Propietario.Email == usuario);
                return Ok(inquilinos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Inquilinos/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var usuario = User.Identity.Name;
                var inquilino = contexto.Contrato.Include(e => e.Inmueble).ThenInclude(x => x.Propietario)
                    .Where(e => e.Inmueble.Propietario.Email == usuario).Single(x => x.IdInquilino == id);

                //que diferencia con este??
                //var inquilino = contexto.Contrato.Include(e => e.Inmueble).ThenInclude(x => x.Propietario)
                  //  .Where(e => e.Inmueble.Propietario.Email == usuario && e.IdInquilino == id).Single();

                return Ok(inquilino);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT: api/Inquilinos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromForm] Inquilino inquilino)
        {
            try
            {
                if (ModelState.IsValid && contexto.Contrato.AsNoTracking().Include(e => e.Inmueble).ThenInclude(x => x.Propietario).FirstOrDefault(e => e.IdInquilino == id && e.Inmueble.Propietario.Email == User.Identity.Name) != null)
                {
                    inquilino.IdInquilino = id;
                    contexto.Inquilino.Update(inquilino);
                    contexto.SaveChanges();
                    return Ok(inquilino);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST: api/Inquilinos
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] Inquilino inquilino)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //inquilino.IdInquilino = contexto.Inquilino.Include(x => x.Contrato).ThenInclude(x => x.Inmueble).ThenInclude(x => x.Propietario).Single(e => e.Email == User.Identity.Name).IdInquilino;
                    contexto.Inquilino.Add(inquilino);
                    contexto.SaveChanges();
                    return CreatedAtAction(nameof(Get), new { id = inquilino.IdInquilino }, inquilino);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // DELETE: api/Inquilinos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var entidad = new Inquilino(); /*contexto.Inquilinos.Include(x => x.Contrato).ThenInclude(x => x.Inmueble).ThenInclude(e => e.Propietario).FirstOrDefault(e => e.IdInquilino == id && e.Contrato.Propietario.Email == User.Identity.Name);*/
                if (entidad != null)
                {
                    contexto.Inquilino.Remove(entidad);
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

        private bool InquilinoExists(int id)
        {
            return contexto.Inquilino.Any(e => e.IdInquilino == id);
        }
    }
}
