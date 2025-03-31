namespace TestAssessmentDevelopsToday.Data.Models
{
    public class TaxiTrip
    {
        public DateTime? PickupDatetime { get; set; }
        public DateTime? DropoffDatetime { get; set; }
        public int PassengerCount { get; set; }
        public double TripDistance { get; set; }
        public string StoreAndFwdFlag { get; set; } = "No";
        public int PULocationID { get; set; }
        public int DOLocationID { get; set; }
        public decimal FareAmount { get; set; }
        public decimal TipAmount { get; set; }
    }
}
