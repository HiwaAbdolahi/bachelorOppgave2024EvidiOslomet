using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using bachelorOppgave13.Models;
using bachelorOppgave13.Interfaces;
using Microsoft.Azure.CognitiveServices.Vision.Face;


namespace bachelorOppgave13.Services.BackgroundServices;
public class FaceBackgroundService : BackgroundService
{
    private readonly IFaceRecognitionService _faceRecognitionService;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILogger<FaceBackgroundService> _logger;

    public FaceBackgroundService(IFaceRecognitionService faceRecognitionService, IEmployeeRepository employeeRepository, ILogger<FaceBackgroundService> logger)
    {
        _faceRecognitionService = faceRecognitionService;
        _employeeRepository = employeeRepository;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Face recognition background service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            //Continuos face recognition goes here --> 

            // Simulate some processing
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }

        _logger.LogInformation("Face recognition background service is stopping.");
    }
}