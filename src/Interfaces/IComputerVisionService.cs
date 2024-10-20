using bachelorOppgave13.Models;

namespace bachelorOppgave13.Interfaces;

public interface IComputerVisionService
{
    Task<bool> AnalyzeVideo(ImageData imageDataUrl);
}