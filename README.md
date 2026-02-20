# NASA APOD Gallery — ASP.NET Core MVC (.NET 8)

Fetches NASA Astronomy Picture of the Day data via API, stores images in SQL Server using ADO.NET, and displays them in a responsive gallery.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server Express or LocalDB
- NASA API key ([get one here](https://api.nasa.gov/))
- SSMS or Azure Data Studio (to run the SQL script)

## Database Setup

1. Open `database.sql` in SSMS or Azure Data Studio.
2. Execute the script — it creates `NasaApodDb` and the `ApodEntries` table with a unique index on `ApodDate`.

## Configuration

Open `appsettings.json` and update:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=NasaApodDb;Trusted_Connection=True;"
  },
  "Nasa": {
    "ApiKey": "YOUR_NASA_API_KEY_HERE"
  }
}
```

- Replace `YOUR_NASA_API_KEY_HERE` with your key (or use `DEMO_KEY` for limited testing).
- Adjust the connection string if using SQL Server Express instead of LocalDB.

## Run the Application

### Visual Studio
1. Open `NasaApod.Web.csproj` in Visual Studio.
2. Press **F5** to build and run.

### dotnet CLI
```bash
cd NasaApod.Web
dotnet restore
dotnet run
```

Browse to `https://localhost:5001` (or `http://localhost:5000`).

## Usage

1. Navigate to **Import** (`/Apod/Import`).
2. Select a **Start Date** and **End Date**, then click **Fetch & Save**.
3. The app fetches the NASA APOD JSON array for that range.
4. Only `media_type == "image"` items are saved; videos are skipped.
5. Navigate to **Gallery** (`/`) to view saved images newest-first.

## Verification Checklist

- [ ] After import, **Inserted** count shows number of new images saved.
- [ ] **Skipped** count shows duplicates + non-image items.
- [ ] Re-importing the same date range shows **Inserted: 0** (all duplicates skipped).
- [ ] Gallery displays images in descending date order.
- [ ] Empty gallery shows "No images saved yet" message.

## Troubleshooting

| Issue | Solution |
|-------|----------|
| **Invalid date range** | Ensure end date ≥ start date. NASA API rejects reversed ranges. |
| **Missing API key** | App throws `InvalidOperationException`. Add `Nasa:ApiKey` in `appsettings.json`. |
| **SQL connection error** | Verify SQL Server is running. Check `DefaultConnection` string. Ensure `NasaApodDb` exists. |
| **NASA API 403** | Invalid or expired API key. Get a new one at [api.nasa.gov](https://api.nasa.gov/). |
| **NASA API 429** | Rate limit exceeded. `DEMO_KEY` allows 30 req/hr. Wait or use a registered key. |
| **NASA API 500** | NASA server error. Retry after a few minutes. |
| **No images appear** | The selected range may only contain videos. Try a different date range. |
