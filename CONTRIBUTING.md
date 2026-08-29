# Contributing to Lotus Planning App

Thank you for your interest in contributing to Lotus Planning App! This document provides guidelines and instructions for contributing to the project.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Setup](#development-setup)
- [How to Contribute](#how-to-contribute)
- [Coding Standards](#coding-standards)
- [Pull Request Process](#pull-request-process)
- [Reporting Bugs](#reporting-bugs)
- [Suggesting Features](#suggesting-features)

## Code of Conduct

By participating in this project, you agree to maintain a respectful and inclusive environment for all contributors.

## Getting Started

1. **Fork the repository** on GitHub
2. **Clone your fork** locally:
   ```bash
   git clone https://github.com/YOUR-USERNAME/LotusPlanningApp.git
   cd LotusPlanningApp
   ```
3. **Add upstream remote**:
   ```bash
   git remote add upstream https://github.com/JohanSmarius/LotusPlanningApp.git
   ```

## Development Setup

Follow the setup instructions in the [README.md](README.md#getting-started) to configure your development environment.

### Prerequisites
- .NET 10 SDK
- SQL Server (Express, LocalDB, or full version)
- Code editor (Visual Studio 2022, VS Code, or JetBrains Rider)

### Quick Setup
```bash
# Restore dependencies
dotnet restore

# Update database
dotnet ef database update --project Infrastructure --startup-project LotusPlanningApp/LotusPlanningApp

# Run the application
dotnet run --project LotusPlanningApp/LotusPlanningApp/LotusPlanningApp.csproj

# Run tests
dotnet test
```

## How to Contribute

### Before You Start

1. **Check existing issues** to see if your contribution is already being worked on
2. **Read the architecture documentation**:
   - [CQRS Architecture](Application/README_CQRS.md)
   - [Copilot Instructions](.github/copilot-instructions.md)
3. **Create an issue** for significant changes to discuss before starting work

### Making Changes

1. **Create a feature branch** from `main`:
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Make your changes** following the coding standards below

3. **Write or update tests** for your changes

4. **Run tests** to ensure everything works:
   ```bash
   dotnet test
   ```

5. **Build the project** to check for errors:
   ```bash
   dotnet build
   ```

6. **Commit your changes** with clear, descriptive commit messages:
   ```bash
   git commit -m "Add: Brief description of your changes"
   ```

## Coding Standards

### General Guidelines

- **Follow SOLID principles** and clean architecture
- **Use meaningful names** for variables, methods, and classes
- **Write async code** for all I/O operations
- **Add XML documentation** for public APIs
- **Avoid code duplication** - refactor common logic into helpers/services
- **Keep methods small** - each method should do one thing well

### Architecture Patterns

#### CQRS Pattern
- **Commands** for write operations (state modifications)
- **Queries** for read operations (data retrieval)
- All commands and queries should be **immutable** (use `record` types)
- Handlers should use **repositories**, not DbContext directly

Example:
```csharp
// Command
public record CreateEventCommand(EventDTO EventData) : ICommand<EventDTO>;

// Handler
public class CreateEventCommandHandler : ICommandHandler<CreateEventCommand, EventDTO>
{
    private readonly IEventRepository _eventRepository;
    
    public CreateEventCommandHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }
    
    public async Task<EventDTO> Handle(CreateEventCommand command, CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

#### Repository Pattern
- All data access through repositories
- Repository interfaces in `Application/` layer
- Repository implementations in `Infrastructure/` layer
- All repository methods should be async

### Naming Conventions

- **Commands**: `<Verb><Entity>Command` (e.g., `CreateEventCommand`, `UpdateShiftCommand`)
- **Queries**: `Get<Entity><Criteria>Query` (e.g., `GetEventByIdQuery`, `GetAllShiftsQuery`)
- **Handlers**: `<CommandOrQueryName>Handler`
- **Interfaces**: Prefix with `I` (e.g., `IEventRepository`)

### Code Style

- Use **nullable reference types** (`?`) for optional values
- Use **records** for DTOs, Commands, and Queries
- Add **XML comments** for public methods and classes:
  ```csharp
  /// <summary>
  /// Creates a new event.
  /// </summary>
  /// <param name="eventData">The event data.</param>
  /// <returns>The created event.</returns>
  public async Task<Event> CreateEventAsync(EventDTO eventData)
  ```

### Security

- **Never hardcode secrets** - use configuration or environment variables
- **Always validate user input** before processing
- **Use parameterized queries** (EF Core handles this automatically)
- **Implement proper authentication and authorization**
- **Sanitize output** to prevent XSS attacks

### Testing

- Write **unit tests** for business logic
- Write **integration tests** for database operations
- Use descriptive test names: `MethodName_Scenario_ExpectedBehavior`
- Follow the **AAA pattern** (Arrange, Act, Assert)

Example:
```csharp
[Fact]
public async Task CreateEvent_ValidData_ReturnsCreatedEvent()
{
    // Arrange
    var eventData = new EventDTO { Name = "Test Event" };
    
    // Act
    var result = await _handler.Handle(new CreateEventCommand(eventData), CancellationToken.None);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal("Test Event", result.Name);
}
```

## Pull Request Process

1. **Update documentation** if you've changed functionality
2. **Ensure all tests pass** and code builds successfully
3. **Update the CHANGELOG** if applicable
4. **Create a pull request** with:
   - Clear title describing the change
   - Detailed description of what was changed and why
   - Reference to related issues (e.g., "Fixes #123")
5. **Wait for review** - maintainers will review your PR
6. **Address feedback** if requested
7. **Squash commits** if requested before merging

### Pull Request Checklist

- [ ] Code follows the project's coding standards
- [ ] Tests have been added/updated and all pass
- [ ] Documentation has been updated
- [ ] Commit messages are clear and descriptive
- [ ] No merge conflicts with main branch
- [ ] Code has been reviewed for security issues
- [ ] CQRS pattern followed for new features
- [ ] Handlers registered in `Application/DependencyInjection.cs`

## Reporting Bugs

When reporting bugs, please include:

1. **Description** of the issue
2. **Steps to reproduce** the behavior
3. **Expected behavior**
4. **Actual behavior**
5. **Environment details**:
   - OS and version
   - .NET version
   - Browser (if applicable)
6. **Screenshots** if applicable
7. **Error messages** or logs

## Suggesting Features

When suggesting features, please:

1. **Check existing issues** to avoid duplicates
2. **Describe the feature** in detail
3. **Explain the use case** - why is this needed?
4. **Provide examples** if possible
5. **Consider the impact** on existing functionality

## Questions?

If you have questions about contributing:
1. Check the [README.md](README.md)
2. Review the [Copilot Instructions](.github/copilot-instructions.md)
3. Read the [CQRS Architecture Guide](Application/README_CQRS.md)
4. Create an issue with the "question" label

## License

By contributing, you agree that your contributions will be licensed under the same [MIT License](LICENSE.txt) that covers the project.

---

Thank you for contributing to Lotus Planning App! 🎉
