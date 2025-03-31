using CsvHelper;
using CsvHelper.Configuration;
using System.Data;
using System.Globalization;
using TestAssessmentDevelopsToday.Config;
using TestAssessmentDevelopsToday.Data.Models;
using TestAssessmentDevelopsToday.Infrastructure;
using TestAssessmentDevelopsToday.Repositories.Interfaces;
using TestAssessmentDevelopsToday.Services.Interfaces;
using TestAssessmentDevelopsToday.Utilities;

namespace TestAssessmentDevelopsToday.Services.Implementations
{
    public class TaxiTripCsvProcessor : ITaxiTripCsvProcessor
    {
        private readonly ITaxiTripRepository _taxiTripRepository;
        private readonly DatabaseInitializer _databaseInitializer;
        private readonly ApplicationConfig _applicationConfig;

        public TaxiTripCsvProcessor(ITaxiTripRepository taxiTripRepository, DatabaseInitializer databaseInitializer, ApplicationConfig applicationConfig)
        {
            _taxiTripRepository = taxiTripRepository;
            _databaseInitializer = databaseInitializer;
            _applicationConfig = applicationConfig;
        }

        public async Task ProcessAsync(string filePath)
        {
            await _databaseInitializer.InitializeDatabaseAsync();

            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                TrimOptions = TrimOptions.Trim
            });

            var duplicates = new HashSet<string>();
            var duplicateRecords = new List<TaxiTrip>();
            var validRecords = new List<TaxiTrip>();

            await csv.ReadAsync();
            csv.ReadHeader();

            while (await csv.ReadAsync())
            {
                try
                {
                    var trip = Map(csv);

                    string tripKey = $"{trip.PickupDatetime}-{trip.DropoffDatetime}-{trip.PassengerCount}";
                    if (!duplicates.Add(tripKey))
                    {
                        duplicateRecords.Add(trip);
                        continue;
                    }

                    if (IsValid(trip))
                    {
                        validRecords.Add(trip);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            await _taxiTripRepository.BulkInsertAsync(validRecords);
            await WriteDuplicatesToCsvAsync(duplicateRecords);
        }

        private async Task WriteDuplicatesToCsvAsync(List<TaxiTrip> duplicateRecords)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "CsvFiles", _applicationConfig.DuplicatesFileName);

            using var writer = new StreamWriter(filePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            await csv.WriteRecordsAsync(duplicateRecords);
        }

        private DataTable BuildTaxiTripDataTable()
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

            return dataTable;
        }

        private void AddToDataTable(DataTable dataTable, TaxiTrip trip)
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

        private TaxiTrip Map(CsvReader csv)
        {
            return new TaxiTrip()
            {
                PickupDatetime = DateTimeHelper.TryParseDate(csv, "tpep_pickup_datetime"),
                DropoffDatetime = DateTimeHelper.TryParseDate(csv, "tpep_dropoff_datetime"),
                PassengerCount = csv.TryGetField<int?>("passenger_count", out var pCount) ? pCount ?? 0 : 0,
                TripDistance = csv.TryGetField<float?>("trip_distance", out var tDistance) ? tDistance ?? 0 : 0,
                StoreAndFwdFlag = csv.GetField<string>("store_and_fwd_flag") == "Y" ? "Yes" : "No",
                PULocationID = csv.TryGetField<int?>("PULocationID", out var puLoc) ? puLoc ?? 0 : 0,
                DOLocationID = csv.TryGetField<int?>("DOLocationID", out var doLoc) ? doLoc ?? 0 : 0,
                FareAmount = csv.TryGetField<decimal?>("fare_amount", out var fare) ? fare ?? 0 : 0,
                TipAmount = csv.TryGetField<decimal?>("tip_amount", out var tip) ? tip ?? 0 : 0
            };
        }

        private bool IsValid(TaxiTrip trip)
        {
            return trip.PassengerCount >= 0 &&
                   trip.TripDistance >= 0 &&
                   trip.FareAmount >= 0 &&
                   trip.TipAmount >= 0 &&
                   trip.PULocationID > 0 &&
                   trip.DOLocationID > 0 &&
                   trip.DropoffDatetime >= trip.PickupDatetime;
        }

    }
}
