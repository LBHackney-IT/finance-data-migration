namespace FinanceDataMigrationApi
{

    /// <summary>
    /// A POCO class for the configured API options.
    /// </summary>
    public class ApiOptions
    {

        /// <summary>
        /// Gets or sets the name of the API.
        /// </summary>
        public string ApiName { get; set; }

        /// <summary>
        /// Gets or sets the hackney API key.
        /// </summary>
        public string HackneyApiKey { get; set; }

    }

}
