using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Relatorio.Core.Interfaces;
using Relatorio.Data.Context;

namespace Relatorio.Controllers
{
    public class RelatorioController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IMessageBusPublishService _messageBus;

        public RelatorioController(ApplicationDbContext context, IMessageBusPublishService messageBus)
        {
            _context = context;
            _messageBus = messageBus;
        }

        public IActionResult Index()
        {
            var relatorios = _context.Relatorios.ToList();
            return View(relatorios);
        }

        public ActionResult Health()
        {
            return Redirect("http:localhost:5000/dashboard");
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
                    
                    var json = JsonConvert.SerializeObject(relatorio);
                    
                    var body = Encoding.UTF8.GetBytes(json);

                    _messageBus.Publish("Relatorios", body);

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