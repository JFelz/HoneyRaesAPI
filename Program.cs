using HoneyRaesAPI.Models;

var builder = WebApplication.CreateBuilder(args);

List<Customer> customers = new List<Customer>()
{
    new Customer()
    {
        Id = 1,
        Name = "Robert Mahr",
        Address = "8758 Sussex St.\r\nMerrillville, IN 46410"
    },
    new Customer()
    {
        Id = 2,
        Name = "Ashley Cohn",
        Address = "619 Baker Rd.\r\nFuquay Varina, NC 27526"
    },
    new Customer()
    {
        Id = 3,
        Name =  "Isabella Rodriguez",
        Address = "569 Kirkland St.\r\nMemphis, TN 38106"

    }
};

List<Employee> employees = new List<Employee>()
{
    new Employee()
    {
        Id = 1,
        Name = "Kimberley Boyd",
        Specialty = "Debugging"

    },
    new Employee()
    {
        Id = 2,
        Name = "Desmond Beau",
        Specialty = "C#"
    },
};

List<ServiceTicket> serviceTickets = new List<ServiceTicket>()
{
    new ServiceTicket()
    {
        Id = 1,
        CustomerId = 2,
        EmployeeId = 1,
        Description = "Payment still processing.",
        Emergency = false,
        DateCompleted = DateTime.Now
    },
    new ServiceTicket()
    {
        Id = 2,
        CustomerId = 3,
        EmployeeId = 2,
        Description = "Hasn't recieved their order. Shipping lost.",
        Emergency = false,
        DateCompleted = DateTime.Now
    },
    new ServiceTicket()
    {
        Id = 3,
        CustomerId = 1,
        EmployeeId = 1,
        Description = "Account hacked.",
        Emergency = true,
        DateCompleted = DateTime.Now
    },
    new ServiceTicket()
    {
        Id = 4,
        CustomerId = 3,
        EmployeeId = 2,
        Description = "Site crashed. Want's to check if order was accepted. Bank charged their account.",
        Emergency = true,
        DateCompleted = DateTime.Now
    },
    new ServiceTicket()
    {
        Id = 5,
        CustomerId = 2,
        EmployeeId = 1,
        Description = "Forgot Password and/or username. Help link won't work.",
        Emergency = false,
        DateCompleted = DateTime.Now
    }

};

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/serviceTickets", () =>
{
    return serviceTickets;
});

app.MapGet("/servicetickets/{id}", (int id) =>
{
    return serviceTickets.FirstOrDefault(s => s.Id == id);
});

app.Run();
