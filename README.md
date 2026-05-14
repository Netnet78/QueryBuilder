# QueryBuilder

QueryBuilder is a lightweight, flexible C# library designed to help developers construct complex, advanced server-side queries with ease. By providing a fluent API and robust expression building, it allows for dynamic filtering, sorting, and paging that translates directly to server-side execution (e.g., SQL via Entity Framework).

## Features

- **Fluent API:** Chainable methods for building queries intuitively.
- **Server-Side Execution:** Designed to work with `IQueryable<T>`, ensuring filters are applied at the database level rather than in memory.
- **Dynamic Filtering:** Support for building complex `WHERE` clauses from runtime inputs.
- **Nested Expressions:** Handle related entities and nested logic (AND/OR groups).
- **Type-Safe:** Leverages C# generics and lambda expressions to catch errors at compile-time.
- **Extensible:** Easily add custom operators or specific database logic.

## Installation

Install QueryBuilder via NuGet:

```bash
dotnet add package QueryBuilder