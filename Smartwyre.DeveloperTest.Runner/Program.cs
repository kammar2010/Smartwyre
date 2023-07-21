using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Types;
using System;

namespace Smartwyre.DeveloperTest.Runner;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enter volume:");
        string volumeString = Console.ReadLine();
        var volume = GetDecimalInput(volumeString);

        Console.WriteLine("Enter Product Identifier:");
        var productIdentifier = Console.ReadLine();

        Console.WriteLine("Enter Rebate Identifier:");
        var rebateIdentifier = Console.ReadLine();

        var service = new RebateService(new RebateDataStore(), new ProductDataStore());
        var result = service.Calculate(new CalculateRebateRequest
        {
            Volume = volume,            
            ProductIdentifier = productIdentifier,
            RebateIdentifier = rebateIdentifier
        });

        Console.WriteLine($"Result success:{result.Success} rebate ammout:{result.RebateAmount}");
    }

    private static decimal GetDecimalInput(string input)
    {
        if (decimal.TryParse(input, out decimal intValue))
        {
            Console.WriteLine("The input is not a valid decimal. Using 0");
        }
        return intValue;
    }
}
