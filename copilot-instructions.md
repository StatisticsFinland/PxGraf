Architecture context:
- Before starting any task, read `docs/architecture.md` to understand the solution structure, project layout, key data flows, and where relevant code lives. This reduces exploration time and avoids incorrect assumptions about file locations or responsibilities.
- Update `docs/architecture.md` if you make any changes to the solution structure, project layout, or key data flows. This ensures that the documentation remains accurate and helpful for future reference.

Follow these guidelines when writing code:

Coding conventions:
- Always use explicit type names instead of 'var' for local variables.
- Prefer new() expressions for object instantiation when the type is clear from context. For collections, use [] initializer syntax where supported (e.g., List<int> numbers = [1, 2, 3];).
- Use collection initializers to both create and populate collections in a single statement.
  Example: List<int> numbers = [1, 2, 3]; instead of new List<int>() { 1, 2, 3 }.
- Declare local variables as const or readonly if they are not reassigned after initialization. This improves code clarity and prevents accidental modification.
- Always include XML documentation comments for public methods and classes.
- Properties of a class should be declared in the order of their visibility: public, protected, internal, private.
- Properties should be declared before methods in a class.
- Use primary constructors for classes where applicable to simplify the class definition and initialization.
- Constructors should be declared before methods in a class.
- Methods should be declared in the order of their visibility: public, protected, internal, private.

Logging:
- When writing logging statements, use template strings for better performance and readability.
- If the logging statement is in a catch block, include the exception object in the log call to capture the stack trace and other details.
- Try to use logger scope to provide context for the logs.

Unit testing:
- Use NUnit as the unit testing framework.
- Use Moq for mocking dependencies in unit tests.
- Use the Assert.That() syntax for all assertions.
- When making multiple assertions in a single test, group them with `using (Assert.EnterMultipleScope()) { ... }`.

Other:
- When refactoring code, update the documentation comments to reflect the changes made.
- Before starting large refactoring tasks or adding new features, create a plan markdown file in the `docs/plans` directory outlining the steps you intend to take. Update this plan as you progress. This helps maintain a clear roadmap and allows for the developer to track progress and make adjustments as needed.