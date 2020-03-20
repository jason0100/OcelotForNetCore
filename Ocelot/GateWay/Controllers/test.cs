using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GateWay.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class test : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<test> _logger;

        public test(ILogger<test> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<GateWay.model> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new model
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
