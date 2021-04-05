using Logging_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Logging_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string _apiKey = "d48f9dc859eda3f1c59c2a7df4f329b9";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<string> Get(string city)
        {
            _logger.LogInformation($"Getting the information about the weather for the {city} city.");

            string downloadUrl = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}";
            string weatherData = String.Empty;

            try
            {
                using (WebClient wc = new WebClient())
                {
                    _logger.LogTrace("Download weather data from the remote server.");
                    weatherData = wc.DownloadString(downloadUrl);
                }

                _logger.LogDebug("Weather data is successfully downloaded.");

                _logger.LogTrace("Start deserializing downloaded weather data.");
                dynamic weatherDataJson = JsonConvert.DeserializeObject<ExpandoObject>(weatherData, new ExpandoObjectConverter());

                var temp = weatherDataJson.main.temp;

                if (temp != null)
                {
                    _logger.LogInformation("Return the temperature information.");

                    return temp.ToString();
                }
                else
                {
                    _logger.LogWarning("Failed to load the temperature value for the city of {city}.", city);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"The error occurs during getting the information about the weather for the {city} city.");
            }

            return string.Empty;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
