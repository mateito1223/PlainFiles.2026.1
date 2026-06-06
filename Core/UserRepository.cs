namespace Core;

public class UserRepository
{
    private readonly string _path;

    public UserRepository(string path)
    {
        _path = path;
        var dir = Path.GetDirectoryName(_path);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        if (!File.Exists(_path))
        {
            File.WriteAllLines(_path, new[]
            {
                "jzuluaga,P@ssw0rd123!,true",
                "mbedoya,S0yS3gur02025*,false"
            });
        }
    }

    public List<UserAccount> ReadAll()
    {
        var result = new List<UserAccount>();
        foreach (var line in File.ReadAllLines(_path))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var u = UserAccount.Parse(line);
            if (u != null) result.Add(u);
        }
        return result;
    }

    public void WriteAll(List<UserAccount> users)
    {
        File.WriteAllLines(_path, users.Select(u => u.ToString()));
    }

    public UserAccount? FindByUsername(string username)
    {
        return ReadAll().FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
    }

    public void SetActive(string username, bool active)
    {
        var users = ReadAll();
        var user = users.FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        if (user != null)
        {
            user.IsActive = active;
            WriteAll(users);
        }
    }
}
