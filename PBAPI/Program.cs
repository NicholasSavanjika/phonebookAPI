using PBAPI.DbHandler;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

var app = builder.Build();

app.UseCors(b =>{
    b.AllowAnyOrigin();
    b.AllowAnyMethod();
    b.AllowAnyHeader();
});

var dbHandler = new DbHandler();
//Phonebook phonebook = new Phonebook();


app.MapGet("/contacts", () => dbHandler.GetAllContacts());
// app.MapPost("/contact", (string Name, int phone) => phonebook.AddPerson(Name, phone));
app.MapGet("/contact", (string searchTerm) => dbHandler.GetContact(searchTerm));


app.Run();
