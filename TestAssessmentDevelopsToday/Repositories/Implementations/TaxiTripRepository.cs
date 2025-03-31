using Microsoft.Data.SqlClient;
using System.Data;
using TestAssessmentDevelopsToday.Config;
using TestAssessmentDevelopsToday.Data.Models;
using TestAssessmentDevelopsToday.Repositories.Interfaces;

namespace TestAssessmentDevelopsToday.Repositories.Implementations
{
    public class TaxiTripRepository : ITaxiTripRepository
    {
        private readonly ApplicationConfig _databaseConfig;

        public TaxiTripRepository(ApplicationConfig databaseConfig) 
        {
            _databaseConfig = databaseConfig;
        }

        public async Task BulkInsertAsync(List<TaxiTrip> trips)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("PickupDatetime", typeof(DateTime));
            dataTable.Columns.Add("DropoffDatetime", typeof(DateTime));
            dataTable.Columns.Add("PassengerCount", typeof(int));
            dataTable.Columns.Add("TripDistance", typeof(double));
            dataTable.Columns.Add("StoreAndFwdFlag", typeof(string));
            dataTable.Columns.Add("PULocationID", typeof(int));
            dataTable.Columns.Add("DOLocationID", typeof(int));
            dataTable.Columns.Add("FareAmount", typeof(decimal));
            dataTable.Columns.Add("TipAmount", typeof(decimal));

            foreach (var trip in trips)
            {
                dataTable.Rows.Add(
                    trip.PickupDatetime,
                    trip.DropoffDatetime,
                    trip.PassengerCount,
                    trip.TripDistance,
                    trip.StoreAndFwdFlag,
                    trip.PULocationID,
                    trip.DOLocationID,
                    trip.FareAmount,
                    trip.TipAmount
                );
            }

            using var connection = new SqlConnection(_databaseConfig.ConnectionString);
            await connection.OpenAsync();

            using var bulkCopy = new SqlBulkCopy(connection);
            bulkCopy.DestinationTableName = "TaxiTrips";

            foreach (DataColumn column in dataTable.Columns)
            {
                bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
            }

            await bulkCopy.WriteToServerAsync(dataTable);
        }
    }
}
