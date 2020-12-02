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
    public class PagosController : Controller
    {
        IRepositorio<Pago> repo;
        IRepositorio<Inquilino> repoInq;
        IRepositorio<Contrato> repoCon;

        public PagosController(IRepositorio<Pago> repositorio, IRepositorio<Contrato> repositorioCon, IRepositorio<Inquilino> repositorioInq)
        {
            repo = repositorio;
            repoInq = repositorioInq;
            repoCon = repositorioCon;
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

        // GET: Pago/Details/5
        public ActionResult Details(int id)
        {
            var con = repo.GetById(id);
            if (con != null)
                return View(con);
            else
            {
                TempData["Error"] = "No se encontro el pago";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Pago/Create
        public ActionResult Create()
        {
            ViewBag.Contratos = repoCon.GetAll();//se podria devolver como parametro a la vista (es otra forma)
            ViewBag.Inquilinos = repoInq.GetAll();//se podria devolver como parametro a la vista (es otra forma)
            return View();
        }

        // POST: Pago/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pago pago)
        {
            try
            {
                //Valida con las anotaciones hechas en el model Contrato
                if (ModelState.IsValid)
                {
                    var res = repo.Alta(pago);
                    if (res != -1)
                    {
                        TempData["Id"] = pago.IdPago;
                        //el Redirect hace que se pierda la info del Viewbag/Viewdata por lo que se puede utilizar el 
                        //TempData de la linea de arriba
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["Error"] = "El pago no pudo darse de alta";
                        ViewBag.Contratos = repoCon.GetAll();
                        ViewBag.Inquilinos = repoInq.GetAll();
                        return View();
                    }
                }
                else
                {
                    ViewBag.Contratos = repoCon.GetAll();
                    ViewBag.Inquilinos = repoInq.GetAll();
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                ViewBag.Contratos = repoCon.GetAll();
                ViewBag.Inquilinos = repoInq.GetAll();
                return View();
            }
        }

        // GET: Pago/Edit/5
        public ActionResult Edit(int id)
        {
            var pago = repo.GetById(id);
            if (pago != null)
            {
                ViewBag.Contratos = repoCon.GetAll();
                ViewBag.Inquilinos = repoInq.GetAll();
                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];
                if (TempData.ContainsKey("Error"))
                    ViewBag.Error = TempData["Error"];
                return View(pago);
            }
            else
            {
                TempData["Error"] = "No se encontro el pago";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Pago/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Pago entidad)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {
                    entidad.IdPago = id;

                    var res = repo.Modificacion(entidad);
                    if (res != -1)
                        return RedirectToAction(nameof(Index));
                    else
                    {
                        ViewBag.Contratos = repoCon.GetAll();
                        ViewBag.Inquilinos = repoInq.GetAll();
                        return View(entidad);
                    }
                }
                else
                {
                    Pago p = repo.GetById(id);
                    ViewBag.Contratos = repoCon.GetAll();
                    ViewBag.Inquilinos = repoInq.GetAll();
                    return View(p);
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
            var pago = repo.GetById(id);
            if (pago != null)
            {
                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];
                if (TempData.ContainsKey("Error"))
                    ViewBag.Error = TempData["Error"];
                return View(pago);
            }
            else
            {
                TempData["Error"] = "No se encontro el pago";
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
                    TempData["Error"] = "No se pudo eliminar el pago";
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