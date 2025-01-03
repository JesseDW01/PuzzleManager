# PuzzleManager

**Target Framework:** .NET 9 (Preview)

## Project Overview

PuzzleManager is a Blazor-based application designed to track and manage a family's (or group of friends') puzzle inventory and experiences. The core idea is to make it easy to:

- Keep an inventory of all puzzles (title, piece count, maker, difficulty, etc.)
- Track checkouts (who currently holds a puzzle, how long they took to complete it)
- Collect puzzle KPIs such as average time to complete, user-submitted difficulty ratings
- Help the family decide which puzzles to buy next, potentially ranking puzzle makers

## Domain Model

The domain layer consists of the following classes:

1. **Puzzle**
   - Represents a jigsaw puzzle with basic attributes like name, piece count, and difficulty rating.
   - References a `PuzzleMaker`.

2. **PuzzleMaker**
   - Represents the brand or manufacturer of puzzles.
   - Holds a collection of `Puzzle` entries.

3. **PuzzleHolder**
   - Represents a person (family member or friend) who can check puzzles in or out.

4. **PuzzleCheckout**
   - Represents a transaction record of a puzzle being checked out by a particular holder.
   - Stores checkout date, return date, and optional completion metrics.

## Future Plans

1. **Blazor UI**  
   - Implement a Blazor Server or WebAssembly project to expose CRUD operations for all domain objects.
   - Show puzzle analytics (charts) via a charting library like Chart.js, MudBlazor, or Syncfusion.

2. **Data Access Layer (EF Core)**  
   - Configure an EF Core `DbContext` that maps these domain objects to tables.
   - Use .NET migrations to manage database schema changes.

3. **Azure Deployment**  
   - Host the Blazor app on Azure App Service.
   - Use Azure SQL Database for persistent storage.
   - Integrate GitHub Actions or Azure DevOps for continuous deployment.

4. **AI & Recommendations**  
   - Predict puzzle completion times based on puzzle data (piece count, maker, etc.).
   - Suggest new puzzles to buy based on user preferences and past completion data.

## Prerequisites

- Visual Studio (Preview) with .NET 9 SDK installed
- Azure subscription (for eventual deployment)
- Basic knowledge of C#, EF Core, and Blazor

## Getting Started

1. **Clone the Repository**  
   ```bash
   git clone https://github.com/YourUser/PuzzleManager.git //todo update
   ```
