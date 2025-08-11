using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Tipo_Datos.Data;
using Tipo_Datos.Models.Entidades;

namespace Tipo_Datos.Controllers
{
    public class ClientesController : Controller
    {
        private readonly DatosDbContext _dbContext;
        public ClientesController(DatosDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _dbContext.Clientes.ToListAsync());
        }

        public IActionResult Nuevo() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> 
            Nuevo([Bind("Nombres,Email,Telefono,Direccion,Cedula_RUC,isDelete")] ClientesModel cliente)
        {
            if (ModelState.IsValid)
            {
                cliente.Create_At = DateTime.Now;
                _dbContext.Add(cliente);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return  View(cliente);
        }

        public async Task<IActionResult> Editar(int? Id)
        {
            if (Id == null) return NotFound();
            var cliente = await _dbContext.Clientes.FindAsync(Id);
            if(cliente==null) return NotFound();

            return View(cliente);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(int id, [Bind("Id,Nombres,Email,Telefono,Direccion,Cedula_RUC,isDelete")] ClientesModel cliente)
        {
            if (id != cliente.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    cliente.Update_At = DateTime.Now;
                    _dbContext.Update(cliente);
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExiste(cliente.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }
        public bool ClienteExiste(int id)
        {
            return _dbContext.Clientes.Any(c => c.Id == id);
        }

        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null) return NotFound();
            var cliente = await _dbContext.Clientes.FirstOrDefaultAsync(c => c.Id == id);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        [HttpPost, ActionName("Eliminar")]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var cliente = await _dbContext.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            _dbContext.Clientes.Remove(cliente);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }


    }
}


