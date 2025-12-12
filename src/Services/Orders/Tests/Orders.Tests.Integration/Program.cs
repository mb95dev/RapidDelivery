// This file is needed for WebApplicationFactory to discover the Program class
// In .NET 6+, Program.cs uses top-level statements, so we need this for testing

using Orders.API;

namespace Orders.Tests.Integration;

// Empty class to satisfy WebApplicationFactory requirement
public partial class Program { }

