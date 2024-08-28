using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Quartz;
using QuartzScheduler;

Console.WriteLine("Hello, World!");

var builder = Host.CreateDefaultBuilder()
    .ConfigureServices((cxt, services) =>
    {
        _ = services.AddQuartz();
        _ = services.AddQuartzHostedService(opt => opt.WaitForJobsToComplete = true);
    }).Build();

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var schedulerService = new QuartzSchedulerService(config);
await schedulerService.RunAllSchedules();