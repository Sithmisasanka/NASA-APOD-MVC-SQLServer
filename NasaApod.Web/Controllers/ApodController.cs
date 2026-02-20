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
    public async Task<IActionResult> Import(DateOnly startDate, DateOnly endDate)
    {
        if (endDate < startDate)
        {
            ModelState.AddModelError("", "End date must be on or after start date.");
            return View();
        }

        try
        {
            var dtos = await _client.GetRangeAsync(startDate, endDate);
            int inserted = 0;
            int skipped = 0;

            foreach (var dto in dtos)
            {
                if (dto.MediaType != "image")
                {
                    skipped++;
                    continue;
                }

                var item = new ApodItem
                {
                    ApodDate = dto.Date,
                    Title = dto.Title ?? "No Title",
                    Explanation = dto.Explanation,
                    MediaType = dto.MediaType,
                    Url = dto.Url ?? string.Empty,
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
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error importing: {ex.Message}");
        }

        return View();
    }
}
