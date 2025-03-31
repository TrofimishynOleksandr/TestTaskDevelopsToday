using Microsoft.Data.SqlClient;
using TestAssessmentDevelopsToday.Config;

namespace TestAssessmentDevelopsToday.Infrastructure
{
    public class DatabaseInitializer
    {
        private readonly ApplicationConfig _databaseConfig;

        public DatabaseInitializer(ApplicationConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }

        private static readonly string InitScript = @"
            IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'TaxiTripsDB')
            BEGIN
                CREATE DATABASE TaxiTripsDB;
            END;
            USE TaxiTripsDB;

            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TaxiTrips')
            BEGIN
                CREATE TABLE TaxiTrips (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    PickupDatetime DATETIME NOT NULL,
                    DropoffDatetime DATETIME NOT NULL,
                    PassengerCount INT CHECK (PassengerCount >= 0),
                    TripDistance DECIMAL(10,2) CHECK (TripDistance >= 0),
                    StoreAndFwdFlag NVARCHAR(3) CHECK (StoreAndFwdFlag IN ('Yes', 'No')),
                    PULocationID INT NOT NULL,
                    DOLocationID INT NOT NULL,
                    FareAmount DECIMAL(10,2) CHECK (FareAmount >= 0),
                    TipAmount DECIMAL(10,2) CHECK (TipAmount >= 0)
                );

                CREATE INDEX IX_TaxiTrips_PULocationID ON TaxiTrips(PULocationID);
                CREATE INDEX IX_TaxiTrips_TipAmount_PULocationID ON TaxiTrips(PULocationID, TipAmount);
                CREATE INDEX IX_TaxiTrips_TripDistance ON TaxiTrips(TripDistance DESC);
                CREATE INDEX IX_TaxiTrips_TravelTime ON TaxiTrips(DropoffDatetime, PickupDatetime);
            END;";

        public async Task InitializeDatabaseAsync()
        {
            try
            {
                using var connection = new SqlConnection(_databaseConfig.ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(InitScript, connection);
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization error: {ex.Message}");
            }
        }
    }
}
