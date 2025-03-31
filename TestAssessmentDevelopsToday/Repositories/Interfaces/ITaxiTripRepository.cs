using TestAssessmentDevelopsToday.Data.Models;

namespace TestAssessmentDevelopsToday.Repositories.Interfaces
{
    public interface ITaxiTripRepository
    {
        Task BulkInsertAsync(List<TaxiTrip> trips);
    }
}
