using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.SqlServer.Management.AlwaysEncrypted.AzureKeyVaultProvider;

#pragma warning disable 618

namespace _09_SQL_always_encrypted_with_key_vault
{
    class Program
    {
        static string connectionString = @"<We will read the connectionString from key vault>";

        static async Task Main(string[] args)
        {
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = GetKeyVaultClient(azureServiceTokenProvider);
            InitializeAzureKeyVaultProvider(azureServiceTokenProvider);

            var secret = await keyVaultClient.GetSecretAsync($"https://nosecrets-vault02.vault.azure.net/secrets/DBConnectionString/");
            connectionString = secret.Value;
            var connStringBuilder = new SqlConnectionStringBuilder(connectionString)
            {
                ColumnEncryptionSetting = SqlConnectionColumnEncryptionSetting.Enabled
            };

            connectionString = connStringBuilder.ConnectionString;

            ResetPersonsTable();
            InsertData();
            Console.WriteLine("-------- Select all -------------");
            foreach (var person in SelectAllPersons())
            {
                Console.WriteLine($"{person.Name} CreditCard: {person.CreditCard} Amount {person.Amount}");
            }

            Console.WriteLine("-------- Query by CreditCard -------------");
            var creditCardHolger = "999-99-0005";
            var selectedPerson = SelectPersonByCreditCard(creditCardHolger);

            Console.WriteLine($"{selectedPerson.Name} CreditCard: {selectedPerson.CreditCard} Amount: {selectedPerson.Amount}");


            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }

        static void ResetPersonsTable()
        {
            var sqlCmd = new SqlCommand("DELETE FROM Persons");
            using (sqlCmd.Connection = new SqlConnection(connectionString))
            {
                sqlCmd.Connection.Open();
                sqlCmd.ExecuteNonQuery();
            }
        }
        private static void InsertData()
        {
            InsertPerson(new Person()
            {
                CreditCard = "999-99-0001",
                Name = "Orlando Gee",
                Amount = 1430
            });
            InsertPerson(new Person()
            {
                CreditCard = "999-99-0002",
                Name = "Keith Harris",
                Amount = 6512847
            });
            InsertPerson(new Person()
            {
                CreditCard = "999-99-0003",
                Name = "Donna Carreras",
                Amount = 97674
            });
            InsertPerson(new Person()
            {
                CreditCard = "999-99-0004",
                Name = "Lars Kaufmann",
                Amount = 76843
            });
            InsertPerson(new Person()
            {
                CreditCard = "999-99-0005",
                Name = "Holger Bönisch",
                Amount = 876343
            });
        }
        static void InsertPerson(Person newPerson)
        {

            var sqlCmdText = @"INSERT INTO [dbo].[Persons] ([CreditCard], [Name], [Amount] )  VALUES (@cn, @name, @amount);";

            var sqlCmd = new SqlCommand(sqlCmdText);


            sqlCmd.Parameters.Add("@cn", DbType.String).Value = newPerson.CreditCard;
            sqlCmd.Parameters.Add("@name", DbType.String).Value = newPerson.Name;
            sqlCmd.Parameters.Add("@amount", DbType.Int32).Value = newPerson.Amount;

            using (sqlCmd.Connection = new SqlConnection(connectionString))
            {
                sqlCmd.Connection.Open();
                sqlCmd.ExecuteNonQuery();
            }
        }
        static List<Person> SelectAllPersons()
        {
            var persons = new List<Person>();
            var sqlCmd = new SqlCommand("SELECT [CreditCard], [Name], [Amount] FROM [dbo].[Persons]", new SqlConnection(connectionString));

            using (sqlCmd.Connection = new SqlConnection(connectionString))
            {
                using (sqlCmd.Connection = new SqlConnection(connectionString))
                {
                    sqlCmd.Connection.Open();
                    var reader = sqlCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        persons.Add(new Person()
                        {
                            CreditCard = reader[0].ToString(),
                            Name = reader[1].ToString(),
                            Amount = reader.GetInt32(2)
                        });
                    }
                }
            }

            return persons;
        }
        static Person SelectPersonByCreditCard(string creditCard)
        {
            var person = new Person();

            var sqlCmd = new SqlCommand(
                "SELECT [CreditCard], [Name], [Amount] FROM [dbo].[Persons] WHERE [CreditCard]=@cn",
                new SqlConnection(connectionString));

            sqlCmd.Parameters.Add("@cn", DbType.String).Value = creditCard;


            using (sqlCmd.Connection = new SqlConnection(connectionString))
            {
                sqlCmd.Connection.Open();
                var reader = sqlCmd.ExecuteReader();

                while (reader.Read())
                {
                    return new Person()
                    {
                        CreditCard = reader[0].ToString(),
                        Name = reader[1].ToString(),
                        Amount = reader.GetInt32(2)
                    };
                }

                return person;
            }
        }

        
        private static KeyVaultClient GetKeyVaultClient(AzureServiceTokenProvider azureServiceTokenProvider)
        {
            var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            return keyVaultClient;
        }

        static void InitializeAzureKeyVaultProvider(AzureServiceTokenProvider azureServiceTokenProvider)
        {
            var azureKeyVaultProvider = new SqlColumnEncryptionAzureKeyVaultProvider(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

            var providers = new Dictionary<string, SqlColumnEncryptionKeyStoreProvider>
            {
                {SqlColumnEncryptionAzureKeyVaultProvider.ProviderName, azureKeyVaultProvider}
            };

            SqlConnection.RegisterColumnEncryptionKeyStoreProviders(providers);
        }
    }

    class Person
    {
        public string CreditCard { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; }
    }
}
