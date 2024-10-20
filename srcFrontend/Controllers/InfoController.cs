using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using srcFrontend.Models;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace srcFrontend.Controllers;

public class InfoController : Controller
{
    
    private readonly IConfiguration _configuration;

    public InfoController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var apiUrl = _configuration["ApiUrl"];
        ViewBag.ApiUrl = apiUrl;
        using HttpClient client = new HttpClient();
        try
        {
            HttpResponseMessage response = await client.GetAsync(apiUrl + "/Info/GetCheckedInEmployees");
            HttpResponseMessage response2 = await client.GetAsync(apiUrl + "/Info/GetCheckedOutEmployees");

            if (response.IsSuccessStatusCode && response2.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStreamAsync();
                var responseBody2 = await response2.Content.ReadAsStreamAsync();
                ValueTask<List<Employee>?> deserializeTask = JsonSerializer.DeserializeAsync<List<Employee>>(responseBody);
                ValueTask<List<Employee>?> deserializeTask2 = JsonSerializer.DeserializeAsync<List<Employee>>(responseBody2);
                var checkedInEmployees = await deserializeTask;
                var checkedOutEmployees = await deserializeTask2;
                Employee employee = new Employee();

                return View((checkedInEmployees,checkedOutEmployees,employee));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return null;
    }
    [HttpGet("/Info/FilterEmployeesFront")]
    public async Task<IActionResult> GetFilteredEmployeesAsync(string sortBy, string department, string role, bool checker)
    {
        var apiUrl = _configuration["ApiUrl"];
        ViewBag.ApiUrl = apiUrl;
        using HttpClient client = new HttpClient();
        try
        {
            string apiUrlFilter = $"{apiUrl}/Info/FilterEmployees?sortBy={sortBy}&department={department}&role={role}&checker={checker}";;
            HttpResponseMessage response = await client.GetAsync(apiUrlFilter);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStreamAsync();
                ValueTask<List<Employee>?> deserializeTask = JsonSerializer.DeserializeAsync<List<Employee>>(responseBody);
                var filteredEmployees = await deserializeTask;

                if (checker)
                {
                    return PartialView("_InfoPartial", filteredEmployees);
                }

                return PartialView("_InfoPartialRemote", filteredEmployees);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return null;
    }
}