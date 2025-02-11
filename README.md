BenchmarkDotNet v0.14.0, Windows 10 (10.0.19045.5247/22H2/2022Update)  
AMD Ryzen 7 5800X3D, 1 CPU, 16 logical and 8 physical cores  
.NET SDK 9.0.101  
  [Host]     : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX2  
  DefaultJob : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX2  


| Method                       | Mean     | Error    | StdDev   | Allocated |
|----------------------------- |---------:|---------:|---------:|----------:|
| SSE_Parser                   | 19.03 us | 0.050 us | 0.045 us |  29.84 KB |
| SSE_StreamReader             | 22.03 us | 0.038 us | 0.032 us |  91.91 KB |
| SSE_Parser_Deserialize       | 68.33 us | 0.106 us | 0.094 us |  72.03 KB |
| SSE_StreamReader_Deserialize | 70.64 us | 0.202 us | 0.157 us | 134.09 KB |
