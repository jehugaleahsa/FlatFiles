using System;
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

            BenchmarkRunner.Run<MapperReadPerformanceTester>(configuration);

            Console.Out.Write("Hit <enter> to continue...");
            Console.In.ReadLine();

            BenchmarkRunner.Run<MapperWritePerformanceTester>(configuration);            

            Console.Out.Write("Hit <enter> to exit...");
            Console.In.ReadLine();
        }
    }
}
