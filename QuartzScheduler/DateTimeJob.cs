namespace QuartzScheduler;

using Quartz;
using System.Threading.Tasks;

internal sealed class DateTimeJob : IJob
{
    public async Task Execute(IJobExecutionContext context) =>
        await Console.Out.WriteLineAsync($"Now is: {DateTime.UtcNow} {context.MergedJobDataMap["UserName"]}:{context.MergedJobDataMap["ExtraData"]}");
}
