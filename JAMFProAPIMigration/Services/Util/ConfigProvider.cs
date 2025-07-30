namespace JAMFProAPIMigration.Services.Util
{
    public record ConfigProvider
    {
        private static readonly string JAMF_URL = Environment.GetEnvironmentVariable("JAMF_URL");
        private static readonly string CLIENT_ID = Environment.GetEnvironmentVariable("JAMF_CLIENT_ID");
        private static readonly string CLIENT_SECRET = Environment.GetEnvironmentVariable("JAMF_CLIENT_SECRET");

        public static string GetJAMFURL() => JAMF_URL;
        public static string GetCLIENT_ID() => CLIENT_ID;
        public static string GetCLIENT_SECRET() => CLIENT_SECRET;
    }
}
