```

BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.7291/22H2/2022Update)
Intel Core i7-6500U CPU 2.50GHz (Max: 2.59GHz) (Skylake), 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.101
  [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3


```
| Method                                            | Mean        | Error     | StdDev    | Rank | Allocated |
|-------------------------------------------------- |------------:|----------:|----------:|-----:|----------:|
| Benchmark_ARC_Hit_Dans_Liste_Fantome              |    58.57 ns |  1.646 ns |  4.802 ns |    1 |         - |
| Benchmark_ARC_Obtenir_Element_Existant            |    62.92 ns |  1.831 ns |  5.371 ns |    1 |         - |
| Benchmark_ARC_Inserer_Avec_Eviction_Et_Adaptation | 1,082.21 ns | 30.589 ns | 89.711 ns |    2 |         - |
