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

            // set up a query
            string query2 = "SELECT person_name.id, CONCAT(first_name, ' ', last_name) AS NAME, contact_number FROM person_name, phone_book WHERE person_name.id=phone_book.id AND (first_name LIKE @p1 OR last_name LIKE @p1);"; // "OR NAME LIKE @p1" needs to be added
            searchTerm = $"%{searchTerm}%";
            var contact = new List<Contact>();
            
            // execute the query
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
        public async Task<Contact> AddContact(string addName, string addNumber) {

            // Location of the space
            int index = addName.IndexOf(" ");
            
            // Get first name
            string firstName =  addName.Substring(0, index);
            
            // Get last name
            string lastName = addName.Substring(index + 1);

            // set up a query
            string query3 = "WITH add_person AS (INSERT INTO person_name (first_name, last_name) VALUES (@p1, @p2) RETURNING ID) INSERT INTO phone_book (id, contact_type, contact_number) SELECT id, 'Mobile', @p3 FROM add_person;";
            firstName = $"{firstName}";
            lastName = $"{lastName}";
            addNumber = $"{addNumber}";
            var addContact = new Contact();
            

            // execute the query
            try {
                if (this.connection.State == System.Data.ConnectionState.Closed) {
                    await this.connection.OpenAsync();
                }
                NpgsqlCommand command = new NpgsqlCommand(query3, this.connection)
                {
                    Parameters =
                    {
                        new("p1", firstName),
                        new("p2", lastName),
                        new("p3", addNumber)
                    }
                };

                var result = command.ExecuteNonQuery();

                addContact.Name = firstName;
                return addContact;
            } catch (Exception e) {
                addContact = new Contact();
                System.Console.WriteLine(e.Message);
            } finally {
                this.connection.CloseAsync();
                
                
            }
            return addContact;
        
        }
        //Delete existing contact
        /*public async Task<List<Contact>> DeleteContact(string addName, string addNumber) {


            // set up a query
            string query3 = "WITH add_person AS (INSERT INTO person_name (first_name, last_name) VALUES (@p1) RETURNING ID) INSERT INTO phone_book (id, contact_type, contact_number) SELECT id, 'Mobile', @p2 FROM add_person;";
            addName = $"%{addName}%";
            addNumber = $"%{addNumber}%";
            var addContact = new List<Contact>();
            

            // execute the query
            try {
                if (this.connection.State == System.Data.ConnectionState.Closed) {
                    await this.connection.OpenAsync();
                }
                NpgsqlCommand command = new NpgsqlCommand(query3, this.connection)
                {
                    Parameters =
                    {
                        new("p1", addName),
                        new("p2", addNumber)
                    }
                };
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        System.Console.WriteLine($"{reader[0]} {reader[1]} {reader[2]}");
                        addContact.Add(new Contact(reader[0].ToString(), reader[1].ToString(), reader[2].ToString()));
                    }
                }

                
            } catch (Exception e) {
                addContact = new List<Contact>();
                System.Console.WriteLine(e.Message);
            } finally {
                this.connection.CloseAsync();
                
                
            }
            return addContact;
        
        }*/
    }
}
