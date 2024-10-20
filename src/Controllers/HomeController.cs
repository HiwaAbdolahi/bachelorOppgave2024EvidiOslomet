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


namespace bachelorOppgave13.Controllers;


public class HomeController : Controller
{
    
    private readonly IFaceRecognitionService _faceRecognitionService;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IComputerVisionService _computerVisionService;

    public HomeController(IEmployeeRepository employeeRepository, IFaceRecognitionService faceRecognitionService, IComputerVisionService computerVisionService)
    {
        _employeeRepository = employeeRepository;
        _faceRecognitionService = faceRecognitionService;
        _computerVisionService = computerVisionService;
    }
    
    [HttpPost ("/Home/PerformFaceRecognitionAsync")]
    public async Task<RecognitionResult> PerformFaceRecognitionAsync([FromBody] ImageData imageData)
    {
        var analyzeTask = _computerVisionService.AnalyzeVideo(imageData);
        var recognitionTask = _faceRecognitionService.PerformFaceRecognitionAsync(imageData, true);

        
        await Task.WhenAll(analyzeTask, recognitionTask);
        
        if (await analyzeTask)
        {
            return await recognitionTask;
        }
        return new RecognitionResult { Message = "No person detected" };
        
    }

    [HttpGet("/Home/SetupPersonGroup")]
    public async Task SetupPersonGroupFromCosmos()
    {
        await _faceRecognitionService.SetupPersonGroupFromCosmos();
    }
    
}