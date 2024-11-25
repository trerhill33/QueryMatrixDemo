# 🚀 Query Matrix Builder

The **Query Matrix Builder** is a Blazor WebAssembly application that empowers users to visually construct and execute complex database queries. It supports nested conditions, logical operators (`AND`, `OR`), and allows for query preview and result export.

## ✨ Features

- 🛠️ **Dynamic Query Builder**: Create queries using nested conditions and groups.
- 🔗 **Logical Operators**: Combine conditions with `AND` or `OR`.
- 👀 **Query Preview**: View the JSON representation of the query structure.
- 📋 **Result Display**: Render query results dynamically in a table or JSON format.
- 💾 **Export Results**: Download query results as a JSON file.
- 🗂️ **Entity Type Selection**: Query different entity types like `Product`, `Category`, etc.

## 🛑 Prerequisites

- 🟦 [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) or higher
- 🐘 [PostgreSQL](https://www.postgresql.org/)

## ⚙️ Installation

1. **Clone the Repository** 🌀
   ```bash
   git clone https://github.com/your-username/query-matrix-builder.git
   cd query-matrix-builder

## ⚙️ Installation

### Set Up Backend 🖥️

1. Navigate to the `Server` project directory.
2. Update the `appsettings.json` file with your PostgreSQL connection string.
3. Run the database migrations:
   ```bash
   dotnet ef database update

## 🧑‍💻 Usage

### Select an Entity Type 🗂️
- Choose an entity type from the dropdown (e.g., `Product`, `Category`).

### Add Conditions and Groups ➕
- Use the **Add Condition** button to define a query condition.
- Use the **Add Group** button to create nested logical groups
