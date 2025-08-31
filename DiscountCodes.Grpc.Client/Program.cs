using DiscountCodes.Grpc;
using Google.Protobuf;
using Grpc.Net.Client;

var serverAddress = "https://localhost:7268";
Console.WriteLine($"Connecting to gRPC server at {serverAddress}");

using var channel = GrpcChannel.ForAddress(serverAddress);
var client = new Discount.DiscountClient(channel);

while (true)
{
    Console.WriteLine("\nDiscount Code Service Client");
    Console.WriteLine("1. Generate discount codes");
    Console.WriteLine("2. Use a discount code");
    Console.WriteLine("3. Exit");
    Console.Write("Choose an option (1-3): ");
    
    var choice = Console.ReadLine()?.Trim();
    
    switch (choice)
    {
        case "1":
            await GenerateDiscountCodes(client);
            break;
        case "2":
            await UseDiscountCode(client);
            break;
        default:
            Console.WriteLine("Invalid option. Please choose 1, 2, or 3.");
            break;
    }
}

static async Task GenerateDiscountCodes(Discount.DiscountClient client)
{
    Console.Write("Enter number of codes to generate (max 2000): ");
    if (!uint.TryParse(Console.ReadLine()?.Trim(), out uint count))
    {
        Console.WriteLine("Invalid count. Please enter a number between 1 and 2000.");
        return;
    }

    Console.Write("Enter code length (7 or 8): ");
    if (!uint.TryParse(Console.ReadLine()?.Trim(), out uint lengthU))
    {
        Console.WriteLine("Invalid length. Please enter 7 or 8.");
        return;
    }

    // build request: Length is a single byte inside ByteString
    var request = new GenerateRequest
    {
        Count = count,                                  
        Length = ByteString.CopyFrom([(byte)lengthU])
    };
    
    try
    {
        var response = await client.GenerateAsync(request);
        
        if (response.Result)
        {
            Console.WriteLine($"\nSuccessfully generated {response.Codes.Count} discount codes:");
            foreach (var code in response.Codes)
            {
                Console.WriteLine($"- {code}");
            }
        }
        else
        {
            Console.WriteLine("Failed to generate all requested codes.");
            if (response.Codes.Count > 0)
            {
                Console.WriteLine($"Generated {response.Codes.Count} codes:");
                foreach (var code in response.Codes)
                {
                    Console.WriteLine($"- {code}");
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error generating discount codes: {ex.Message}");
    }
}

static async Task UseDiscountCode(Discount.DiscountClient client)
{
    Console.Write("Enter discount code to use: ");
    var code = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(code))
    {
        Console.WriteLine("Invalid code. Please enter a valid discount code.");
        return;
    }

    var request = new UseCodeRequest { Code = code.ToUpper() };

    try
    {
        var response = await client.UseCodeAsync(request);

        byte status = response.Result[0];
        string resultMessage = status switch
        {
            0 => "Success! Discount code used successfully.",
            1 => "Invalid discount code.",
            2 => "Discount code has already been used.",
            _ => $"Unknown result code: {status}"
        };

        Console.WriteLine(resultMessage);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error using discount code: {ex.Message}");
    }
}
