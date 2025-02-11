using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Net.ServerSentEvents;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ConsoleApp1
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            BenchmarkRunner.Run<Bench>();
        }
    }
}

[MemoryDiagnoser(false)]
public class Bench
{
    private static readonly MemoryStream stream = new MemoryStream(File.ReadAllBytes("bench.txt"));
    private static readonly StreamReader reader = new StreamReader(stream);

    private readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    [Benchmark]
    public async Task<int> SSE_Parser()
    {
        stream.Position = 0;
        int count = 0;

        var parser = SseParser.Create(stream, (type, data) =>
        {
            var str = Encoding.UTF8.GetString(data);
            return str;
        });
        await foreach (var item in parser.EnumerateAsync())
        {
            if(string.Equals(item.EventType, "state", StringComparison.OrdinalIgnoreCase))
            {
                count++;
            }
        }

        return count;
    }


    [Benchmark]
    public async Task<int> SSE_StreamReader()
    {
        stream.Position = 0;
        int count = 0;
        bool handleNext = false;

        while (true)
        {
            string data = await reader.ReadLineAsync();

            if (data == null)
            {
                break;
            }

            if(handleNext)
            {
                count++;
            }

            handleNext = string.Equals(data, "event: state", StringComparison.OrdinalIgnoreCase);
        }

        return count;
    }

    [Benchmark]
    public async Task<int> SSE_Parser_Deserialize()
    {
        stream.Position = 0;
        int count = 0;

        var parser = SseParser.Create(stream, (type, data) =>
        {
            var str = Encoding.UTF8.GetString(data);
            return str;
        });
        await foreach (var item in parser.EnumerateAsync())
        {
            if (string.Equals(item.EventType, "state", StringComparison.OrdinalIgnoreCase))
            {
                var json = JsonSerializer.Deserialize<EspEvent>(item.Data, jsonOptions);
                count += json != null ? 1 : 0;
            }
        }

        return count;
    }


    [Benchmark]
    public async Task<int> SSE_StreamReader_Deserialize()
    {
        stream.Position = 0;
        int count = 0;
        bool handleNext = false;

        while (true)
        {
            string data = await reader.ReadLineAsync();

            if (data == null)
            {
                break;
            }

            if (handleNext)
            {
                var json = JsonSerializer.Deserialize<EspEvent>(data.AsSpan(6), jsonOptions);
                count += json != null ? 1 : 0;
            }

            handleNext = string.Equals(data, "event: state", StringComparison.OrdinalIgnoreCase);
        }

        return count;
    }
}

public class EspEvent
{
    public override string ToString()
    {
        return $"Id: {Id}, Value: {Value}, Name: {Name}, State: {State}, Event_Type: {Event_Type}";
    }

    public string Id { get; set; }
    public object Value { get; set; }
    public string Name { get; set; }
    public string State { get; set; }
    public string Event_Type { get; set; }
}