using bachelorOppgave13.Interfaces;
using bachelorOppgave13.Models;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using ApiKeyServiceClientCredentials = Microsoft.Azure.CognitiveServices.Vision.ComputerVision.ApiKeyServiceClientCredentials;


namespace bachelorOppgave13.Services;

public class ComputerVisionService : IComputerVisionService
{
    private readonly ComputerVisionClient _visionClient;
    private readonly string CV_Endpoint = "https://bachelor13vision.cognitiveservices.azure.com/";
    private readonly string CV_Key = "989f09241a7c45f7a6a70c6ca95d2097";

    public ComputerVisionService()
    {
        _visionClient = Authenticate(CV_Key, CV_Endpoint);
    }

    public async Task<bool> AnalyzeVideo(ImageData imageData)
    {
        try
        {
            
            Console.WriteLine("Computer Vision started");
            string imageDataUrl = imageData.ImageDataUrl;
            byte[] imageBytes = Convert.FromBase64String(imageDataUrl);
            using (MemoryStream stream = new MemoryStream(imageBytes))
            {
                var analysisResult = await _visionClient.AnalyzeImageInStreamAsync(stream, new List<VisualFeatureTypes?> { VisualFeatureTypes.Objects });

                foreach (var obj in analysisResult.Objects)
                {
                    if (obj.ObjectProperty == "person")
                    {
                        return true;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"(ComputerVision: Error analyzing image: {ex.Message}");
        }

        return false;
    }
    
    private ComputerVisionClient Authenticate(string subscriptionKey, string endpoint)
    {
        var credentials = new ApiKeyServiceClientCredentials(subscriptionKey);
        var client = new ComputerVisionClient(credentials)
        {
            Endpoint = endpoint
        };
        return client;
    }

}