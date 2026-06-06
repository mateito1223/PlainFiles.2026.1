//namespace Core;

//public class Person
//{
//    public int Id { get; set; }
//    public string Name { get; set; } = null!;
//    public int Age { get; set; }
//}

namespace Core;

public class Person
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string City { get; set; } = null!;
    public decimal Balance { get; set; }

    public static Person? Parse(string line)
    {
        var fields = line.Split(',');
        if (fields.Length < 6) return null;
        if (!int.TryParse(fields[0].Trim(), out int id)) return null;
        if (!decimal.TryParse(fields[5].Trim(), System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out decimal balance)) return null;
        return new Person
        {
            Id = id,
            FirstName = fields[1].Trim(),
            LastName = fields[2].Trim(),
            Phone = fields[3].Trim(),
            City = fields[4].Trim(),
            Balance = balance
        };
    }

    public override string ToString() =>
        $"{Id},{FirstName},{LastName},{Phone},{City},{Balance.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}";
}