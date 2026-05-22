using BenchmarkDotNet.Running;

namespace AdvancedCache.Benchmarks
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<CacheARCBenchmarks>();
        }
    }
}
