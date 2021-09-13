using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.Common;

namespace DataProviderFactory
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("*** DataProvider ***");

            string dataProvider = ConfigurationManager.AppSettings["provider"];
            string connectionString = ConfigurationManager.ConnectionStrings["AutoLogSqlProvider"].ConnectionString;

            DbProviderFactory factory = DbProviderFactories.GetFactory(dataProvider);
            using (DbConnection connection = factory.CreateConnection())
            {
                if (connection == null)
                {
                    ShowError("Connection");

                    return;
                }

                Console.WriteLine($"Your connection object is {connection.GetType().Name}");
                connection.ConnectionString = connectionString;
                connection.Open();

                DbCommand command = factory.CreateCommand();
                if (command == null)
                {
                    ShowError("Command");

                    return;
                }

                command.Connection = connection;
                command.CommandText = "Select * From Inventory";

                Console.WriteLine($"Your command object is {command.GetType().Name}");

                using (DbDataReader dataReader = command.ExecuteReader())
                {
                    Console.WriteLine($"Your data reader object is {dataReader.GetType().Name}");
                    Console.WriteLine("\n*** Current Inventory ***");
                    
                    while (dataReader.Read())
                    {
                        Console.WriteLine($"-> Car #{dataReader["CarId"]} is a {dataReader["Make"]}.");
                    }
                }
            }
            
            Console.ReadLine();
        }

        private static void ShowError(string objectName)
        {
            Console.WriteLine($"There was an issue creating the {objectName}");
        }
    }
}
