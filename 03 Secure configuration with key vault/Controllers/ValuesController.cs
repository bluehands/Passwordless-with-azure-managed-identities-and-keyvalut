using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Secure_configuration_with_key_vault.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        public IConfiguration Configuration { get; }

        public ValuesController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var secretKey = "StorageConnectionString";
            var secretValue = Configuration.GetValue<string>(secretKey);

            var publicSettingKey = "MyPublicSetting";
            var publicSettingValue= Configuration.GetValue<string>(publicSettingKey);

            return new string[] { $"Key: {secretKey}, Value: {secretValue}", $"Key: {publicSettingKey}, Value: {publicSettingValue}" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
