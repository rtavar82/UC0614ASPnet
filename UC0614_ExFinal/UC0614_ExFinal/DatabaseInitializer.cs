using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace UC0614_ExFinal
{
    public static class DatabaseInitializer
    {
        public static void Initialize()
        {
            string connectionString = DatabaseConfig.ConnectionString;

            SqlConnectionStringBuilder builder =
                new SqlConnectionStringBuilder(connectionString);

            string nomeBaseDados = builder.InitialCatalog;

            if (String.IsNullOrWhiteSpace(nomeBaseDados))
            {
                throw new InvalidOperationException(
                    "A connection string deve indicar o nome da base de dados em Initial Catalog.");
            }

            // -------------------------------------------------
            // 1. Ligar à master para verificar/criar a BD
            // -------------------------------------------------

            SqlConnectionStringBuilder masterBuilder =
                new SqlConnectionStringBuilder(connectionString);

            masterBuilder.InitialCatalog = "master";

            using (SqlConnection ligacao =
                new SqlConnection(masterBuilder.ConnectionString))
            {
                ligacao.Open();

                const string sql = @"
IF DB_ID(@nomeBaseDados) IS NULL
BEGIN
    DECLARE @comando NVARCHAR(MAX);

    SET @comando =
        N'CREATE DATABASE ' + QUOTENAME(@nomeBaseDados);

    EXEC(@comando);
END";

                using (SqlCommand comando =
                    new SqlCommand(sql, ligacao))
                {
                    comando.Parameters.Add(
                        "@nomeBaseDados",
                        SqlDbType.NVarChar,
                        128).Value = nomeBaseDados;

                    comando.ExecuteNonQuery();
                }
            }

            // -------------------------------------------------
            // 2. Verificar se a estrutura já existe
            // -------------------------------------------------

            bool estruturaExiste = false;

            using (SqlConnection ligacao =
                new SqlConnection(connectionString))
            {
                ligacao.Open();

                using (SqlCommand comando =
                    new SqlCommand(
                        @"SELECT COUNT(*)
                          FROM sys.tables
                          WHERE name = 'Perfis';",
                        ligacao))
                {
                    estruturaExiste =
                        Convert.ToInt32(comando.ExecuteScalar()) > 0;
                }
            }

            // -------------------------------------------------
            // 3. Se não existir estrutura, executar o script
            // -------------------------------------------------

            if (!estruturaExiste)
            {
                string caminhoScript =
                    Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "Database",
                        "UC0614_ExFinal.sql");

                if (!File.Exists(caminhoScript))
                {
                    throw new FileNotFoundException(
                        "Não foi encontrado o script da base de dados.",
                        caminhoScript);
                }

                string script =
                    File.ReadAllText(caminhoScript);

                string[] blocos = Regex.Split(
                    script,
                    @"^\s*GO\s*;?\s*$",
                    RegexOptions.Multiline |
                    RegexOptions.IgnoreCase);

                using (SqlConnection ligacao =
                    new SqlConnection(connectionString))
                {
                    ligacao.Open();

                    foreach (string bloco in blocos)
                    {
                        if (String.IsNullOrWhiteSpace(bloco))
                        {
                            continue;
                        }

                        using (SqlCommand comando =
                            new SqlCommand(bloco, ligacao))
                        {
                            comando.CommandTimeout = 120;
                            comando.ExecuteNonQuery();
                        }
                    }
                }
            }

            // -------------------------------------------------
            // 4. Criar administrador inicial, se necessário
            // -------------------------------------------------

            CriarAdministradorInicialSeNecessario();
        }


        private static void CriarAdministradorInicialSeNecessario()
        {
            string nome =
                Environment.GetEnvironmentVariable(
                    "UC0614_ADMIN_NAME");

            string email =
                Environment.GetEnvironmentVariable(
                    "UC0614_ADMIN_EMAIL");

            string password =
                Environment.GetEnvironmentVariable(
                    "UC0614_ADMIN_PASSWORD");

            using (SqlConnection ligacao =
                new SqlConnection(DatabaseConfig.ConnectionString))
            {
                ligacao.Open();

                // Verificar se já existe administrador
                using (SqlCommand verificar =
                    new SqlCommand(
                        @"SELECT COUNT(*)
                          FROM Utilizadores
                          WHERE PerfilId = 1;",
                        ligacao))
                {
                    int quantidade =
                        Convert.ToInt32(
                            verificar.ExecuteScalar());

                    if (quantidade > 0)
                    {
                        return;
                    }
                }

                // Validar variáveis
                if (String.IsNullOrWhiteSpace(nome) ||
                    String.IsNullOrWhiteSpace(email) ||
                    String.IsNullOrWhiteSpace(password))
                {
                    throw new InvalidOperationException(
                        "Não existe nenhum administrador. " +
                        "Configure UC0614_ADMIN_NAME, " +
                        "UC0614_ADMIN_EMAIL e " +
                        "UC0614_ADMIN_PASSWORD.");
                }

                if (password.Length < 8)
                {
                    throw new InvalidOperationException(
                        "UC0614_ADMIN_PASSWORD deve ter pelo menos 8 caracteres.");
                }

                // Verificar se o email já existe
                using (SqlCommand verificarEmail =
                    new SqlCommand(
                        @"SELECT COUNT(*)
                          FROM Utilizadores
                          WHERE Email = @email;",
                        ligacao))
                {
                    verificarEmail.Parameters.AddWithValue(
                        "@email",
                        email);

                    int existeEmail =
                        Convert.ToInt32(
                            verificarEmail.ExecuteScalar());

                    if (existeEmail > 0)
                    {
                        throw new InvalidOperationException(
                            "O email definido em UC0614_ADMIN_EMAIL " +
                            "já pertence a outra conta.");
                    }
                }

                string passwordHash =
                    PasswordSecurity.CreateHash(password);

                // Criar administrador
                using (SqlCommand inserir =
                    new SqlCommand(
                        @"INSERT INTO Utilizadores
                          (
                              Nome,
                              Email,
                              PasswordHash,
                              PerfilId,
                              Ativo
                          )
                          VALUES
                          (
                              @nome,
                              @email,
                              @passwordHash,
                              1,
                              1
                          );",
                        ligacao))
                {
                    inserir.Parameters.AddWithValue(
                        "@nome",
                        nome);

                    inserir.Parameters.AddWithValue(
                        "@email",
                        email);

                    inserir.Parameters.AddWithValue(
                        "@passwordHash",
                        passwordHash);

                    inserir.ExecuteNonQuery();
                }
            }
        }
    }
}