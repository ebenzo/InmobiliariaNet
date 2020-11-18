using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria.Controllers
{
    public class PropietariosController : Controller
    {
        IRepositorioPropietario repo;

        public PropietariosController(IRepositorioPropietario repositorio)
        {
            repo = repositorio;
        }

        public IActionResult Index()
		{
			var lista = repo.GetAll();
            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            else if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            else if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(lista);
        }

        // GET: Propietario/Details/5
        public ActionResult Details(int id)
        {
            var prop = repo.GetById(id);
            if (prop != null)
                return View(prop);
            else
            {
                TempData["Error"] = "No se encontro el propietario";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Propietario/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Propietario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Propietario propietario)
        {
            try
            {
                TempData["Nombre"] = propietario.Nombre;
                //Valida con las anotaciones hechas en el model Propietario
                if (ModelState.IsValid)
                {
                    propietario.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: propietario.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes("SALADA"),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));
                    var res = repo.Alta(propietario);
                    if (res != -1)
                    {
                        TempData["Id"] = propietario.IdPropietario;
                        //el Redirect hace que se pierda la info del Viewbag/Viewdata por lo que se puede utilizar el 
                        //TempData de la linea de arriba
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["Error"] = "El propietario no pudo darse de alta";
                        return View();
                    }
                }
                else
                    return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View();
            }
        }

        [HttpPost]
        public JsonResult Buscar(string s)
        {
            var res = repo.GetAll().Where(x => x.Nombre.Contains(s) || x.Apellido.Contains(s));
            return new JsonResult(res);
        }

        // GET: Propietario/Edit/5
        public ActionResult Edit(int id)
        {
            var prop = repo.GetById(id);
            if (prop != null)
                return View(prop);
            else
            {
                TempData["Error"] = "No se encontro el propietario";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Propietario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {
                    Propietario p = repo.GetById(id);
                    p.Nombre = collection[nameof(p.Nombre)].ToString();
                    p.Apellido = collection[nameof(p.Apellido)].ToString();
                    p.Dni = collection[nameof(p.Dni)].ToString();
                    p.Telefono = collection[nameof(p.Telefono)].ToString();
                    p.Email = collection[nameof(p.Email)].ToString();
                    p.Direccion = collection[nameof(p.Direccion)].ToString();
                    
                    var res = repo.Modificacion(p);
                    if (res != -1)
                        return RedirectToAction(nameof(Index));
                    else
                    {
                        TempData["Error"] = "No se pudo editar el propietario";
                        return View();
                    }
                }
                else
                    return View();
            }
            catch(Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View();
            }
        }

        // GET: Propietario/Delete/5
        public ActionResult Delete(int id)
        {
            var prop = repo.GetById(id);
            if (prop != null)
                return View(prop);
            else
            {
                TempData["Error"] = "No se encontro el propietario";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Propietario/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var res = repo.Baja(id);
                if (res != -1)
                    return RedirectToAction(nameof(Index));
                else
                {
                    TempData["Error"] = "No se pudo eliminar el propietario";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View();
            }
        }
    }
}