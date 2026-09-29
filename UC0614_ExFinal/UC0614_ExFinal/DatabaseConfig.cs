using System;

namespace UC0614_ExFinal
{
    public static class DatabaseConfig
    {
        public static string ConnectionString
        {
            get
            {
                string connectionString =
                    Environment.GetEnvironmentVariable("UC0614_DB_CONNECTION");

                if (String.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException(
                        "A variável de ambiente UC0614_DB_CONNECTION não está configurada.");
                }

                return connectionString;
            }
        }
    }
}