using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace FlatFiles.Benchmark
{
    class Program
    {
        static void Main()
        {
            RunBenchmarks();

            //runPerformanceMonitor();
        }

        private static void RunBenchmarks()
        {
            var configuration = new ManualConfig()
            {
                Options = ConfigOptions.KeepBenchmarkFiles
            };
            configuration.AddColumn(StatisticColumn.Min);
            configuration.AddColumn(StatisticColumn.Max);
            configuration.AddColumnProvider(DefaultConfig.Instance.GetColumnProviders().ToArray());
            configuration.AddLogger(DefaultConfig.Instance.GetLoggers().ToArray());
            configuration.AddDiagnoser(DefaultConfig.Instance.GetDiagnosers().ToArray());
            configuration.AddAnalyser(DefaultConfig.Instance.GetAnalysers().ToArray());
            configuration.AddJob(DefaultConfig.Instance.GetJobs().ToArray());
            configuration.AddValidator(DefaultConfig.Instance.GetValidators().ToArray());

            BenchmarkRunner.Run<CoreBenchmarkSuite>(configuration);

            Console.Out.Write("Hit <enter> to exit...");
            Console.In.ReadLine();
        }

        [SuppressMessage("CodeQuality", "IDE0051")]
        private static void RunPerformanceMonitor()
        {
            var tester = new AsyncVsSyncTest();
            for (int i = 0; i != 10; ++i)
            {
                tester.SyncTest();
            }

            var stopwatch = Stopwatch.StartNew();
            string syncResult = tester.SyncTest();
            stopwatch.Stop();
            Console.Out.WriteLine($"Sync Execution Time: {stopwatch.Elapsed}");
            Console.Out.WriteLine($"Sync Result Count: {syncResult.Length}");

            for (int i = 0; i != 10; ++i)
            {
                tester.AsyncTest().Wait();
            }

            stopwatch.Restart();
            string asyncResult = tester.AsyncTest().Result;
            stopwatch.Stop();
            Console.Out.WriteLine($"Async Execution Time: {stopwatch.Elapsed}");
            Console.Out.WriteLine($"Async Result Count: {asyncResult.Length}");
        }
    }
}
