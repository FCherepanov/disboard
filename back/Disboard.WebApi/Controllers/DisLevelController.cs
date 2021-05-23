namespace Disboard.WebApi.Controllers
{
    using Disboard.WebApi.Classes;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using ML;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    [ApiController]
    [Route("[controller]")]
    public class DisLevelController : ControllerBase
    {


        readonly Forecaster[] _forecasters;
        readonly int[] _regId;

        public DisLevelController(IWebHostEnvironment environment)
        {
            _forecasters = Dictionaries.DisReasons.Select(x => new Forecaster(Path.Combine(environment.ContentRootPath, $"model{x.Key}.zip"), x.Key)).ToArray();
            var json = System.IO.File.ReadAllText(Path.Combine(environment.ContentRootPath, "admin_level_4.json"));
            var regions = JsonConvert.DeserializeObject<MapData>(json);
            _regId = regions.features.Select(x => x.id).ToArray();
        }

        [HttpGet]
        public IEnumerable<DisLevel> Get(int dt)
        {
            var response = new List<DisLevel>();
            foreach (var region in _regId)
            {
                var res = _forecasters.Select(x => new DisLevel { RegionId =region, DisasterId = x.DisasterId, Value = (int)Math.Round(x.Forecast(dt, region)[0])*10 }).OrderByDescending(x => x.Value).First();
                res.Caption = Dictionaries.DisReasons[res.DisasterId];
                response.Add(res);
            }

            return response.OrderByDescending(x=>x.Value).ToArray();
        }
    }
}
