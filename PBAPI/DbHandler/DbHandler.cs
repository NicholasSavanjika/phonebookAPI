using Npgsql;
using NpgSqlExample.models;

namespace PBAPI.DbHandler
{
    public class DbHandler
    {
        // Connect to the database and login
        private const string CONNECTION_STRING = "Host=db.wifiandinternetsolutions.com.au;" +
            "Username=dtpphoneuser;" +
            "Password=kangan;" +
            "Database=dtpphonebook_staging";
        private NpgsqlConnection connection;

        public DbHandler() {
            this.connection = new NpgsqlConnection(CONNECTION_STRING);
            
            // start connection
            this.connection.OpenAsync(); 
        }
        // Get all contacts
        public async Task<List<Contact>> GetAllContacts() {


            // set up a query to get all contacts
            string query = "SELECT person_name.id, CONCAT(first_name , ' ' , last_name) AS name, contact_number FROM person_name, phone_book WHERE person_name.id=phone_book.id;";
            var contacts = new List<Contact>();

            // execute the query to get all contacts
            try {
                await using var dataSource = NpgsqlDataSource.Create(CONNECTION_STRING);
                await using var command = dataSource.CreateCommand(query);
                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    System.Console.WriteLine($"{reader[0]} {reader[1]} {reader[2]}");
                    contacts.Add(new Contact(reader[0].ToString(), reader[1].ToString(), reader[2].ToString()));
                }
            // prevent API from crashing if error occurs
            } catch (Exception e) {
                contacts = new List<Contact>();
                System.Console.WriteLine(e.Message);
            } finally {
                this.connection.CloseAsync();
                
                
            }
            return contacts;            
        }

        // Search for contacts in database
        public async Task<List<Contact>> GetContact(string searchTerm) {

            // set up a query to search for contacts
            string query2 = "SELECT * FROM (SELECT person_name.id, CONCAT(first_name , ' ' , last_name) AS name, contact_number FROM person_name, phone_book WHERE person_name.id=phone_book.id) AS nameOutput WHERE name ILIKE @p1;";
            searchTerm = $"%{searchTerm}%";
            var contact = new List<Contact>();
            
            // execute the query to search for contacts
            try {
                await using var dataSource = NpgsqlDataSource.Create(CONNECTION_STRING);
                await using var command = dataSource.CreateCommand(query2);
                command.Parameters.Add(new("p1", searchTerm));
                

                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    System.Console.WriteLine($"{reader[0]} {reader[1]} {reader[2]}");
                    contact.Add(new Contact(reader[0].ToString(), reader[1].ToString(), reader[2].ToString()));
                }
            // prevent API from crashing if error occurs    
            } catch (Exception e) {
                contact = new List<Contact>();
                System.Console.WriteLine(e.Message);
            } finally {
                this.connection.CloseAsync();
                
                
            }
            return contact;
        }

        // Add new contact
        public async Task<Contact?> AddContact(Contact newContact) {
            
            int contactNumberCheck;
            if (int.TryParse(newContact.ContactNumber, out contactNumberCheck) == false) {
                return null;
            }
            if (newContact.Name == "" || newContact.Name == null) {
                return null;
            }
            string firstName = null;
            string lastName = null;
            // check if contact has a last name
            if (newContact.Name.Contains(" ")) {

            
                // Get position of the space
                int index = newContact.Name.IndexOf(" ");
            
                // Get first name
                firstName =  newContact.Name.Substring(0, index);
            
                // Get last name
                lastName = newContact.Name.Substring(index + 1);
            }
            else {
                firstName = newContact.Name;
            }

            // set up a query to add a contact
            string query3 = "WITH add_person AS (INSERT INTO person_name (first_name, last_name) VALUES (@p1, @p2) RETURNING ID) INSERT INTO phone_book (id, contact_type, contact_number) SELECT id, 'Mobile', @p3 FROM add_person;";
            firstName = $"{firstName}";
            lastName = $"{lastName}";
            newContact.ContactNumber = $"{newContact.ContactNumber}";
            var addContact = new Contact();
            

            // execute the query to add a contact
            try {
                await using var dataSource = NpgsqlDataSource.Create(CONNECTION_STRING);
                await using var command = dataSource.CreateCommand(query3);
                command.Parameters.Add(new("p1", firstName));
                command.Parameters.Add(new("p2", lastName));
                command.Parameters.Add(new("p3", newContact.ContactNumber));

                var result = await command.ExecuteNonQueryAsync();
                System.Console.WriteLine(result);
                
                return newContact;
            // prevent API from crashing if error occurs  
            } catch (Exception e) {
                newContact = new Contact();
                System.Console.WriteLine(e.Message);
            } 
            
            return newContact;
        
        }
        // Delete existing contact
        public async Task<Contact?> DeleteContact(int id) {


            // set up a query to delete a contact
            string query4 = "WITH delete_person AS (DELETE FROM person_name WHERE person_name.id = @p1 RETURNING ID ) DELETE FROM phone_book WHERE phone_book.id IN (SELECT ID FROM delete_person)";

            var deleteContact = new Contact();
            

            // execute the query to delete a contact
            try {
                await using var dataSource = NpgsqlDataSource.Create(CONNECTION_STRING);
                await using var command = dataSource.CreateCommand(query4);
                command.Parameters.Add(new("p1", id));
                
                var result = await command.ExecuteNonQueryAsync();
                System.Console.WriteLine(result);

                if (result == 1) {
                    deleteContact.Id = "Deleted";
                }
                return deleteContact;

            // prevent API from crashing if error occurs  
            } catch (Exception e) {
                deleteContact = new Contact();
                System.Console.WriteLine(e.Message);
            }
            return deleteContact;
        
        }
        // Update existing contact
        public async Task<Contact?> UpdateContact(Contact updateContact) {
            
            //Check for id to update, and for name and contact number to update in database
            if(updateContact.Id == "" || updateContact.Id == null) {
                return null;
            }
            int contactNumberCheck;
            if (int.TryParse(updateContact.ContactNumber, out contactNumberCheck) == false) {
                return null;
            }
            if (updateContact.Name == "" || updateContact.Name == null) {
                return null;
            }
            int updateId;
            Int32.TryParse(updateContact.Id, out updateId);
            string updateFirstName = null;
            string updateLastName = null;
            // check if contact has a last name
            if (updateContact.Name.Contains(" ")) {

            
                // Get position of the space
                int index = updateContact.Name.IndexOf(" ");
            
                // Get first name
                updateFirstName =  updateContact.Name.Substring(0, index);
            
                // Get last name
                updateLastName = updateContact.Name.Substring(index + 1);
            }
            else {
                updateFirstName = updateContact.Name;
            }

            // set up a query to update a contact
            string query5 = "WITH upd1 AS (	UPDATE person_name SET first_name= @p1, last_name= @p2 WHERE id= @p4 returning id) UPDATE phone_book SET contact_number= @p3 FROM upd1 WHERE upd1.id=phone_book.id;";
            
            updateFirstName = $"{updateFirstName}";
            updateLastName = $"{updateLastName}";
            updateContact.ContactNumber = $"{updateContact.ContactNumber}";

            

            // execute the query to update a contact
            try {
                await using var dataSource = NpgsqlDataSource.Create(CONNECTION_STRING);
                await using var command = dataSource.CreateCommand(query5);
                command.Parameters.Add(new("p1", updateFirstName));
                command.Parameters.Add(new("p2", updateLastName));
                command.Parameters.Add(new("p3", updateContact.ContactNumber));
                command.Parameters.Add(new("p4", updateId));

                var result = await command.ExecuteNonQueryAsync();
                System.Console.WriteLine(result);
                
                return updateContact;
            // prevent API from crashing if error occurs  
            } catch (Exception e) {
                updateContact = new Contact();
                System.Console.WriteLine(e.Message);
            } 
            
            return updateContact;
        
        }

    }
}
