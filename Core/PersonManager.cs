namespace Core;

using System.Text.RegularExpressions;

public class PersonManager
{
    private readonly PersonCsvHelper _csvHelper;
    private readonly LogWriter _logger;
    private readonly string _loggedUser;

    public PersonManager(PersonCsvHelper csvHelper, LogWriter logger, string loggedUser)
    {
        _csvHelper = csvHelper;
        _logger = logger;
        _loggedUser = loggedUser;
    }

    public void ListPeople()
    {
        var people = _csvHelper.ReadAll();
        if (people.Count == 0)
        {
            Console.WriteLine("No records found.");
            _logger.WriteLog("info", _loggedUser, "Listed people: no records.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine($"{"ID",5} {"First Name",-20} {"Last Name",-20} {"Phone",-15} {"City",-15} {"Balance",15}");
        Console.WriteLine($"{"-----",5} {"--------------------",-20} {"--------------------",-20} {"---------------",-15} {"---------------",-15} {"---------------",15}");
        foreach (var p in people)
        {
            Console.WriteLine($"{p.Id,5} {p.FirstName,-20} {p.LastName,-20} {p.Phone,-15} {p.City,-15} {("$" + p.Balance.ToString("N2")),15}");
        }
        _logger.WriteLog("info", _loggedUser, $"Listed {people.Count} people.");
    }

    public void AddPerson()
    {
        var people = _csvHelper.ReadAll();

        Console.Write("ID (number): ");
        var idInput = Console.ReadLine()?.Trim() ?? string.Empty;
        if (!int.TryParse(idInput, out int newId) || newId <= 0)
        {
            Console.WriteLine("ID must be a positive number.");
            return;
        }
        if (people.Any(p => p.Id == newId))
        {
            Console.WriteLine($"ID {newId} already exists. It must be unique.");
            return;
        }

        Console.Write("First Name: ");
        var firstName = Console.ReadLine()?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(firstName)) { Console.WriteLine("First name is required."); return; }

        Console.Write("Last Name: ");
        var lastName = Console.ReadLine()?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(lastName)) { Console.WriteLine("Last name is required."); return; }

        Console.Write("Phone: ");
        var phone = Console.ReadLine()?.Trim() ?? string.Empty;
        if (!IsValidPhone(phone)) { Console.WriteLine("Invalid phone. Must be 7-15 digits."); return; }

        Console.Write("City: ");
        var city = Console.ReadLine()?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(city)) { Console.WriteLine("City is required."); return; }

        Console.Write("Balance: ");
        var balanceInput = Console.ReadLine()?.Trim() ?? string.Empty;
        if (!decimal.TryParse(balanceInput, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out decimal balance) || balance < 0)
        {
            Console.WriteLine("Balance must be a positive number.");
            return;
        }

        people.Add(new Person { Id = newId, FirstName = firstName, LastName = lastName, Phone = phone, City = city, Balance = balance });
        _csvHelper.WriteAll(people);
        Console.WriteLine("Person added successfully.");
        _logger.WriteLog("info", _loggedUser, $"Added person ID={newId}, Name={firstName} {lastName}.");
    }

    public void EditPerson()
    {
        var people = _csvHelper.ReadAll();

        Console.Write("Enter the ID of the person to edit: ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out int id)) { Console.WriteLine("Invalid ID."); return; }

        var person = people.FirstOrDefault(p => p.Id == id);
        if (person == null)
        {
            Console.WriteLine($"No person found with ID {id}.");
            _logger.WriteLog("warn", _loggedUser, $"Edit attempt on non-existent ID={id}.");
            return;
        }

        Console.WriteLine($"Editing: {person.FirstName} {person.LastName} (press ENTER to keep current value)");

        Console.Write($"First Name [{person.FirstName}]: ");
        var firstName = Console.ReadLine()?.Trim();
        if (!string.IsNullOrEmpty(firstName)) person.FirstName = firstName;

        Console.Write($"Last Name [{person.LastName}]: ");
        var lastName = Console.ReadLine()?.Trim();
        if (!string.IsNullOrEmpty(lastName)) person.LastName = lastName;

        Console.Write($"Phone [{person.Phone}]: ");
        var phone = Console.ReadLine()?.Trim();
        if (!string.IsNullOrEmpty(phone))
        {
            if (!IsValidPhone(phone)) { Console.WriteLine("Invalid phone. Changes not saved."); return; }
            person.Phone = phone;
        }

        Console.Write($"City [{person.City}]: ");
        var city = Console.ReadLine()?.Trim();
        if (!string.IsNullOrEmpty(city)) person.City = city;

        Console.Write($"Balance [{person.Balance:F2}]: ");
        var balanceInput = Console.ReadLine()?.Trim();
        if (!string.IsNullOrEmpty(balanceInput))
        {
            if (!decimal.TryParse(balanceInput, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out decimal balance) || balance < 0)
            {
                Console.WriteLine("Invalid balance. Changes not saved.");
                return;
            }
            person.Balance = balance;
        }

        _csvHelper.WriteAll(people);
        Console.WriteLine("Person updated successfully.");
        _logger.WriteLog("info", _loggedUser, $"Edited person ID={id}, Name={person.FirstName} {person.LastName}.");
    }

    public void DeletePerson()
    {
        var people = _csvHelper.ReadAll();

        Console.Write("Enter the ID of the person to delete: ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out int id)) { Console.WriteLine("Invalid ID."); return; }

        var person = people.FirstOrDefault(p => p.Id == id);
        if (person == null)
        {
            Console.WriteLine($"No person found with ID {id}.");
            _logger.WriteLog("warn", _loggedUser, $"Delete attempt on non-existent ID={id}.");
            return;
        }

        Console.WriteLine($"\nYou are about to delete:");
        Console.WriteLine($"  ID:         {person.Id}");
        Console.WriteLine($"  First Name: {person.FirstName}");
        Console.WriteLine($"  Last Name:  {person.LastName}");
        Console.WriteLine($"  Phone:      {person.Phone}");
        Console.WriteLine($"  City:       {person.City}");
        Console.WriteLine($"  Balance:    {"$" + person.Balance.ToString("N2")}");

        Console.Write("\nAre you sure you want to delete this person? (y/n): ");
        if (Console.ReadLine()?.Trim().ToLower() != "y")
        {
            Console.WriteLine("Delete cancelled.");
            _logger.WriteLog("info", _loggedUser, $"Delete of ID={id} cancelled by user.");
            return;
        }

        people.Remove(person);
        _csvHelper.WriteAll(people);
        Console.WriteLine("Person deleted successfully.");
        _logger.WriteLog("info", _loggedUser, $"Deleted person ID={id}, Name={person.FirstName} {person.LastName}.");
    }

    public void ShowReport()
    {
        var people = _csvHelper.ReadAll();
        if (people.Count == 0)
        {
            Console.WriteLine("No records to report.");
            _logger.WriteLog("info", _loggedUser, "Report requested: no records.");
            return;
        }

        var grouped = people.OrderBy(p => p.City).ThenBy(p => p.Id).GroupBy(p => p.City).ToList();
        decimal grandTotal = 0;

        Console.WriteLine();
        foreach (var group in grouped)
        {
            decimal cityTotal = 0;
            Console.WriteLine($"Ciudad: {group.Key}");
            Console.WriteLine();
            Console.WriteLine($"{"ID",-5}  {"Nombres",-15}  {"Apellidos",-15}  {"Saldo",15}");
            Console.WriteLine($"{"---",-5}  {"---------------",-15}  {"---------------",-15}  {"---------------",15}");

            foreach (var p in group)
            {
                Console.WriteLine($"{p.Id,-5}  {p.FirstName,-15}  {p.LastName,-15}  {("$" + p.Balance.ToString("N2")),15}");
                cityTotal += p.Balance;
            }

            Console.WriteLine($"{"",5}  {"",15}  {"",15}  {"===============",15}");
            Console.WriteLine($"{"Total: " + group.Key,-40}  {("$" + cityTotal.ToString("N2")),15}");
            Console.WriteLine();
            grandTotal += cityTotal;
        }

        Console.WriteLine($"{"",5}  {"",15}  {"",15}  {"===============",15}");
        Console.WriteLine($"{"Total General:",-40}  {("$" + grandTotal.ToString("N2")),15}");
        Console.WriteLine();
        _logger.WriteLog("info", _loggedUser, $"Report by city generated. Grand total: {grandTotal:F2}.");
    }

    private static bool IsValidPhone(string phone)
    {
        if (!Regex.IsMatch(phone, @"^[0-9\s\+\-]+$")) return false;
        var digits = Regex.Replace(phone, @"[^\d]", "");
        return digits.Length >= 7 && digits.Length <= 15;
    }
}