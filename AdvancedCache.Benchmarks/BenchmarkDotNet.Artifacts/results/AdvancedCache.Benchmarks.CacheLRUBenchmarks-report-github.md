```

BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.7291/22H2/2022Update)
Intel Core i7-6500U CPU 2.50GHz (Max: 2.59GHz) (Skylake), 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.101
  [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3


```
| Method                             | Mean     | Error    | StdDev   | Allocated |
|----------------------------------- |---------:|---------:|---------:|----------:|
| Benchmark_Obtenir_Element_Existant | 30.29 ns | 1.193 ns | 3.500 ns |         - |
| Benchmark_Inserer_Avec_Eviction    | 40.85 ns | 1.371 ns | 3.912 ns |         - |
