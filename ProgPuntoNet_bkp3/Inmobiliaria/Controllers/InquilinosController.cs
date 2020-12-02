using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria.Controllers
{
    [Authorize]
    public class InquilinosController : Controller
    {
        IRepositorio<Inquilino> repo;

        public InquilinosController(IRepositorio<Inquilino> repositorio)
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

        // GET: Inquilino/Details/5
        public ActionResult Details(int id)
        {
            var inq = repo.GetById(id);
            if (inq != null)
                return View(inq);
            else
            {
                TempData["Error"] = "No se encontro el inquilino";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Inquilino/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Inquilino/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inquilino inquilino)
        {
            try
            {
                TempData["Nombre"] = inquilino.Nombre;
                //Valida con las anotaciones hechas en el model Inquilino
                if (ModelState.IsValid)
                {
                    var res = repo.Alta(inquilino);
                    if (res != -1)
                    {
                        TempData["Id"] = inquilino.IdInquilino;
                        //el Redirect hace que se pierda la info del Viewbag/Viewdata por lo que se puede utilizar el 
                        //TempData de la linea de arriba
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["Error"] = "El inquilino no pudo darse de alta";
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

        // GET: Inquilino/Edit/5
        public ActionResult Edit(int id)
        {
            var inq = repo.GetById(id);
            if (inq != null)
                return View(inq);
            else
            {
                TempData["Error"] = "No se encontro el inquilino";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Inquilino/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {
                    Inquilino i = repo.GetById(id);
                    i.Nombre = collection[nameof(i.Nombre)].ToString();
                    i.Apellido = collection[nameof(i.Apellido)].ToString();
                    i.Dni = collection[nameof(i.Dni)].ToString();
                    i.Telefono = collection[nameof(i.Telefono)].ToString();
                    i.Email = collection[nameof(i.Email)].ToString();
                    i.Direccion = collection[nameof(i.Direccion)].ToString();

                    var res = repo.Modificacion(i);
                    if (res != -1)
                        return RedirectToAction(nameof(Index));
                    else
                    {
                        TempData["Error"] = "No se pudo editar el inquilino";
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

        // GET: Inquilino/Delete/5
        public ActionResult Delete(int id)
        {
            var inq = repo.GetById(id);
            if (inq != null)
                return View(inq);
            else
            {
                TempData["Error"] = "No se encontro el inquilino";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Inquilino/Delete/5
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
                    TempData["Error"] = "No se pudo eliminar el inquilino";
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