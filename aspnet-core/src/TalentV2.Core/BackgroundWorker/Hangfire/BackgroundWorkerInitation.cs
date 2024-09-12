using Abp.Dependency;
using Abp.Threading.BackgroundWorkers;
using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace TalentV2.BackgroundWorker.Hangfire
{
	public class BackgroundWokerInitation
	{
		public static IIocManager iocManager = IocManager.Instance;
		public static readonly ConcurrentDictionary<string, object> workers =new();
		static BackgroundWokerInitation()
		{
			workers.TryAdd("Crawling CV From Firebase Trigger",
				new WorkerJob<CrawlCVFromFirebaseWorker>(
					iocManager.Resolve<CrawlCVFromFirebaseWorker>(),
					firebase => firebase.HangfireIntegrated()));

			workers.TryAdd("Crawling CV From AWS Trigger",
				new WorkerJob<CrawlCVFromAWSWorker>(
					iocManager.Resolve<CrawlCVFromAWSWorker>(),
					aws => aws.HangfireIntegrated()));

			workers.TryAdd("Noticing Interview Trigger",
				new WorkerJob<NoticeInterviewWorker>(
					iocManager.Resolve<NoticeInterviewWorker>(),
					interview => interview.HangfireIntegrated()));

			workers.TryAdd("Noticing Interview Result Trigger",
				new WorkerJob<NoticeInterviewResultWorker>(
					iocManager.Resolve<NoticeInterviewResultWorker>(),
					resultInterview => resultInterview.HangfireIntegrated()));
		}

		public class WorkerJob<T> where T : IBackgroundWorker
		{
			public T Worker { get; set; }
			public Expression<Action<T>> Trigger { get; set; }

			public WorkerJob(T worker, Expression<Action<T>> trigger)
			{
				Worker = worker;
				Trigger = trigger;
			}
		}
	}
}
