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
            configuration.Add(StatisticColumn.Min);
            configuration.Add(StatisticColumn.Max);
            configuration.Add(DefaultConfig.Instance.GetColumnProviders().ToArray());
            configuration.Add(DefaultConfig.Instance.GetLoggers().ToArray());
            configuration.Add(DefaultConfig.Instance.GetDiagnosers().ToArray());
            configuration.Add(DefaultConfig.Instance.GetAnalysers().ToArray());
            configuration.Add(DefaultConfig.Instance.GetJobs().ToArray());
            configuration.Add(DefaultConfig.Instance.GetValidators().ToArray());

            var tester = new AsyncVsSyncTest();
            Stopwatch timer = Stopwatch.StartNew();
            tester.SyncTest();
            timer.Stop();
            Console.Out.WriteLine($"Sync: {timer.Elapsed}.");

            timer.Restart();
            tester.AsyncTest().Wait();
            timer.Stop();
            Console.Out.WriteLine($"Async: {timer.Elapsed}");

            //BenchmarkRunner.Run<SimpleCsvTester>(configuration);
            //for (int i = 0; i != 1000; ++i)
            //{
            //    var tester = new PropertyVsFieldTester();
            //    tester.RunPropertyTest();
            //    tester.RunFieldTest();
            //}

            Console.Out.Write("Hit <enter> to exit...");
            Console.In.ReadLine();
        }
    }
}
