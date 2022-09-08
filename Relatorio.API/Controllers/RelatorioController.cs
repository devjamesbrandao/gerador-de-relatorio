using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Relatorio.Data.Context;

namespace Relatorio.Controllers
{
    public class RelatorioController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RelatorioController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var relatorios = _context.Relatorios.ToList();
            return View(relatorios);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Relatorio.Core.Models.Relatorio relatorio)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(relatorio);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Something went wrong {ex.Message}");
                }
            }
            ModelState.AddModelError(string.Empty, $"Something went wrong, invalid model");
            return View();
        }        
    }
}