using AdvancedCache.Benchmarks;
using BenchmarkDotNet.Running;

BenchmarkRunner.Run<CacheARCBenchmarks>();
BenchmarkRunner.Run<CacheLRUBenchmarks>();
BenchmarkRunner.Run<CacheLFUBenchmark>();
BenchmarkRunner.Run<CacheWTinyLfuBenchmarks>();
