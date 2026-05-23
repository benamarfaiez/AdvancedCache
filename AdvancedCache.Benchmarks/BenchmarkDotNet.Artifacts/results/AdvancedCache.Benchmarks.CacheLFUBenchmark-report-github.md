```

BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.7291/22H2/2022Update)
Intel Core i7-6500U CPU 2.50GHz (Max: 2.59GHz) (Skylake), 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.101
  [Host]     : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.1 (10.0.1, 10.0.125.57005), X64 RyuJIT x86-64-v3


```
| Method                      | Capacite | Mean      | Error    | StdDev    | Median    | Rank | Gen0   | Allocated |
|---------------------------- |--------- |----------:|---------:|----------:|----------:|-----:|-------:|----------:|
| &#39;Obtenir (Cache Miss)&#39;      | 100      |  11.93 ns | 0.287 ns |  0.307 ns |  12.00 ns |    1 |      - |         - |
| &#39;Obtenir (Cache Miss)&#39;      | 10000    |  20.21 ns | 0.371 ns |  0.329 ns |  20.10 ns |    2 |      - |         - |
| &#39;Inserer avec Éviction LFU&#39; | 100      |  56.43 ns | 1.075 ns |  0.839 ns |  56.42 ns |    3 |      - |       1 B |
| &#39;Inserer (Mise à jour)&#39;     | 100      |  59.30 ns | 1.195 ns |  2.030 ns |  58.80 ns |    3 |      - |       1 B |
| &#39;Obtenir (Cache Hit)&#39;       | 100      | 101.93 ns | 9.213 ns | 26.875 ns | 102.00 ns |    4 | 0.0001 |       1 B |
| &#39;Inserer avec Éviction LFU&#39; | 10000    | 103.47 ns | 2.083 ns |  4.573 ns | 103.05 ns |    4 |      - |         - |
| &#39;Inserer (Mise à jour)&#39;     | 10000    | 109.86 ns | 4.035 ns | 11.314 ns | 104.69 ns |    4 |      - |         - |
| &#39;Obtenir (Cache Hit)&#39;       | 10000    | 120.41 ns | 4.416 ns | 12.952 ns | 116.49 ns |    5 |      - |         - |
