using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Numerics;

var currentDirectory = Directory.GetCurrentDirectory();

var storesDirectory = Path.Combine(currentDirectory, "stores");

var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);

var salesFiles = FindFiles(storesDirectory);

var salesTotal = CalculateSalesTotal(salesFiles);

GenerateSalesSummary(salesFiles, salesTotal);

File.AppendAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{salesTotal}{Environment.NewLine}");

foreach (var file in salesFiles)
{
    Console.WriteLine(file);
}

IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();

    var foundFiles = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);

    foreach (var file in foundFiles)
    {
        var extension = Path.GetExtension(file);
       
        if (extension == ".json")
        {
            salesFiles.Add(file);
        }
    }
    return salesFiles;
}

double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
    double salesTotal = 0;

    foreach (var file in salesFiles)
    {
        string salesJson = File.ReadAllText(file);

        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);

        salesTotal += data?.Total ?? 0;
    }

    
    return salesTotal;
}
void GenerateSalesSummary(IEnumerable<string> salesFiles, double salesTotal)
{

    File.WriteAllText(Path.Combine(salesTotalDir, "salesSummary.txt"), $"Sales Summary{Environment.NewLine}{new string('-', 40)}{Environment.NewLine}Total Sales ${salesTotal:N2}{Environment.NewLine}");

    foreach (var file in salesFiles)
    {
        if (Path.GetFileName(file) == "sales.json")
        {
            string salesJson = File.ReadAllText(file);

            SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);

            var total = data?.Total ?? 0;

            var storeName = Path.GetFileName(Path.GetDirectoryName(file));
            File.AppendAllText(Path.Combine(salesTotalDir, "salesSummary.txt"), $"{storeName}: ${total:N2}{Environment.NewLine}");
        }
    }
    }

record SalesData(double Total);



