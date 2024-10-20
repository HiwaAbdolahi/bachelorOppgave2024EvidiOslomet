namespace bachelorOppgave13.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.Azure.Cosmos;
using Azure.Storage.Blobs;
using bachelorOppgave13.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using bachelorOppgave13.Interfaces;
using Container = Microsoft.Azure.Cosmos.Container;

public class FaceRecognitionService : IFaceRecognitionService
{
    
    private const string SubscriptionKey = "6920e2f5fcfb45dd897f754d60fb7bc8";
    private const string Endpoint = "https://bachelor13face.cognitiveservices.azure.com/";
    private string GroupID = "my-person-group20123948574834";
    private const string CosmosEndpointUri = "https://bachelor13db.documents.azure.com:443/";
    private const string CosmosPrimaryKey = "uYcNfxn2ZJwBCtgvJ3wDvf2njCzlSq7NWKTQzLw3kZysdj6aKJpjWRFdmiPs5pEmmAQlZQ7tkB0AACDbQGBrWw==";
    private const string BlobConnectionString = "DefaultEndpointsProtocol=https;AccountName=bachelor13blob;AccountKey=fM/AMbvzOAmVNbikgHQyAFFO4t3CUPV2Noet+eQL+3ciWoHPakdzBqYuGQXlj2bsY7860z1fJjFp+AStijOl4g==;EndpointSuffix=core.windows.net";

    private readonly IEmployeeRepository _employeeRepository;

    

    public FaceRecognitionService( IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }
    
    [HttpPost]
    public async Task<RecognitionResult> PerformFaceRecognitionAsync(ImageData imageData, Boolean option)
    {
        string imageDataUrl = imageData.ImageDataUrl;
        try
        {
            if (string.IsNullOrEmpty(imageDataUrl))
            {
                throw new ArgumentNullException(nameof(imageDataUrl), "No image data received.");
            }

            byte[] imageBytes = Convert.FromBase64String(imageDataUrl);
            IFaceClient faceClient = Authenticate(Endpoint, SubscriptionKey);

            IList<DetectedFace> detectedFaces = await faceClient.Face.DetectWithStreamAsync(new MemoryStream(imageBytes));

            if (detectedFaces.Count == 0)
            {
                return new RecognitionResult { Message = "No face detected." };
            }

            foreach (DetectedFace detectedFace in detectedFaces)
            {
                IList<Guid> faceIds = new List<Guid> { detectedFace.FaceId.Value };
                var identifyResults = await faceClient.Face.IdentifyAsync(faceIds, GroupID);

                if (identifyResults != null && identifyResults.Any())
                {
                    foreach (var identifyResult in identifyResults)
                    {
                        if (identifyResult.Candidates.Count > 0)
                        {
                            Guid personId = identifyResult.Candidates[0].PersonId;
                            Person person = await faceClient.PersonGroupPerson.GetAsync(GroupID, personId);
                            string personName = person.Name;
                            string[] nameParts = personName.Split(' ');
                            string firstName = nameParts[0];
                            string lastName = nameParts[1];
                            string detectionTime = DateTime.Now.ToString("HH:mm");
                            
                            var employee = await GetEmployeeByNameAsync(firstName, lastName);
                            await UpdateCheckedInStatusAsync(employee, option);

                            return new RecognitionResult { Message = $"{personName}, clocked in at {detectionTime}" };
                        }
                    }
                }
            }

            return new RecognitionResult { Message = "No match found in any person group." };
        }
        catch (Exception ex)
        {
            return new RecognitionResult { ErrorMessage = $"Error processing image: {ex.Message}" };
        }
    }
   
    [HttpGet]
    public async Task<Employee> GetEmployeeByNameAsync(string firstName, string lastName)
    {
        var employee = await _employeeRepository.GetEmployeeByNameAsync(firstName, lastName);
        return employee;
    }

    [HttpPut]
    public async Task UpdateCheckedInStatusAsync(Employee employee, bool checkedIn)
    {
        await _employeeRepository.UpdateCheckedInStatusAsync(employee, checkedIn);     
    }

    private static IFaceClient Authenticate(string endpoint, string key)
    {
        return new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
    }
    
    public async Task SetupPersonGroupFromCosmos()
    {
        try
        {
            Console.WriteLine("Setup started");
            CosmosClient cosmosClient = new CosmosClient(CosmosEndpointUri, CosmosPrimaryKey);
            Database cosmosDatabase = cosmosClient.GetDatabase("bachelor13db");
            Container cosmosContainer = cosmosDatabase.GetContainer("Persons");
            BlobServiceClient blobServiceClient = new BlobServiceClient(BlobConnectionString);
            IFaceClient faceClient = Authenticate(Endpoint, SubscriptionKey);
            await faceClient.PersonGroup.DeleteAsync(GroupID);
            await faceClient.PersonGroup.CreateAsync(GroupID, "Persons");

            QueryDefinition queryDefinition = new QueryDefinition("SELECT * FROM c");
            FeedIterator<dynamic> queryResultSetIterator = cosmosContainer.GetItemQueryIterator<dynamic>(queryDefinition);
            List<dynamic> persons = new List<dynamic>();
            
            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<dynamic> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                persons.AddRange(currentResultSet);
            }
            
            foreach (var person in persons)
            {
                string personName = $"{person["personFirstName"]} {person["personLastName"]}";
                
                Guid personId = await CreatePersonAsync(faceClient, GroupID, personName);
                
                var imageUrls = ((IEnumerable<dynamic>)person.imageUrl).Select(x => x.ToString()).ToList();
                List<string> stringList = imageUrls.Select(x => (string)x).ToList();
                
                await AddFacesToPersonAsync(faceClient, GroupID, personId, stringList, blobServiceClient);
            }
            
            await faceClient.PersonGroup.TrainAsync(GroupID);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting up person group from Cosmos DB: {ex.Message}");
        }
    }
    
    private async Task AddFacesToPersonAsync(IFaceClient faceClient, string personGroupId, Guid personId, List<string> imageUrls, BlobServiceClient blobServiceClient)
    {
        foreach (var imageUrl in imageUrls)
        {
            try
            {
                Uri uri = new Uri(imageUrl);
                string containerName = uri.Segments[1].Trim('/');
                string blobName = string.Join("", uri.Segments[2..]);

                Console.WriteLine($"Container Name: {containerName}, Blob Name: {blobName}");


                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                BlobClient blobClient = containerClient.GetBlobClient(blobName);
                
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await blobClient.DownloadToAsync(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    await faceClient.PersonGroupPerson.AddFaceFromStreamAsync(personGroupId, personId, memoryStream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding face from URL {imageUrl}: {ex.Message}");

            }
        }
    }
    private async Task<Guid> CreatePersonAsync(IFaceClient faceClient, string personGroupId, string personName)
    {
        var person = await faceClient.PersonGroupPerson.CreateAsync(personGroupId, personName);
        return person.PersonId;
    }
    
}