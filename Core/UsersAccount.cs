namespace Core;

public class UserAccount
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool IsActive { get; set; }

    public static UserAccount? Parse(string line)
    {
        var fields = line.Split(',');
        if (fields.Length < 3) return null;
        return new UserAccount
        {
            Username = fields[0].Trim(),
            Password = fields[1].Trim(),
            IsActive = fields[2].Trim().ToLower() == "true"
        };
    }

    public override string ToString() =>
        $"{Username},{Password},{IsActive.ToString().ToLower()}";
}
