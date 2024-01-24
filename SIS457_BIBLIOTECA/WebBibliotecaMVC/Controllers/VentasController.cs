using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBibliotecaMVC.Models;

namespace WebibliotecaMVCa.Controllers
{
    [Authorize]
    public class VentasController : Controller
    {
        private readonly BibliotecaContext _context;

        public VentasController(BibliotecaContext context)
        {
            _context = context;
        }

        // GET: Ventas
        public async Task<IActionResult> Index()
        {
            var bibliotecaContext = _context.Venta.Where(x => x.Estado != -1).Include(v => v.IdClienteNavigation).Include(v => v.IdUsuarioNavigation);
            return View(await bibliotecaContext.ToListAsync());
        }

        // GET: Ventas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Venta == null)
            {
                return NotFound();
            }

            var venta = await _context.Venta
                .Include(v => v.IdClienteNavigation)
                .Include(v => v.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (venta == null)
            {
                return NotFound();
            }

            return View(venta);
        }

        // GET: Ventas/Create
        public IActionResult Create()
        {
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios.Where(x => x.Estado != -1 && x.Estado != 0).Select(x => new
            {
                x.IdUsuario,
                Nombre = $"{x.Usuario1}"
            }).ToList(), "IdUsuario", "Nombre");

            ViewData["IdCliente"] = new SelectList(_context.Clientes.Where(x => x.Estado != -1 && x.Estado != 0).Select(x => new
            {
                x.Id,
                Nombre = $"{x.RazonSocial}"
            }).ToList(), "Id", "Nombre");

            return View();
            //ViewData["IdCliente"] = new SelectList(_context.Clientes, "Id", "Id");
            //ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario");
            //return View();
        }

        // POST: Ventas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdUsuario,IdCliente,TotalVenta,FechaVenta")] Venta venta)
        {
            if (!int.IsEvenInteger(venta.IdCliente) || !int.IsEvenInteger(venta.IdUsuario))
            {
                venta.UsuarioRegistro = User.Identity?.Name;
                venta.FechaRegistro = DateTime.Now;
                venta.Estado = 1;
                _context.Add(venta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCliente"] = new SelectList(_context.Clientes, "Id", "Id", venta.IdCliente);
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", venta.IdUsuario);
            return View(venta);
        }

        // GET: Ventas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Venta == null)
            {
                return NotFound();
            }

            var venta = await _context.Venta.FindAsync(id);
            if (venta == null)
            {
                return NotFound();
            }
            ViewData["IdCliente"] = new SelectList(_context.Clientes, "Id", "Id", venta.IdCliente);
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "Usuario1", venta.IdUsuario);
            return View(venta);
        }

        // POST: Ventas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdUsuario,IdCliente,TotalVenta,FechaVenta,UsuarioRegistro,FechaRegistro,Estado")] Venta venta)
        {
            if (id != venta.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    venta.UsuarioRegistro = User.Identity?.Name;
                    _context.Update(venta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VentaExists(venta.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCliente"] = new SelectList(_context.Clientes, "Id", "Id", venta.IdCliente);
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", venta.IdUsuario);
            return View(venta);
        }

        // GET: Ventas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Venta == null)
            {
                return NotFound();
            }

            var venta = await _context.Venta
                .Include(v => v.IdClienteNavigation)
                .Include(v => v.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (venta == null)
            {
                return NotFound();
            }

            return View(venta);
        }

        // POST: Ventas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Venta == null)
            {
                return Problem("Entity set 'BibliotecaContext.Venta'  is null.");
            }
            var venta = await _context.Venta.FindAsync(id);
            if (venta != null)
            {
                venta.Estado = -1;
                venta.UsuarioRegistro = User.Identity?.Name ?? "";
                //_context.Venta.Remove(venta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VentaExists(int id)
        {
            return (_context.Venta?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
