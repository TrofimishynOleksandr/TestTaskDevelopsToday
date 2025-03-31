namespace TestAssessmentDevelopsToday.Config
{
    public class ApplicationConfig
    {
        public string ConnectionString { get; }
        public string DuplicatesFileName { get; }

        public ApplicationConfig(string connectionString, string duplicatesFileName)
        {
            ConnectionString = connectionString;
            DuplicatesFileName = duplicatesFileName;
        }
    }
}
