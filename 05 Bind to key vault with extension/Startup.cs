using Bind_to_key_vault_with_extension;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Willezone.Azure.WebJobs.Extensions.AzureKeyVault;

[assembly: WebJobsStartup(typeof(Startup))]
namespace Bind_to_key_vault_with_extension
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            var tempProvider = builder.Services.BuildServiceProvider();
            var config = tempProvider.GetRequiredService<IConfiguration>();
            builder.AddAzureKeyVault(config["AzureKeyVault_Uri"]);
        }
    }
}
