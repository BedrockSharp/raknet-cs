# RaknetCS Tests

This directory contains the unit tests for the RaknetCS project. We use `xUnit` as the testing framework.

## How to run tests

### Command Line

To run all tests from the command line, use the following command from the root of the repository:

```powershell
dotnet test RaknetCS.sln
```

### Visual Studio / VS Code

1.  **Visual Studio**: Open the solution `RaknetCS.sln`. Go to **Test > Run All Tests** or open the **Test Explorer** window (`Ctrl+E, T`).
2.  **VS Code**: Install the "C# Dev Kit" extension. The tests will appear in the **Testing** tab in the activity bar.

## Test Structure

- `RaknetCS.Tests/Protocol`: Contains tests for the Raknet protocol, including `RaknetReader`, `RaknetWriter`, and various packet serialization/deserialization.

## Adding New Tests

When adding new tests, ensure they are placed in the appropriate subdirectory and follow the naming convention `*Tests.cs`. Use the `[Fact]` attribute for simple tests and `[Theory]` for data-driven tests.
