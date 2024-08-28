namespace QuartzScheduler;

using Quartz;

[PersistJobDataAfterExecution]
[DisallowConcurrentExecution]
internal sealed class HelloJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var timesRun = context.MergedJobDataMap.GetInt("TimesRun");
            timesRun++;
            await Console.Out.WriteLineAsync($"Executed at: {DateTime.UtcNow}");
            await Console.Out.WriteLineAsync($"Greetings from HelloJob! Times run {timesRun}");
            context.JobDetail.JobDataMap["TimesRun"] = timesRun.ToString();
        }
        catch (Exception ex)
        {
            throw new JobExecutionException(ex);
        }
    }
}
