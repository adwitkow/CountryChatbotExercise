using CountryChatbotExercise.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CountryChatbotExercise.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountryController : ControllerBase
    {
        private const string RestCountriesBaseUri = "https://restcountries.eu/rest/v2/";

        private readonly ILogger<CountryController> _logger;

        public CountryController(ILogger<CountryController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<RestCountry>> Get()
        {
            return await GetAll();
        }

        [HttpGet("all")]
        public async Task<IEnumerable<RestCountry>> GetAll()
        {
            return await RequestRestCountryEndpoint<IEnumerable<RestCountry>>("all");
        }

        [HttpGet("name/{country}")]
        public async Task<RestCountry> GetCountry(string country)
        {
            return await RequestRestCountryEndpoint<RestCountry>($"name/{country}");
        }

        private async Task<T> RequestRestCountryEndpoint<T>(string endpoint)
        {
            // According to https://stackoverflow.com/a/49588931/5832956,
            // it's better to create a new instance of RestClient for each request.
            var client = new RestClient(RestCountriesBaseUri);
            var request = new RestRequest(endpoint, Method.GET, DataFormat.Json);

            var result = await client.GetAsync<T>(request);

            return result;
        }
    }
}
