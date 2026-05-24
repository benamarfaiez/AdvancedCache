```

BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.7291/22H2/2022Update)
Intel Core i7-6500U CPU 2.50GHz (Max: 2.59GHz) (Skylake), 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.101
  [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3


```
| Method                            | Capacite | Mean     | Error    | StdDev   | Median   | Rank | Allocated |
|---------------------------------- |--------- |---------:|---------:|---------:|---------:|-----:|----------:|
| &#39;Obtenir (Cache Miss)&#39;            | 100      | 12.17 ns | 0.327 ns | 0.808 ns | 11.84 ns |    1 |         - |
| &#39;Obtenir (Cache Miss)&#39;            | 10000    | 19.29 ns | 0.270 ns | 0.210 ns | 19.30 ns |    2 |         - |
| &#39;Obtenir (Cache Hit)&#39;             | 100      | 49.22 ns | 0.568 ns | 0.504 ns | 49.38 ns |    3 |         - |
| &#39;Inserer (Mise à jour)&#39;           | 100      | 51.32 ns | 0.685 ns | 0.572 ns | 51.05 ns |    4 |         - |
| &#39;Inserer avec Éviction W-TinyLFU&#39; | 100      | 52.81 ns | 0.742 ns | 0.794 ns | 52.67 ns |    4 |         - |
| &#39;Inserer (Mise à jour)&#39;           | 10000    | 84.67 ns | 1.709 ns | 1.515 ns | 84.39 ns |    5 |         - |
| &#39;Obtenir (Cache Hit)&#39;             | 10000    | 85.05 ns | 0.836 ns | 0.782 ns | 85.21 ns |    5 |         - |
| &#39;Inserer avec Éviction W-TinyLFU&#39; | 10000    | 85.11 ns | 1.080 ns | 0.958 ns | 85.41 ns |    5 |         - |
