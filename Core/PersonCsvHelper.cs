namespace Core;

public class PersonCsvHelper
{
    private readonly string _path;

    public PersonCsvHelper(string path)
    {
        _path = path;
        var dir = Path.GetDirectoryName(_path);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        if (!File.Exists(_path))
            File.WriteAllText(_path, string.Empty);
    }

    public List<Person> ReadAll()
    {
        var result = new List<Person>();
        foreach (var line in File.ReadAllLines(_path))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var p = Person.Parse(line);
            if (p != null) result.Add(p);
        }
        return result;
    }

    public void WriteAll(List<Person> people)
    {
        File.WriteAllLines(_path, people.Select(p => p.ToString()));
    }
}
