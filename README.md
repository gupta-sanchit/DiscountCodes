# Discount Codes

## Description
A modular .NET 8 solution for generating, managing, and validating discount codes using **gRPC services**, **Entity Framework Core**, and **cryptography utilities**.  
This repository is designed for **extensibility** and **secure operations**.

---

## Key Features
- 🚀 **gRPC service** for discount code operations  
- 🖥️ **Client console app** to consume the gRPC API  
- 🗄️ **Persistence layer** with EF Core + SQLite  
- 🔐 **Cryptography utilities** for secure code generation  
- 🧩 **Abstractions** for easy extensibility  

---

## Technologies Used
- [.NET 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) (SDK & runtime)  
- gRPC (inter-service communication)  
- Entity Framework Core 8  
- SQLite (local DB: `db/discount.db`)   

---

## High-Level Design
For a detailed architectural overview, see the [High-Level Design](HIGH-LEVEL-DESIGN.md).

---

## Prerequisites
- .NET 8.0 SDK installed  

---

## Installation
```sh
git clone https://github.com/yourusername/DiscountCodes.git
cd DiscountCodes
dotnet restore
dotnet build
```

---

## Running the Solution

### Option 1: Visual Studio
This solution is configured for Visual Studio with a multi-project launch profile.
Open the `.sln` file in Visual Studio and use the default launch configuration to start both the Client and Server projects simultaneously. Select the "Client and Server" launch profile and press F5 to run both services together.

### Option 2: .NET CLI
You can also run the projects individually using the .NET CLI:

Start the gRPC service:
```sh
dotnet run --project DiscountCodes.GrpcService
```

#### Using the Client Project
The client project (`DiscountCodes.Grpc.Client`) is a console application that demonstrates how to interact with the gRPC server. You can use it to generate and consume discount codes via the gRPC API.

To run the client:
```sh
dotnet run --project DiscountCodes.Grpc.Client
```

By default, the client will connect to the locally running gRPC server. You may need to update the server address in the client code if your server is running on a different host or port.

**Typical client operations:**
- Generate discount codes by sending a request to the server
- Consume (use) a discount code and receive the outcome (success, already used, or invalid)

Refer to the client project's `Program.cs` for usage examples and available commands.

---

## Requirement Satisfaction

| Requirement              | How It’s Satisfied |
|---------------------------|---------------------|
| **Persistent Storage**    | EF Core + SQLite (`db/discount.db`) |
| **Code Length**           | 7–8 chars enforced by API & logic |
| **Random & Unique**       | Random generator + DB uniqueness constraint |
| **Single Use**            | DB tracks usage; codes can only be consumed once |
| **Repeatable Generation** | API allows repeated requests |
| **Batch Limit**           | Max 2,000 codes per request |
| **Parallel Processing**   | Transactional operations & concurrency handling |
| **Unit Tests**            | Modular structure supports easy testability |

---

## API Documentation
See `discount.proto` below for gRPC definitions.

### discount.proto
```proto
syntax = "proto3";
option csharp_namespace = "DiscountCodes.Grpc";
package discount;

service Discount {
  rpc Generate (GenerateRequest) returns (GenerateResponse);
  rpc UseCode  (UseCodeRequest)  returns (UseCodeResponse);
}

message GenerateRequest {
  uint32 count  = 1;  // <= 2000
  bytes length = 2;  // 7 or 8
}

message GenerateResponse {
  bool result = 1; 
  repeated string codes = 2;
}

message UseCodeRequest {
  string code = 1; // 7-8 uppercase
}

message UseCodeResponse {
  bytes result = 1;  // 0=success, 1=invalid, 2=already used
}
```
