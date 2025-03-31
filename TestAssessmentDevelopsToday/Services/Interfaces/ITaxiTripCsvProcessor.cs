namespace TestAssessmentDevelopsToday.Services.Interfaces
{
    public interface ITaxiTripCsvProcessor
    {
        Task ProcessAsync(string filePath);
    }
}
