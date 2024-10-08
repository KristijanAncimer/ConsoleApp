using System;
using System.Globalization;
using System.Reflection;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("Enter the full path of the CSV file:");
        string filePath = Console.ReadLine();

        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            StringBuilder logBuilder = new StringBuilder();
            var newCsvLines = new List<string>
            {
                "FirstName,LastName,BirthDate,HexFirstName,HexLastName,HexBirthDate"
            };
            newCsvLines.AddRange(lines.Select((line, index) =>
            {
                string[] columns = line.Split(',');

                if (columns.Length == 3)
                {
                    string firstName = columns[0].ToUpper();
                    string lastName = columns[1].ToUpper();
                    string[] dateFormats = { "yyyy-MM-dd", "dd-MM-yyyy"};
                    if (DateTime.TryParseExact(columns[2], dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthDate))
                    {
                        string formattedDate = birthDate.ToString("dd-MM-yyyy");
                        string hexFirstName = ConvertToHex(firstName);
                        string hexLastName = ConvertToHex(lastName);
                        string hexDate = ConvertToHex(formattedDate);

                        Console.WriteLine($"Name: {firstName}, Surname: {lastName}, Birthdate: {formattedDate}");
                        Console.WriteLine($"Hexadecimal format: {hexFirstName} {hexLastName} {hexDate}");

                        logBuilder.AppendLine($"Processed: {firstName}, {lastName}, {formattedDate} (Hex: {hexFirstName} {hexLastName} {hexDate})");

                        return $"{firstName},{lastName},{formattedDate},{hexFirstName},{hexLastName},{hexDate}";
                    }
                    else
                    {
                        Console.WriteLine($"Invalid date format on line {index + 1}.");
                        logBuilder.AppendLine($"Invalid date format on line {index + 1}: {line}");
                    }
                }
                else
                {
                    Console.WriteLine($"Invalid number of columns on line {index + 1}. Expected 3 columns but found {columns.Length}.");
                    logBuilder.AppendLine($"Invalid number of columns on line {index + 1}: {line}");
                }


                return string.Empty;
            }).Where(line => !string.IsNullOrEmpty(line)).ToList());
            string newFilePath = Path.Combine(Path.GetDirectoryName(filePath), "processed_" + Path.GetFileName(filePath));
            File.WriteAllLines(newFilePath, newCsvLines, Encoding.UTF8);
            Console.WriteLine($"Processed data saved to {newFilePath}");

            Console.WriteLine("Processing log:");
            Console.WriteLine(logBuilder.ToString());
        }
        else
        {
            Console.WriteLine("File not found. Please check the file path and try again.");
        }
    }

    static string ConvertToHex(string input)
    {
        StringBuilder hexBuilder = new StringBuilder();
        foreach (char c in input)
        {
            hexBuilder.Append(((int)c).ToString("X"));
        }
        return hexBuilder.ToString();
    }
}