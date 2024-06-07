using System.Globalization;

namespace JsonToTable;

internal static class DataTransformer
{
    public static Dictionary<string, object>[] Transform(Dictionary<string, object>[] data, string commands)
    {
        string[] commandList = commands
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(c => c.Trim())
            .ToArray();

        foreach (string command in commandList)
        {
            if (data.Length == 0)
            {
                return data;
            }

            data = command switch
            {
                var c when c.StartsWith("find", StringComparison.OrdinalIgnoreCase) => Find(data, c),
                var c when c.StartsWith("where", StringComparison.OrdinalIgnoreCase) => Where(data, c),
                var c when c.StartsWith("order by", StringComparison.OrdinalIgnoreCase) => OrderBy(data, c),
                var c when c.StartsWith("select", StringComparison.OrdinalIgnoreCase) => Select(data, c),
                var c when c.StartsWith("skip", StringComparison.OrdinalIgnoreCase) => Skip(data, c),
                var c when c.StartsWith("take", StringComparison.OrdinalIgnoreCase) => Take(data, c),
                _ => throw new InvalidOperationException("Invalid command")
            };
        }

        return data;
    }

    public static Dictionary<string, object>[] Find(Dictionary<string, object>[] data, string command)
    {
        string searchKeyword = command[4..].Trim();
        return data
            .Where(d => d.Values.Any(v => (v ?? "").ToString()!.Contains(searchKeyword, StringComparison.OrdinalIgnoreCase)))
            .ToArray();
    }

    public static Dictionary<string, object>[] Where(Dictionary<string, object>[] data, string command)
    {
        string[] parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();
        if (parts.Length < 3)
        {
            throw new InvalidOperationException("Invalid where command");
        }

        string columnName = parts[0];
        string operatorName = parts[1].ToLower(CultureInfo.InvariantCulture);
        string value = parts[2];

        if (!double.TryParse(value, out double number))
        {
            return operatorName switch
            {
                "eq" => data.Where(d => d[columnName]?.ToString() == value).ToArray(),
                "ne" => data.Where(d => d[columnName]?.ToString() != value).ToArray(),
                _ => throw new InvalidOperationException(
                    "Invalid operator, supported operators for non numbers are: eq, ne")
            };
        }

        return operatorName switch
        {
            "lt" => data.Where(d => double.Parse(d[columnName]?.ToString() ?? "0") < number).ToArray(),
            "gt" => data.Where(d => double.Parse(d[columnName]?.ToString() ?? "0") > number).ToArray(),
            "eq" => data.Where(d => double.Parse(d[columnName]?.ToString() ?? "0") == number).ToArray(),
            "ne" => data.Where(d => double.Parse(d[columnName]?.ToString() ?? "0") != number).ToArray(),
            "le" => data.Where(d => double.Parse(d[columnName]?.ToString() ?? "0") <= number).ToArray(),
            "ge" => data.Where(d => double.Parse(d[columnName]?.ToString() ?? "0") >= number).ToArray(),
            _ => throw new InvalidOperationException(
                "Invalid operator, supported operators are: lt, gt, eq, ne, le, ge")
        };
    }

    public static Dictionary<string, object>[] OrderBy(Dictionary<string, object>[] data, string command)
    {
        string[] parts = command[8..].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 1)
        {
            throw new InvalidOperationException("Invalid order by command");
        }

        string columnName = parts[0];
        bool isDescending = parts.Length > 1 && parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);
        bool isNumber = data.Any(d => double.TryParse(d[columnName]?.ToString(), out _));

        return isNumber
            ? isDescending
                ? [.. data.OrderByDescending(d => double.Parse(d[columnName]?.ToString() ?? "0"))]
                : [.. data.OrderBy(d => double.Parse(d[columnName]?.ToString() ?? "0"))]
            : isDescending
                ? [.. data.OrderByDescending(d => d[columnName]?.ToString())]
                : [.. data.OrderBy(d => d[columnName]?.ToString())];
    }

    public static Dictionary<string, object>[] Select(Dictionary<string, object>[] data, string command)
    {
        string[] columns = command[6..].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim()).ToArray();
        return data.Select(d => columns.ToDictionary(c => c, c => d[c])).ToArray();
    }

    public static Dictionary<string, object>[] Skip(Dictionary<string, object>[] data, string command)
    {
        if (!int.TryParse(command[4..], out int count))
        {
            throw new InvalidOperationException("Invalid skip command");
        }

        return [.. data.Skip(count)];
    }

    public static Dictionary<string, object>[] Take(Dictionary<string, object>[] data, string command)
    {
        if (!int.TryParse(command[4..], out int count))
        {
            throw new InvalidOperationException("Invalid take command");
        }

        return [.. data.Take(count)];
    }
}
