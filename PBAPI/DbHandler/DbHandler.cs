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
        //Get all contacts
        public async Task<List<Contact>> GetAllContacts() {


            // set up a query
            string query = "SELECT person_name.id, CONCAT(first_name , ' ' , last_name) AS name, contact_number FROM person_name, phone_book WHERE person_name.id=phone_book.id;";
            var contacts = new List<Contact>();

            // execute the query
            try {
                if (this.connection.State == System.Data.ConnectionState.Closed) {
                    await this.connection.OpenAsync();
                }
                NpgsqlCommand command = new NpgsqlCommand(query, this.connection);

                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        System.Console.WriteLine($"{reader[0]} {reader[1]} {reader[2]}");
                        contacts.Add(new Contact(reader[0].ToString(), reader[1].ToString(), reader[2].ToString()));
                    }
                }

                
            } catch (Exception e) {
                contacts = new List<Contact>();
                System.Console.WriteLine(e.Message);
            } finally {
                this.connection.CloseAsync();
                
                
            }
            return contacts;            
        }
        //Get one contact (or many contacts if I coded it that way)
        public async Task<List<Contact>> GetContact(string searchTerm) {

            
            string query2 = "SELECT person_name.id, CONCAT(first_name, ' ', last_name) AS NAME, contact_number FROM person_name, phone_book WHERE person_name.id=phone_book.id AND (first_name LIKE @p1 OR last_name LIKE @p1);";
            searchTerm = $"%{searchTerm}%";
            var contact = new List<Contact>();
            // set up a query
            /*NpgsqlCommand command = new NpgsqlCommand(query2, this.connection)
            {
                Parameters =
                {
                    new("p1", searchTerm)
                }
            };
            

            // execute the query


            using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    System.Console.WriteLine($"{reader[0]} {reader[1]} {reader[2]}");
                    contact.Add(new Contact(reader[0].ToString(), reader[1].ToString(), reader[2].ToString()));
                }
            }

            return contact;*/
            
            try {
                if (this.connection.State == System.Data.ConnectionState.Closed) {
                    await this.connection.OpenAsync();
                }
                NpgsqlCommand command = new NpgsqlCommand(query2, this.connection)
                {
                    Parameters =
                    {
                        new("p1", searchTerm)
                    }
                };
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        System.Console.WriteLine($"{reader[0]} {reader[1]} {reader[2]}");
                        contact.Add(new Contact(reader[0].ToString(), reader[1].ToString(), reader[2].ToString()));
                    }
                }

                
            } catch (Exception e) {
                contact = new List<Contact>();
                System.Console.WriteLine(e.Message);
            } finally {
                this.connection.CloseAsync();
                
                
            }
            return contact;
        }
        //Add new contact
        /*public async Task Add(Contact contact)AddContact() {


            // set up a query
            NpgsqlCommand command = new NpgsqlCommand("SELECT person_name.id, CONCAT(first_name , ' ' , last_name) AS name, contact_number FROM person_name, phone_book WHERE person_name.id=phone_book.id;", this.connection);
            {
                Parameters =
                {
                    new("p1", "name"),
                    new("p2", "contactNumber")
                }
            };
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
        }*/
    }
}
