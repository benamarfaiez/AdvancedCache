```

BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.7291/22H2/2022Update)
Intel Core i7-6500U CPU 2.50GHz (Max: 2.59GHz) (Skylake), 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.101
  [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3


```
| Method                                            | Mean        | Error     | StdDev     | Rank | Allocated |
|-------------------------------------------------- |------------:|----------:|-----------:|-----:|----------:|
| Benchmark_ARC_Hit_Dans_Liste_Fantome              |    54.65 ns |  2.002 ns |   5.745 ns |    1 |         - |
| Benchmark_ARC_Obtenir_Element_Existant            |    68.44 ns |  3.100 ns |   9.141 ns |    2 |         - |
| Benchmark_ARC_Inserer_Avec_Eviction_Et_Adaptation | 1,081.09 ns | 44.895 ns | 132.375 ns |    3 |         - |
