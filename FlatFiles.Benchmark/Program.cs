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

            BenchmarkRunner.Run<SimpleAsyncCsvTester>(configuration);
            //var tester = new AsyncVsSyncTest();
            //tester.SyncTest();

            //var stopwatch = Stopwatch.StartNew();
            //string syncResult = tester.SyncTest();
            //stopwatch.Stop();
            //Console.Out.WriteLine(stopwatch.Elapsed);
            //Console.Out.WriteLine(syncResult.Length);

            //tester.AsyncTest().Wait();

            //stopwatch.Restart();
            //string asyncResult = tester.AsyncTest().Result;
            //stopwatch.Stop();
            //Console.Out.WriteLine(stopwatch.Elapsed);
            //Console.Out.WriteLine(asyncResult.Length);

            Console.Out.Write("Hit <enter> to exit...");
            Console.In.ReadLine();
        }
    }
}
