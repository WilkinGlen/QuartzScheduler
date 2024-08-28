namespace QuartzScheduler;

using Microsoft.Extensions.Configuration;
using Quartz;
using Quartz.Impl;
using System.Collections.Specialized;

public interface IQuartzSchedulerService
{
    Task RunAllSchedules();
}

public sealed class QuartzSchedulerService(IConfiguration config) : IQuartzSchedulerService
{
    private readonly IConfiguration config = config;

    private const string JobName1 = "GlensJob";
    private const string JobDescription1 = "A job to test Quartz Scheduler using Sql Server.";
    private const string JobName2 = "GlensJob2";
    private const string JobDescription2 = "Another job to test Quartz Scheduler using Sql Server.";
    private const string GroupName = "GlensGroup";
    private const string TriggerName1 = "GlensTrigger";
    private const string TriggerName2 = "GlensTrigger2";
    private const string EasternTimeZoneName = "Eastern Standard Time";

    public async Task RunAllSchedules()
    {
        var properties = new NameValueCollection
        {
            ["quartz.jobStore.useProperties"] = this.config["QuartzJobStoreUseProperties"],
            ["quartz.jobStore.type"] = this.config["QuartzJobStoreType"],
            ["quartz.jobStore.driverDelegateType"] = this.config["QuartzJobStoreDriverDelegateType"],
            ["quartz.jobStore.tablePrefix"] = this.config["QuartzJobStoreTablePrefix"],
            ["quartz.jobStore.dataSource"] = this.config["QuartzJobStoreDataSource"],
            ["quartz.serializer.type"] = this.config["QuartzSerializerType"],
            ["quartz.dataSource.myDS.connectionString"] = this.config["QuartzDataSourceMyDSConnectionString"],
            ["quartz.dataSource.myDS.provider"] = this.config["QuartzDataSourceMyDSProvider"]
        };

        var schedulerFactory = new StdSchedulerFactory(properties);
        var scheduler = await schedulerFactory.GetScheduler();
        var job = await scheduler.GetJobDetail(new JobKey(JobName1, GroupName));
        var job2 = await scheduler.GetJobDetail(new JobKey(JobName2, GroupName));

        if (job == null)
        {
            job = JobBuilder.Create<HelloJob>()
                    .WithIdentity(JobName1, GroupName)
                    .WithDescription(JobDescription1)
                    .UsingJobData("TimesRun", "0")
                    .Build();

            var trigger = await scheduler.GetTrigger(new TriggerKey(TriggerName1, GroupName));
            trigger ??= TriggerBuilder.Create()
                .WithIdentity(TriggerName1, GroupName)
                .WithSchedule(CronScheduleBuilder
                .DailyAtHourAndMinute(10, 47)
                .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById(EasternTimeZoneName)))
                .Build();

            _ = await scheduler.ScheduleJob(job, trigger);
        }

        if (job2 == null)
        {
            job2 = JobBuilder.Create<DateTimeJob>()
                    .WithIdentity(JobName2, GroupName)
                    .WithDescription(JobDescription2)
                    .UsingJobData("UserName", "Glen Wilkin")
                    .UsingJobData("ExtraData", "Jana Statute")
                    .Build();

            var trigger2 = await scheduler.GetTrigger(new TriggerKey(TriggerName2, GroupName));
            trigger2 ??= TriggerBuilder.Create()
                .WithIdentity(TriggerName2, GroupName)
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(5)
                    .RepeatForever())
                .Build();

            _ = await scheduler.ScheduleJob(job2, trigger2);
        }

        await scheduler.Start();
    }
}
