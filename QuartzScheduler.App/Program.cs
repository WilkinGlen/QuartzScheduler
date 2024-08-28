using Quartz;
using QuartzScheduler;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddQuartz();
builder.Services.AddQuartzHostedService(opt => opt.WaitForJobsToComplete = true);

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var quartzService = new QuartzSchedulerService(config);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

await quartzService.RunAllSchedules();

app.Run();
