using PBAPI.DbHandler;
using NpgSqlExample.models;

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
app.MapPost("/contact", (Contact newContact) => dbHandler.AddContact(newContact));
app.MapGet("/contact", (string searchTerm) => dbHandler.GetContact(searchTerm));
app.MapDelete("/contact", (int id) => dbHandler.DeleteContact(id));
app.MapPut("/contact", (Contact updateContact) => dbHandler.UpdateContact(updateContact));


app.Run();
