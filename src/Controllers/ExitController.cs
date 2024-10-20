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
using Container = Microsoft.Azure.Cosmos.Container;
using bachelorOppgave13.Interfaces;

namespace bachelorOppgave13.Controllers;

public class ExitController : Controller
{
    
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IFaceRecognitionService _faceRecognitionService;
    private readonly IComputerVisionService _computerVisionService;

    public ExitController(IEmployeeRepository employeeRepository, IFaceRecognitionService faceRecognitionService, IComputerVisionService computerVisionService)
    {
        _employeeRepository = employeeRepository;
        _faceRecognitionService = faceRecognitionService;
        _computerVisionService = computerVisionService;
    }
    
    [HttpPost("/Exit/PerformFaceRecognitionAsync")]
    public async Task<RecognitionResult> PerformFaceRecognitionAsync([FromBody] ImageData imageData)
    {
        var analyzeTask = _computerVisionService.AnalyzeVideo(imageData);
        var recognitionTask = _faceRecognitionService.PerformFaceRecognitionAsync(imageData, false);

        
        await Task.WhenAll(analyzeTask, recognitionTask);
        
        if (await analyzeTask)
        {
            return await recognitionTask;
        }
        return new RecognitionResult { Message = "No person detected" };
    }
    
}