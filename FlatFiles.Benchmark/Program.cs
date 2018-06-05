using System;
using System.Diagnostics;
using System.Linq;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace FlatFiles.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ManualConfig();
            configuration.KeepBenchmarkFiles = false;
            configuration.Add(StatisticColumn.Min);
            configuration.Add(StatisticColumn.Max);
            configuration.Add(DefaultConfig.Instance.GetColumnProviders().ToArray());
            configuration.Add(DefaultConfig.Instance.GetLoggers().ToArray());
            configuration.Add(DefaultConfig.Instance.GetDiagnosers().ToArray());
            configuration.Add(DefaultConfig.Instance.GetAnalysers().ToArray());
            configuration.Add(DefaultConfig.Instance.GetJobs().ToArray());
            configuration.Add(DefaultConfig.Instance.GetValidators().ToArray());

            BenchmarkRunner.Run<SimpleCsvTester>(configuration);

            //var tester = new AsyncVsSyncTest();
            //for (int i = 0; i != 10; ++i)
            //{
            //    tester.SyncTest();
            //}

            //var stopwatch = Stopwatch.StartNew();
            //string syncResult = tester.SyncTest();
            //stopwatch.Stop();
            //Console.Out.WriteLine($"Sync Execution Time: {stopwatch.Elapsed}");
            //Console.Out.WriteLine($"Sync Result Count: {syncResult.Length}");

            //for (int i = 0; i != 10; ++i)
            //{
            //    tester.AsyncTest().Wait();
            //}

            //stopwatch.Restart();
            //string asyncResult = tester.AsyncTest().Result;
            //stopwatch.Stop();
            //Console.Out.WriteLine($"Async Execution Time: {stopwatch.Elapsed}");
            //Console.Out.WriteLine($"Async Result Count: {asyncResult.Length}");

            Console.Out.Write("Hit <enter> to exit...");
            Console.In.ReadLine();
        }
    }
}
