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
    public class ContratosController : Controller
    {
        IRepositorio<Contrato> repo;
        IRepositorio<Inquilino> repoInq;
        IRepositorioInmueble<Inmueble> repoIn;

        public ContratosController(IRepositorio<Contrato> repositorio, IRepositorioInmueble<Inmueble> repositorioIn, IRepositorio<Inquilino> repositorioInq)
        {
            repo = repositorio;
            repoInq = repositorioInq;
            repoIn = repositorioIn;
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

        // GET: Contrato/Details/5
        public ActionResult Details(int id)
        {
            var con = repo.GetById(id);
            if (con != null)
                return View(con);
            else
            {
                TempData["Error"] = "No se encontro el contrato";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Contrato/Create
        public ActionResult Create()
        {
            ViewBag.Inmuebles = repoIn.GetAll();//se podria devolver como parametro a la vista (es otra forma)
            ViewBag.Inquilinos = repoInq.GetAll();//se podria devolver como parametro a la vista (es otra forma)
            return View();
        }

        // POST: Contrato/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Contrato contrato)
        {
            try
            {
                //Valida con las anotaciones hechas en el model Contrato
                if (ModelState.IsValid)
                {
                    var res = repo.Alta(contrato);
                    if (res != -1)
                    {
                        TempData["Id"] = contrato.IdContrato;
                        //el Redirect hace que se pierda la info del Viewbag/Viewdata por lo que se puede utilizar el 
                        //TempData de la linea de arriba
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["Error"] = "El contrato no pudo darse de alta";
                        ViewBag.Inmuebles = repoIn.GetAll();
                        ViewBag.Inquilinos = repoInq.GetAll();
                        return View();
                    }
                }
                else
                {
                    ViewBag.Inmuebles = repoIn.GetAll();
                    ViewBag.Inquilinos = repoInq.GetAll();
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                ViewBag.Inmuebles = repoIn.GetAll();
                ViewBag.Inquilinos = repoInq.GetAll();
                return View();
            }
        }

        // GET: Contrato/Edit/5
        public ActionResult Edit(int id)
        {
            var contrato = repo.GetById(id);
            if (contrato != null)
            {
                ViewBag.Inmuebles = repoIn.GetAll();
                ViewBag.Inquilinos = repoInq.GetAll();
                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];
                if (TempData.ContainsKey("Error"))
                    ViewBag.Error = TempData["Error"];
                return View(contrato);
            }
            else
            {
                TempData["Error"] = "No se encontro el contrato";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Contrato/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Contrato entidad)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {
                    entidad.IdContrato = id;
                    
                    var res = repo.Modificacion(entidad);
                    if (res != -1)
                        return RedirectToAction(nameof(Index));
                    else
                    {
                        ViewBag.Inmuebles = repoIn.GetAll();
                        ViewBag.Inquilinos = repoInq.GetAll();
                        return View(entidad);
                    }
                }
                else
                {
                    Contrato c = repo.GetById(id);
                    ViewBag.Inmuebles = repoIn.GetAll();
                    ViewBag.Inquilinos = repoInq.GetAll();
                    return View(c);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Contrato/Delete/5
        public ActionResult Delete(int id)
        {
            var contrato = repo.GetById(id);
            if (contrato != null)
            {
                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];
                if (TempData.ContainsKey("Error"))
                    ViewBag.Error = TempData["Error"];
                return View(contrato);
            }
            else
            {
                TempData["Error"] = "No se encontro el contrato";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Contrato/Delete/5
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
                    var contrato = repo.GetById(id);
                    TempData["Error"] = "No se pudo eliminar el contrato";
                    return View(contrato);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return RedirectToAction(nameof(Delete));
            }
        }
    }
}