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

        public List<Contact> GetAllContacts() {
            // start connection
            this.connection.Open();

            // set up a query
            NpgsqlCommand command = new NpgsqlCommand("SELECT person_name.id, CONCAT(first_name , ' ' , last_name) AS name, contact_number FROM person_name, phone_book WHERE person_name.id=phone_book.id;", this.connection);

            // execute the query
            NpgsqlDataReader reader = command.ExecuteReader();
            var contacts = new List<Contact>();

            while(reader.Read()) {
                System.Console.WriteLine($"{reader[0]} {reader[1]} {reader[2]}");
                contacts.Add(new Contact(reader[0].ToString(), reader[1].ToString(), reader[2].ToString()));
            }

            return contacts;
        }
    }
}