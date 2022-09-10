using System;
using System.Collections.Generic;
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
        private readonly IRelatorioRepository _repositorio;

        private readonly IMessageBusPublishService _messageBus;

        public RelatorioController(IRelatorioRepository repositorio, IMessageBusPublishService messageBus)
        {
            _repositorio = repositorio;
            _messageBus = messageBus;
        }

        [HttpGet]
        public async Task<ActionResult<List<Core.Models.Relatorio>>> Index()
        {
            var relatorios = await _repositorio.ObterTodosRelatorios();

            return View(relatorios);
        }

        public ActionResult Health()
        {
            return Redirect("/dashboard");
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
                    await _repositorio.AdicionarRelatorio(relatorio);

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