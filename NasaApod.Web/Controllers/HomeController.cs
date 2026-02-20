using Microsoft.AspNetCore.Mvc;
using NasaApodMvc.Services;

namespace NasaApodMvc.Controllers;

public class HomeController : Controller
{
    private readonly ApodRepository _repository;

    public HomeController(ApodRepository repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _repository.GetAllAsync();
        return View(items);
    }
}
