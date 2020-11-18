using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria.Controllers
{
    public class InmueblesController : Controller
    {
        //IRepositorio<Inmueble> repo;
        IRepositorioInmueble<Inmueble> repo;
        IRepositorioPropietario repoProp;

        public InmueblesController(IRepositorioInmueble<Inmueble> repositorio, IRepositorioPropietario repositorioProp)
        {
            repo = repositorio;
            repoProp = repositorioProp;
        }

        // GET: Inmuebles
        public ActionResult Index()
        {
            var lista = repo.GetAllWithProp();
            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            else if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            else if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(lista);
        }

        // GET: Inmuebles/Details/5
        public ActionResult Details(int id)
        {
            var inmu = repo.GetById(id);
            if (inmu != null)
                return View(inmu);
            else
            {
                TempData["Error"] = "No se encontro el inmueble";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Inmuebles/Create
        public ActionResult Create()
        {
            ViewBag.Propietarios = repoProp.GetAll();
            return View();//se podria devolver como parametro a la vista (es otra forma)
        }

        // POST: Inmuebles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inmueble inmueble)
        {
            try
            {
                //Valida con las anotaciones hechas en el model Inmueble
                if (ModelState.IsValid)
                {
                    var res = repo.Alta(inmueble);
                    if (res != -1)
                    {
                        TempData["Id"] = inmueble.IdInmueble;
                        //el Redirect hace que se pierda la info del Viewbag/Viewdata por lo que se puede utilizar el 
                        //TempData de la linea de arriba
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["Error"] = "El inmueble no pudo darse de alta";
                        return View();
                    }
                }
                else
                {
                    ViewBag.Propietarios = repoProp.GetAll();
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

        // GET: Inmuebles/Edit/5
        public ActionResult Edit(int id)
        {
            var inmueble = repo.GetById(id);
            if (inmueble != null)
            {
                ViewBag.Propietarios = repoProp.GetAll();
                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];
                if (TempData.ContainsKey("Error"))
                    ViewBag.Error = TempData["Error"];
                return View(inmueble);
            }
            else
            {
                TempData["Error"] = "No se encontro el inmueble";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Inmuebles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {
                    Inmueble i = repo.GetById(id);
                    i.Tipo = collection[nameof(i.Tipo)].ToString();
                    i.Direccion = collection[nameof(i.Direccion)].ToString();
                    i.Uso = collection[nameof(i.Uso)].ToString();
                    i.Ambientes = int.Parse(collection[nameof(i.Ambientes)].ToString());
                    i.Disponible = int.Parse(collection[nameof(i.Disponible)].ToString());
                    i.Precio = decimal.Parse(collection[nameof(i.Precio)].ToString());
                    //i.Habilitado = int.Parse(collection[nameof(i.Habilitado)].ToString());
                    i.IdPropietario = int.Parse(collection[nameof(i.IdPropietario)].ToString());

                    var res = repo.Modificacion(i);
                    if (res != -1)
                        return RedirectToAction(nameof(Index));
                    else
                    {
                        ViewBag.Propietarios = repoProp.GetAll();
                        return View(i);
                    }
                }
                else
                {
                    Inmueble i = repo.GetById(id);
                    ViewBag.Propietarios = repoProp.GetAll();
                    return View(i);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Inmuebles/Delete/5
        public ActionResult Delete(int id)
        {
            var inmu = repo.GetById(id);
            if (inmu != null)
            {
                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];
                if (TempData.ContainsKey("Error"))
                    ViewBag.Error = TempData["Error"];
                return View(inmu);
            }
            else
            {
                TempData["Error"] = "No se encontro el inmueble";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Inmuebles/Delete/5
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
                    var inmu = repo.GetById(id);
                    TempData["Error"] = "No se pudo eliminar el inmueble";
                    return View(inmu);
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