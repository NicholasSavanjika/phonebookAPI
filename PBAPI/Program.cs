using PBLIB;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

Phonebook phonebook = new Phonebook();


app.MapGet("/GetAll", () => phonebook.GetAll());
app.MapGet("/Add", (string Name, int phone) => phonebook.AddPerson(Name, phone));
app.MapGet("FindByName", (string search) => phonebook.FindByName(search));


app.Run();
