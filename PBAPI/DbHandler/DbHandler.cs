using Npgsql;
using NpgSqlExample.models;

namespace PBAPI.DbHandler
{
    public class DbHandler
    {
        private const string CONNECTION_STRING = "Host=db.wifiandinternetsolutions.com.au;" +
            "Username=dtpphoneuser;" +
            "Password=kangan;" +
            "Database=dtpphonebook";
        private NpgsqlConnection connection;

        public DbHandler() {
            this.connection = new NpgsqlConnection(CONNECTION_STRING);
            
            // start connection
            this.connection.OpenAsync(); 
        }
        
        public async Task<List<Contact>> GetAllContacts() {


            // set up a query
            NpgsqlCommand command = new NpgsqlCommand("SELECT person_name.id, CONCAT(first_name , ' ' , last_name) AS name, contact_number FROM person_name, phone_book WHERE person_name.id=phone_book.id;", this.connection);
            var contacts = new List<Contact>();

            // execute the query


            using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    System.Console.WriteLine($"{reader[0]} {reader[1]} {reader[2]}");
                    contacts.Add(new Contact(reader[0].ToString(), reader[1].ToString(), reader[2].ToString()));
                }
            }

            return contacts;
        }
    }
}
