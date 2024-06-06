using System.Text.Json;

using JsonToTable;

const char LeftUpperCorner = '┌';
const char RightUpperCorner = '┐';
const char LeftLowerCorner = '└';
const char RightLowerCorner = '┘';
const char HorizontalLine = '─';
const char VerticalLine = '│';
const char LeftTee = '├';
const char RightTee = '┤';

string? json = "", line;
while ((line = Console.ReadLine()) != null)
{
    json += line;
}

if (string.IsNullOrWhiteSpace(json))
{
    Console.WriteLine("Please provide a JSON string.");
    return;
}

bool isArray = json.Trim().StartsWith('[');
if (isArray)
{
    json = JsonSerializer.Serialize(JsonSerializer.Deserialize<Dictionary<string, object>[]>(json));
    Dictionary<string, object>[]? data = JsonSerializer.Deserialize<Dictionary<string, object>[]>(json);
    if (data is null || data.Length == 0)
    {
        return;
    }

    data = DataTransformer.Transform(data, string.Join(' ', args));

    if (data.Length == 0)
    {
        return;
    }

    PrintTable(data);
}
else
{
    json = JsonSerializer.Serialize(JsonSerializer.Deserialize<Dictionary<string, object>>(json));
    Dictionary<string, object>? data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
    if (data is null)
    {
        return;
    }

    PrintTransposedTable(data);
}

static void PrintTransposedTable(Dictionary<string, object> data)
{
    int[] columnLengths = [data.Keys.Max(x => x.Length), data.Values.Max(x => (x ?? "").ToString()!.Length)];
    int headerLength = columnLengths.Sum(x => x) + 7;

    PrintTopLine(headerLength);

    foreach (KeyValuePair<string, object> kvp in data)
    {
        Console.WriteLine(
            $"{VerticalLine} {kvp.Key.PadRight(columnLengths[0])} {VerticalLine} " +
            $"{(kvp.Value ?? "").ToString()!.PadRight(columnLengths[1])} {VerticalLine}");
    }

    PrintBottomLine(headerLength);
}

static void PrintTable(params Dictionary<string, object>[] data)
{
    int[] columnLengths = GetColumnLengths(data);
    int headerLength = GetHeaderLength(data, columnLengths);

    PrintHeader(headerLength, columnLengths, [.. data.First().Keys]);

    foreach (Dictionary<string, object> item in data)
    {
        for (int i = 0; i < columnLengths.Length; i++)
        {
            KeyValuePair<string, object> kvp = item.ElementAt(i);
            Console.Write($"{VerticalLine} {(kvp.Value ?? "").ToString()!.PadRight(columnLengths[i])} ");
        }
        Console.WriteLine(VerticalLine);
    }

    PrintBottomLine(headerLength);
}

static void PrintHeader(int headerLength, int[] columnLengths, string[] headers)
{
    PrintTopLine(headerLength);

    for (int i = 0; i < headers.Length; i++)
    {
        Console.Write($"{VerticalLine} {headers[i].PadRight(columnLengths[i])} ");
    }
    Console.WriteLine(VerticalLine);

    PrintMiddleLine(headerLength);
}

static int[] GetColumnLengths(Dictionary<string, object>[] data)
{
    var columnLengths = data.First().ToDictionary(x => x.Key, x => x.Key.ToString().Length);
    foreach (Dictionary<string, object> item in data)
    {
        foreach (KeyValuePair<string, object> kvp in item)
        {
            columnLengths[kvp.Key] = Math.Max(columnLengths[kvp.Key], (kvp.Value ?? "").ToString()?.Length ?? 0);
        }
    }

    return [.. columnLengths.Values];
}

static int GetHeaderLength(Dictionary<string, object>[] data, int[] columnLengths) =>
    columnLengths.Sum(x => x) + (columnLengths.Length * 3) + 1;

static void PrintTopLine(int length) =>
    Console.WriteLine($"{LeftUpperCorner}{new string(HorizontalLine, length - 2)}{RightUpperCorner}");
static void PrintMiddleLine(int length) =>
    Console.WriteLine($"{LeftTee}{new string(HorizontalLine, length - 2)}{RightTee}");
static void PrintBottomLine(int length) =>
    Console.WriteLine($"{LeftLowerCorner}{new string(HorizontalLine, length - 2)}{RightLowerCorner}");
