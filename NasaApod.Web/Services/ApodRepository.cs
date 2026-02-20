using System.Data;
using System.Data.SqlClient;
using NasaApodMvc.Models;

namespace NasaApodMvc.Services;

public class ApodRepository
{
    private readonly string _connectionString;

    public ApodRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is missing.");
    }

    public async Task<int> InsertIfNewAsync(ApodItem item)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        string sql = @"
            IF NOT EXISTS (SELECT 1 FROM ApodEntries WHERE ApodDate = @ApodDate)
            BEGIN
                INSERT INTO ApodEntries (ApodDate, Title, Explanation, Url, MediaType, ServiceVersion)
                VALUES (@ApodDate, @Title, @Explanation, @Url, @MediaType, @ServiceVersion);
                SELECT 1;
            END
            ELSE
            BEGIN
                SELECT 0;
            END";

        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@ApodDate", SqlDbType.Date).Value = item.ApodDate.ToDateTime(TimeOnly.MinValue);
        cmd.Parameters.Add("@Title", SqlDbType.NVarChar, 300).Value = item.Title;
        cmd.Parameters.Add("@Explanation", SqlDbType.NVarChar, -1).Value = (object?)item.Explanation ?? DBNull.Value;
        cmd.Parameters.Add("@Url", SqlDbType.NVarChar, 1000).Value = item.Url;
        cmd.Parameters.Add("@MediaType", SqlDbType.NVarChar, 50).Value = item.MediaType;
        cmd.Parameters.Add("@ServiceVersion", SqlDbType.NVarChar, 50).Value = (object?)item.ServiceVersion ?? DBNull.Value;

        var result = await cmd.ExecuteScalarAsync();
        return result != null ? (int)result : 0;
    }

    public async Task<List<ApodItem>> GetAllAsync()
    {
        var items = new List<ApodItem>();
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        string sql = "SELECT Id, ApodDate, Title, Explanation, Url, MediaType, ServiceVersion, SavedAt FROM ApodEntries ORDER BY ApodDate DESC";
        using var cmd = new SqlCommand(sql, conn);
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            items.Add(new ApodItem
            {
                Id = reader.GetInt32(0),
                ApodDate = DateOnly.FromDateTime(reader.GetDateTime(1)),
                Title = reader.GetString(2),
                Explanation = reader.IsDBNull(3) ? null : reader.GetString(3),
                Url = reader.GetString(4),
                MediaType = reader.GetString(5),
                ServiceVersion = reader.IsDBNull(6) ? null : reader.GetString(6),
                SavedAt = reader.GetDateTime(7)
            });
        }
        return items;
    }
}
