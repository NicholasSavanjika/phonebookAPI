using Npgsql;
using NpgSqlExample.models;

namespace PBAPI.DbHandler
{
    public class DbHandler
    {
        private const string CONNECTION_STRING = "Host=db.wifiandinternetsolutions.com.au;" +
            "Username=elsa;" +
            "Password=kangan;" +
            "Database=elsa_phonebook";
        private NpgsqlConnection connection = new NpgsqlConnection(CONNECTION_STRING);

        public List<Person> GetPersons() {
            // start connection
            this.connection.Open();

            // set up a query
            NpgsqlCommand command = new NpgsqlCommand("Select * from person_name;", this.connection);

            // execute the query
            NpgsqlDataReader reader = command.ExecuteReader();
            var persons = new List<Person>();

            while(reader.Read()) {
                System.Console.WriteLine($"{reader[0]} {reader[1]} {reader[2]}");
                persons.Add(new Person(reader[0].ToString(), reader[1].ToString(), reader[2].ToString()));
            }

            return persons;
        }
    }
}