using PBAPI.DbHandler;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var dbHandler = new DbHandler();
//Phonebook phonebook = new Phonebook();


app.MapGet("/contacts", () => dbHandler.GetPersons());
// app.MapPost("/contact", (string Name, int phone) => phonebook.AddPerson(Name, phone));
// app.MapGet("/contact", (string search) => phonebook.FindByName(search));


app.Run();
