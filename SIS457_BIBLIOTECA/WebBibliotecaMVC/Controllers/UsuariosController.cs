using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBibliotecaMVC.Models;

namespace WebBibliotecaMVC.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly BibliotecaContext _context;

        public UsuariosController(BibliotecaContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            var bibliotecaContext = _context.Usuarios.Where(x => x.Estado != -1).Include(u => u.IdEmpleadoNavigation);
            return View(await bibliotecaContext.ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados.Where(x => x.Estado != -1 && x.Estado != 0).Select(x => new
            {
                x.IdEmpleado,
                NombreCompleto = $"{x.Nombre} {x.Apellidos}"
            }).ToList(), "IdEmpleado", "NombreCompleto");

            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdUsuario,IdEmpleado,Usuario1")] Usuario usuario)
        {
            if (!string.IsNullOrEmpty(usuario.Usuario1))
            {
                usuario.Clave = Util.Encrypt("sis457");
                usuario.UsuarioRegistro = "LAPTOP-3G16GEOJ\\VICTUS hp";
                usuario.FechaRegistro = DateTime.Now;
                usuario.Estado = 1;
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "IdEmpleado", usuario.IdEmpleado);
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados.Where(x => x.Estado != -1 && x.Estado != 0), "IdEmpleado", "Nombre", usuario.IdEmpleado);
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdUsuario,IdEmpleado,Usuario1,Clave,UsuarioRegistro,FechaRegistro,Estado")] Usuario usuario)
        {
            if (id != usuario.IdUsuario)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    usuario.UsuarioRegistro = User.Identity?.Name;
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.IdUsuario))
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
            ViewData["IdEmpleado"] = new SelectList(_context.Empleados, "IdEmpleado", "IdEmpleado", usuario.IdEmpleado);
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(m => m.IdUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Usuarios == null)
            {
                return Problem("Entity set 'bibliotecaContext.Usuarios'  is null.");
            }
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                usuario.Estado = -1;
                usuario.UsuarioRegistro = User.Identity?.Name ?? "";
                //_context.Usuarios.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return (_context.Usuarios?.Any(e => e.IdUsuario == id)).GetValueOrDefault();
        }
    }
}
