# NASA APOD Gallery

A .NET 8 ASP.NET Core MVC application that fetches Astronomy Picture of the Day (APOD) data from the NASA API and stores it in a SQL Server database using pure ADO.NET.

## Prerequisites
- .NET 8 SDK
- SQL Server Express or LocalDB
- SSMS or Azure Data Studio
- NASA API Key

## Database Setup
1. Open SSMS or Azure Data Studio.
2. Execute the included `database.sql` script.
3. This script will automatically create:
   - Database: `NasaApodDb`
   - Table: `dbo.ApodEntries`
   - Unique Index: `UX_ApodEntries_ApodDate` (prevents duplicate date insertions)

## Configuration (`appsettings.json`)
Provide your database connection and NASA API key in `appsettings.json`.

**A) SQL Server Express Example:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=DESKTOP-NJJLRH\\SQLEXPRESS;Database=NasaApodDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Nasa": {
    "ApiKey": "YOUR_NASA_API_KEY_HERE"
  }
}
```

**B) LocalDB Example:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=NasaApodDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Nasa": {
    "ApiKey": "YOUR_NASA_API_KEY_HERE"
  }
}
```

## Run Instructions

**Using .NET CLI:**
```bash
dotnet restore
dotnet run
```

**Using Visual Studio:**
1. Open the project solution.
2. Press F5 to build and launch the application.

## Usage
- **Gallery (`/`):** View successfully imported APOD items. Displays image, title, and date, sorted with the newest records first.
- **Import Data (`/Apod/Import`):** Select a date range to fetch from NASA and save to the database.
  - **Inserted:** New image records successfully saved.
  - **Skipped:** Records ignored because they already exist, are not images (e.g., videos), or are missing critical data.
  - *Re-import Proof:* Re-importing the exact same date range will securely result in `Inserted: 0` and `Skipped: N`.
  - *Validation:* Future dates, reversed dates (Start > End), and ranges over 30 days are strictly blocked by both UI and backend validation.

## Verification Checklist
- [ ] `database.sql` executed successfully to create `NasaApodDb` and `dbo.ApodEntries`.
- [ ] `UX_ApodEntries_ApodDate` unique index prevents duplicate date entries.
- [ ] Connection string strongly targets the correct SQL instance.
- [ ] Import page properly displays strict validations (No future dates). 
- [ ] Exact duplicate imports return expected `Inserted = 0`.
- [ ] Gallery correctly queries ADO.NET and displays results newest first.

## Troubleshooting

| Symptom | Fix / Cause |
| :--- | :--- |
| **"Network-related or instance-specific error"** | Connection string mismatch. Ensure the `Server` name in `appsettings.json` exactly matches your active SSMS instance. |
| **"Invalid object name 'dbo.ApodEntries'"** | The `database.sql` script was either not executed, or was accidentally executed against the `master` database. |
| **Import button immediately shows Red alert** | The UI validation blocked the request (e.g., future date selected, range > 30 days). Adjust dates and retry. |
| **Import completes but Inserted = 0** | The dates were either already imported previously (duplicates safely skipped), or the records returned were videos (non-image media is skipped). |
| **"NASA API unavailable" (502 Error)** | The server experienced a DNS/Network timeout reaching NASA, or the NASA service is temporarily down. |
