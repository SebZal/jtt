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
    json = JsonSerializer.Serialize(JsonSerializer.Deserialize<Dictionary<string, object>[]>(json)); // minify JSON
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
    json = JsonSerializer.Serialize(JsonSerializer.Deserialize<Dictionary<string, object>>(json)); // minify JSON
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

static void PrintTable(Dictionary<string, object>[] data)
{
    string[] columnNames = GetColumnNames(data);
    AddMissingKeys(data, columnNames);

    Dictionary<string, int> columnLengths = GetColumnLengths(data, columnNames);
    int headerLength = GetHeaderLength(columnLengths);

    PrintHeader(headerLength, columnLengths, [.. columnNames]);

    foreach (Dictionary<string, object> item in data)
    {
        foreach (KeyValuePair<string, int> columnLength in columnLengths)
        {
            Console.Write($"{VerticalLine} {(item[columnLength.Key] ?? "").ToString()!.PadRight(columnLength.Value)} ");
        }
        Console.WriteLine(VerticalLine);
    }

    PrintBottomLine(headerLength);
}

static void PrintHeader(int headerLength, Dictionary<string, int> columnLengths, string[] headers)
{
    PrintTopLine(headerLength);

    foreach (string header in headers)
    {
        Console.Write($"{VerticalLine} {header.PadRight(columnLengths[header])} ");
    }
    Console.WriteLine(VerticalLine);

    PrintMiddleLine(headerLength);
}

static Dictionary<string, int> GetColumnLengths(Dictionary<string, object>[] data, string[] columnNames)
{
    var columnLengths = columnNames.ToDictionary(x => x, x => x.Length);
    foreach (Dictionary<string, object> item in data)
    {
        foreach (KeyValuePair<string, object> kvp in item)
        {
            columnLengths[kvp.Key] = Math.Max(columnLengths[kvp.Key], (kvp.Value ?? "").ToString()?.Length ?? 0);
        }
    }

    return columnLengths;
}

static string[] GetColumnNames(Dictionary<string, object>[] data) // TODO verify order
{
    // Some elements may not have all the keys, to be safe, we get all the keys from all the elements
    HashSet<string> columnNames = [];
    foreach (Dictionary<string, object> item in data)
    {
        columnNames.UnionWith(item.Keys);
    }

    return [.. columnNames];
}

static void AddMissingKeys(Dictionary<string, object>[] data, string[] columnNames)
{
    foreach (Dictionary<string, object> item in data)
    {
        foreach (string columnName in columnNames)
        {
            if (!item.ContainsKey(columnName))
            {
                item[columnName] = "";
            }
        }
    }
}

static int GetHeaderLength(Dictionary<string, int> columnLengths) =>
    columnLengths.Sum(x => x.Value) + (columnLengths.Select(x => x.Value).Count() * 3) + 1;

static void PrintTopLine(int length) =>
    Console.WriteLine($"{LeftUpperCorner}{new string(HorizontalLine, length - 2)}{RightUpperCorner}");
static void PrintMiddleLine(int length) =>
    Console.WriteLine($"{LeftTee}{new string(HorizontalLine, length - 2)}{RightTee}");
static void PrintBottomLine(int length) =>
    Console.WriteLine($"{LeftLowerCorner}{new string(HorizontalLine, length - 2)}{RightLowerCorner}");
