using bachelorOppgave13.Interfaces;
using bachelorOppgave13.Models;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.Cosmos;
using Azure.Storage.Blobs;
using Container = Microsoft.Azure.Cosmos.Container;
using System.Net;
using Microsoft.IdentityModel.Tokens;

namespace bachelorOppgave13.Repositories;

public class EmployeeRepository : IEmployeeRepository
{

    private const string SubscriptionKey = "6920e2f5fcfb45dd897f754d60fb7bc8";
    private const string Endpoint = "https://bachelor13face.cognitiveservices.azure.com/";
    private string GroupID = "my-person-group20123948574834";
    private const string CosmosEndpointUri = "https://bachelor13db.documents.azure.com:443/";
    private const string CosmosPrimaryKey = "uYcNfxn2ZJwBCtgvJ3wDvf2njCzlSq7NWKTQzLw3kZysdj6aKJpjWRFdmiPs5pEmmAQlZQ7tkB0AACDbQGBrWw==";
    private const string BlobConnectionString = "DefaultEndpointsProtocol=https;AccountName=bachelor13blob;AccountKey=fM/AMbvzOAmVNbikgHQyAFFO4t3CUPV2Noet+eQL+3ciWoHPakdzBqYuGQXlj2bsY7860z1fJjFp+AStijOl4g==;EndpointSuffix=core.windows.net";

    private readonly CosmosClient cosmosClient = new CosmosClient(CosmosEndpointUri, CosmosPrimaryKey);
    
    
    public async Task<Employee> CreateEmployeeAsync(Employee employee, List<IFormFile> files)
    {
        Container container = cosmosClient.GetContainer("bachelor13db", "Persons");
        List<String> imageUrlArray = new List<String>();

        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                var blobServiceClient = new BlobServiceClient(BlobConnectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient("images");
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var blobClient = containerClient.GetBlobClient(fileName);
                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, true);
                }
                
                var imageUrl = blobClient.Uri.ToString();
                imageUrlArray.Add(imageUrl);
            }
        }
    
        employee.imageUrl = imageUrlArray.ToArray();
        
        try
        {
            ItemResponse<Employee> response = await container.CreateItemAsync(employee, new PartitionKey(employee.personId.ToString()));

            return response.Resource;
        }
        catch(Exception e)
        {
            Console.WriteLine($"Error creating employee: {e.Message}");
            throw;
        }
        
    }

    public async Task<List<Employee>> GetAllEmployeesAsync()
    {
        Container container = cosmosClient.GetContainer("bachelor13db", "Persons");

        try
        {
            List<Employee> employees = new List<Employee>();

            
            using (FeedIterator<Employee> iterator = container.GetItemQueryIterator<Employee>(new QueryDefinition("SELECT * FROM c")))
            {
                while (iterator.HasMoreResults)
                {

                    FeedResponse<Employee> response = await iterator.ReadNextAsync();
                    employees.AddRange(response);

                }
            }

            return employees;
        }
        catch(Exception e)
        {
            Console.WriteLine($"Error retrieving employees: {e.Message}");
            throw;
        }
    }

    public async Task<Employee> GetEmployeeByIdAsync(string id)
    {
        Container container = cosmosClient.GetContainer("bachelor13db", "Persons");
        var docId = await GetDocIdByPersonIdAsync(id);

        try
        {
            ItemResponse<Employee> response = await container.ReadItemAsync<Employee>(docId, new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error retrieving employee: {e.Message}");
            throw;
        }
    }

    public async Task<Employee> GetEmployeeByNameAsync(string firstName, string lastName)
    {
        var container = cosmosClient.GetContainer("bachelor13db", "Persons");
        try
        {
            
            var query = new QueryDefinition($"SELECT * FROM c WHERE c.personFirstName = @firstName AND c.personLastName = @lastName")
                .WithParameter("@firstName", firstName)
                .WithParameter("@lastName", lastName);

            var iterator = container.GetItemQueryIterator<Employee>(query);
            var response = await iterator.ReadNextAsync();

            return response.FirstOrDefault();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting employee by name: {ex.Message}");
            return null;
        }
    }

    public async Task<List<Employee>> GetCheckedInEmployeesAsync()
    {
        Container container = cosmosClient.GetContainer("bachelor13db", "Persons");

        try
        {
            var query = new QueryDefinition("SELECT * FROM c WHERE c.checkedIn = true");
            var iterator = container.GetItemQueryIterator<Employee>(query);

            var checkedInEmployees = new List<Employee>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();

                checkedInEmployees.AddRange(response.ToList());
            }

            return checkedInEmployees;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error retrieving checked in employees: {e.Message}");
            throw;
        }
    }

    public async Task<List<Employee>> GetCheckedOutEmployeesAsync()
    {
        Container container = cosmosClient.GetContainer("bachelor13db", "Persons");

        try
        {
            var query = new QueryDefinition("SELECT * FROM c WHERE c.checkedIn = false");
            var iterator = container.GetItemQueryIterator<Employee>(query);

            var checkedInEmployees = new List<Employee>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                checkedInEmployees.AddRange(response.ToList());
            }

            return checkedInEmployees;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error retrieving checked in employees: {e.Message}");
            throw;
        }
    }

    public async Task<bool> UpdateEmployeeAsync(string personId, Employee employee, List<IFormFile> files)
    {
        Container container = cosmosClient.GetContainer("bachelor13db", "Persons");
        List<String> imageUrlArray = new List<String>();
        Employee employeeOriginal = await GetEmployeeByIdAsync(personId);
        
        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                var blobServiceClient = new BlobServiceClient(BlobConnectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient("images");
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var blobClient = containerClient.GetBlobClient(fileName);
                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, true);
                }
                
                var imageUrl = blobClient.Uri.ToString();
                imageUrlArray.Add(imageUrl);
            }
        }
    
        employee.imageUrl = imageUrlArray.ToArray();
        
        
        var id = await GetDocIdByPersonIdAsync(personId);
        
        if (!string.IsNullOrEmpty(id))
        {
            employee.Id = employeeOriginal.Id;
            employee.personId = employeeOriginal.personId;
            if (employee.imageUrl.IsNullOrEmpty())
            {
                employee.imageUrl = employeeOriginal.imageUrl;
            }

            employee.checkInTime = employeeOriginal.checkInTime;
        }
        else
        {
            Console.WriteLine($"Error updating employee, id is nullorEmpty");
        }
        
        try
        { 
            ItemResponse<Employee> response = await container.ReplaceItemAsync(employee, id, new PartitionKey(employee.personId.ToString()));
            return true;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error updating employee: {e.Message}");
            throw;
        }
    }

    public async Task<bool> UpdateCheckedInStatusAsync(Employee employee, bool checkedIn)
    {
        Container container = cosmosClient.GetContainer("bachelor13db", "Persons");

        try
        {
            Console.WriteLine(employee.personFirstName + employee.personLastName + " is being updated from " + employee.checkedIn + " to " + checkedIn);
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);

            string detectionTime = localTime.ToString("HH:mm dd/MM/yyyy");


            employee.checkedIn = checkedIn;
            employee.checkInTime = detectionTime;

            
            await container.ReplaceItemAsync(employee, employee.Id.ToString());
            return true;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            Console.WriteLine(employee + " Not found");
            throw new KeyNotFoundException($"Employee with ID '{employee.Id}' not found.");
            
        }
    }

    public async Task<bool> DeleteEmployeeAsync(string personId)
    {
        Container container = cosmosClient.GetContainer("bachelor13db", "Persons");
        var id = await GetDocIdByPersonIdAsync(personId);
        Console.WriteLine(personId);
        
        try
        {
            await container.DeleteItemAsync<Employee>(id,new PartitionKey(personId));
            return true;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error deleting employee: {e.Message}");
            throw;
        }
    }
    
    public async Task<List<Employee>> GetFilteredEmployeesAsync(string sortBy, string department, string role, Boolean checker)
    {
        
        var allEmployees = await GetCheckedOutEmployeesAsync();
        
        if (checker)
        {
            allEmployees = await GetCheckedInEmployeesAsync();
        }
        
        var filteredEmployees = allEmployees;
        
        switch (sortBy)
        {
            case "1": 
                filteredEmployees = filteredEmployees.OrderByDescending(e => e.checkInTime).ToList();
                break;
            case "2": 
                filteredEmployees = filteredEmployees.OrderBy(e => e.personLastName).ThenBy(e => e.personFirstName).ToList();
                break;
            case "default":
                break;
        }
        
        if (department != "Default")
        {
            filteredEmployees = filteredEmployees.Where(e => e.department == department).ToList();
        }
        
        if (role != "Default")
        {
            filteredEmployees = filteredEmployees.Where(e => e.role == role).ToList();
        }
        
        return filteredEmployees;
    }

    public async Task<string> GetDocIdByPersonIdAsync(string personId)
    {
        Container container = cosmosClient.GetContainer("bachelor13db", "Persons");
        try
        {
            var query = new QueryDefinition($"SELECT VALUE c.id FROM c WHERE c.personId = @personId")
                .WithParameter("@personId", personId);

            Console.WriteLine($"Executing query: {query} with personId: {personId}" );

            var iterator = container.GetItemQueryIterator<string>(query);
            var docId = await iterator.ReadNextAsync();
        
            var firstId = docId.FirstOrDefault();
            Console.WriteLine($"Document ID found: {firstId}");

            return firstId;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in GetDocIdByPersonIdAsync: {e}");
            throw;
        }
    }

}

