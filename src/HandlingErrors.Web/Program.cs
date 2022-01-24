using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using HandlingErrors.Web;

Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>()) 
    .Build()
    .Run();
