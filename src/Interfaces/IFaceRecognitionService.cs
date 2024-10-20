using bachelorOppgave13.Models;
namespace bachelorOppgave13.Interfaces;

public interface IFaceRecognitionService 
{
    Task<RecognitionResult> PerformFaceRecognitionAsync(ImageData imageDataUrl, Boolean option);
    Task SetupPersonGroupFromCosmos();

}