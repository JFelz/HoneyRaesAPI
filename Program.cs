using HoneyRaesAPI.Models;

var builder = WebApplication.CreateBuilder(args);

List<Customer> customers = new List<Customer>()
{
    new Customer()
    {
        Id = 1,
        Name = "Robert Mahr",
        Address = "8758 Sussex St. Merrillville, IN 46410"
    },
    new Customer()
    {
        Id = 2,
        Name = "Ashley Cohn",
        Address = "619 Baker Rd. Fuquay Varina, NC 27526"
    },
    new Customer()
    {
        Id = 3,
        Name =  "Isabella Rodriguez",
        Address = "569 Kirkland St. Memphis, TN 38106"
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
    new Employee()
    {
        Id = 5,
        Name = "Jovanni Feliz",
        Specialty = "CyberSecurity"
    },
};

List<ServiceTicket> serviceTickets = new List<ServiceTicket>()
{
    new ServiceTicket()
    {
        Id = 1,
        CustomerId = 2,
        Description = "Payment still processing.",
        Emergency = false,

    },
    new ServiceTicket()
    {
        Id = 2,
        CustomerId = 3,
        EmployeeId = 2,
        Description = "Hasn't recieved their order. Shipping lost.",
        Emergency = true

    },
    new ServiceTicket()
    {
        Id = 3,
        CustomerId = 1,
        Description = "Account hacked.",
        Emergency = true,
        DateCompleted = new DateTime(2016, 2, 15)
    },
    new ServiceTicket()
    {
        Id = 4,
        CustomerId = 3,
        EmployeeId = 2,
        Description = "Site crashed. Want's to check if order was accepted. Bank charged their account.",
        Emergency = true
    },
    new ServiceTicket()
    {
        Id = 5,
        CustomerId = 2,
        EmployeeId = 2,
        Description = "Forgot Password and/or username. Help link won't work.",
        Emergency = false,
        DateCompleted = DateTime.Now
    },
    new ServiceTicket()
    {
        Id = 6,
        CustomerId = 1,
        EmployeeId = 2,
        Description = "Button issue",
        Emergency = false,
        DateCompleted = new DateTime(2023, 2, 15)
    },
    new ServiceTicket()
    {
        Id = 7,
        CustomerId = 3,
        EmployeeId = 1,
        Description = "Login issue",
        Emergency = false,
        DateCompleted = new DateTime(2023, 2, 15)
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
    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(s => s.Id == id);
    if (serviceTicket == null)
    {
        Console.WriteLine("Service Ticket is not in database!");
        return Results.NotFound();
    }
    serviceTicket.Employee = employees.FirstOrDefault(e => e.Id == serviceTicket.EmployeeId);

    serviceTicket.Customer = customers.FirstOrDefault(c => c.Id == serviceTicket.CustomerId);
    return Results.Ok(serviceTicket);
});

app.MapGet("/employees", () =>
{
    return employees;
});

app.MapGet("/employees/{id}", (int id) =>
{
    Employee employee = employees.FirstOrDefault(e => e.Id == id);
    if (employee == null)
    {
        return Results.NotFound();
    }
    employee.ServiceTickets = serviceTickets.Where(s => s.EmployeeId == id).ToList();
    return Results.Ok(employee);
});

app.MapGet("/customers", () =>
{
    return customers;
});

app.MapGet("/customers/{id}", (int id) => 
{
    Customer customer = customers.FirstOrDefault(c => c.Id == id);
    if (customer == null)
    {
        return Results.NotFound();
    }
    customer.ServiceTickets = serviceTickets.Where(s => s.CustomerId == id).ToList();
    return Results.Ok(customer);
});

//This creates a new Id. It checks if an Id is the highest number in the array, if so, add 1.
//It will continuously add the next highest Id number to every serviceticket added.
app.MapPost("/servicetickets", (ServiceTicket serviceTicket) =>
{
    serviceTicket.Id = serviceTickets.Max(st => st.Id) + 1;
    serviceTickets.Add(serviceTicket);
    return serviceTicket;
});

app.MapDelete("/serviceticketsDEL/{id}", (int id) =>
{
    ServiceTicket serviceTicketDEL = serviceTickets.FirstOrDefault(st => st.Id == id);
    serviceTickets.Remove(serviceTicketDEL);
});

app.MapPut("/servicetickets/{id}", (int id, ServiceTicket serviceTicket) =>
{
    ServiceTicket ticketToUpdate = serviceTickets.FirstOrDefault(st => st.Id == id);
    int ticketIndex = serviceTickets.IndexOf(ticketToUpdate);
    if (ticketToUpdate == null)
    {
        return Results.NotFound();
    }
    //the id in the request route doesn't match the id from the ticket in the request body. That's a bad request!
    if (id != serviceTicket.Id)
    {
        return Results.BadRequest();
    }
    serviceTickets[ticketIndex] = serviceTicket;
    return Results.Ok();
});

app.MapPost("/servicetickets/{id}/complete", (int id) =>
{
    ServiceTicket ticketCompleted = serviceTickets.FirstOrDefault(st => st.Id == id);
    ticketCompleted.DateCompleted = DateTime.Today;

});

app.MapGet("/servicetickets/active", () =>
{
    List<ServiceTicket> serviceTicket = serviceTickets.Where(st => st.Emergency == true && st.DateCompleted == null).ToList();

    return serviceTicket;
});

app.MapGet("/servicetickets/unassigned", () =>
{
    List<ServiceTicket> unassignedTickets = serviceTickets.Where(st => st.EmployeeId == null).ToList();
    return unassignedTickets;
});

app.MapGet("/servicetickets/inactive", () =>
{
    DateTime currentYear = DateTime.Now;

    List<ServiceTicket> serviceTicket = serviceTickets.Where(st => st.DateCompleted != null && st.DateCompleted < currentYear.AddYears(-1)).ToList();
    return serviceTicket;
});

app.MapGet("/employees/available", () =>
{
    List<ServiceTicket> serviceTicket = serviceTickets.Where(x => serviceTickets.Any(y => y.EmployeeId == x.Id && y.DateCompleted == null)).ToList();
    List<ServiceTicket> AvailableEmployees = serviceTickets.Where(x => serviceTicket.Any(y => y.EmployeeId != x.Id)).ToList();
    return AvailableEmployees;
});

app.MapGet("employees/{id}/customers/assigned", (int id) =>
{
    List<ServiceTicket> currentEmployees = serviceTickets.Where(st => st.EmployeeId == id).ToList();
    List<int> customerIds = currentEmployees.Select(x => x.CustomerId).ToList();
    List<Customer> assignedCustomer = customers.Where(c => customerIds.Contains(c.Id)).ToList();
    if (assignedCustomer.Count == 0)
    {
        return null;
    } 
    return assignedCustomer;
});

app.MapGet("employees/EOTM", () =>
{
    DateTime currentDate = DateTime.Now;
    List<ServiceTicket> lastMonth = serviceTickets.Where(x => x.DateCompleted > currentDate.AddMonths(-1)).ToList();
    List<ServiceTicket> dateCompletedObjs = lastMonth.Where(x => x.DateCompleted != null && x.EmployeeId != null).ToList();
    List<Employee> EmOfMonth = employees.Where(x => dateCompletedObjs.Any(y => y.EmployeeId == x.Id)).ToList();

    return Results.Ok(EmOfMonth);
});

app.MapGet("/completed", () =>
{
    var completedTickets = serviceTickets.Where(st => st.DateCompleted != null).OrderBy(st => st.DateCompleted);
    return Results.Ok(completedTickets);
});

app.Run();