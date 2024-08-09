# FJVApp

## Overview
FJVApp is a .NET application designed to fetch, process, and report records from various sources. The application follows a layered architecture, promoting maintainability and scalability while adhering to SOLID principles.

## Application Flow
1. **Fetching Records**: The application starts by fetching records from specified sources (JSON and XML) using the `JsonRecordFetcher` and `XmlRecordFetcher` classes. These classes implement the `IRecordFetcher` interface, allowing for easy extension and substitution.

2. **Processing Records**: Once the records are fetched, they are processed by the `RecordProcessor`. This component utilizes the `RecordMatcher` to categorize records into joined and orphaned records.

3. **Reporting Records**: After processing, the results are reported using the `RecordReporter`, which sends the categorized records to a specified endpoint.

4. **Error Handling**: The application includes robust error handling, retry mechanisms, and logging to ensure reliability during the fetching and processing stages.

## Architecture
The application is structured using a **Layered Architecture**, which organizes code by functionality into distinct layers. This approach enhances separation of concerns and makes it easier to manage and understand the codebase.

## How to Run the Application

1. **Run the Python Fixture**:
   - First, execute the following command to run the Python script that starts the endpoints:
   ```bash
   python3 ./fixture_3.py
   ```

2. **Prerequisites**:
   - [.NET SDK](https://dotnet.microsoft.com/download) (version 8.0 or later)

3. **Restore Dependencies**:
   ```bash
   dotnet restore
   ```

4. **Run the Application**:
   ```bash
   dotnet run
   ```

5. **Validate the categorization of the sent records, running the following command::**
   ```bash
   ./check.sh
   ```
