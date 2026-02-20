using Microsoft.AspNetCore.Mvc;
using NasaApodMvc.Models;
using NasaApodMvc.Services;

namespace NasaApodMvc.Controllers;

public class ApodController : Controller
{
    private readonly NasaApodClient _client;
    private readonly ApodRepository _repository;

    public ApodController(NasaApodClient client, ApodRepository repository)
    {
        _client = client;
        _repository = repository;
    }

    [HttpGet]
    public IActionResult Import()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Import(string? startDate, string? endDate)
    {
        // Server validation 1: Missing dates -> 400
        if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate))
        {
            return BadRequest("StartDate and EndDate are required");
        }

        if (!DateOnly.TryParse(startDate, out var start) || !DateOnly.TryParse(endDate, out var end))
        {
             return BadRequest("Invalid date format.");
        }

        // Server validation: No future dates -> 400
        var today = DateOnly.FromDateTime(DateTime.Today);
        if (start > today || end > today)
        {
            return BadRequest("Future dates are not allowed.");
        }

        // Server validation 2: Start > End -> 400
        if (end < start)
        {
            return BadRequest("StartDate must be <= EndDate");
        }

        // Server validation 3: Range limit -> 400
        var rangeDays = end.DayNumber - start.DayNumber + 1;
        if (rangeDays > 30)
        {
            return BadRequest("Maximum allowed range is 30 days");
        }

        try
        {
            var dtos = await _client.GetRangeAsync(start, end);
            int inserted = 0;
            int skipped = 0;

            foreach (var dto in dtos)
            {
                // Data mapping validation
                if (dto.Date == default || string.IsNullOrWhiteSpace(dto.Url) || string.IsNullOrWhiteSpace(dto.Title))
                {
                    skipped++;
                    continue;
                }

                if (dto.MediaType != "image")
                {
                    skipped++;
                    continue;
                }

                var item = new ApodItem
                {
                    ApodDate = dto.Date,
                    Title = dto.Title,
                    Explanation = dto.Explanation,
                    MediaType = dto.MediaType,
                    Url = dto.Url,
                    ServiceVersion = dto.ServiceVersion
                };

                int result = await _repository.InsertIfNewAsync(item);
                if (result > 0) inserted++;
                else skipped++;
            }

            ViewBag.Inserted = inserted;
            ViewBag.Skipped = skipped;
            ViewBag.Message = $"Import complete. Inserted: {inserted}, Skipped: {skipped}.";
        }
        catch (HttpRequestException ex)
        {
            // NASA API failure handling -> 502
            return StatusCode(502, $"NASA API unavailable: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }

        return View();
    }
}
