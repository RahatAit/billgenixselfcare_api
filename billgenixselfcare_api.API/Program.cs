using billgenixselfcare_api.Application;
using billgenixselfcare_api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Authorization Policies
builder.Services.AddAuthorizationBuilder()
    // Policy for User
    .AddPolicy("User.View", policy => policy.RequireAssertion(context =>
        context.User.HasClaim(claim => claim.Type == "Permission" && claim.Value == "User.View") ||
        context.User.IsInRole("Super Admin")
    ))
    .AddPolicy("User.Create", policy => policy.RequireAssertion(context =>
        context.User.HasClaim(claim => claim.Type == "Permission" && claim.Value == "User.Create") ||
        context.User.IsInRole("Super Admin")
    ))
    // Policy for Menu
    .AddPolicy("Menu.View", policy => policy.RequireClaim("Permission", "Menu.View"))
    .AddPolicy("Menu.Create", policy => policy.RequireClaim("Permission", "Menu.Create"))
    .AddPolicy("Menu.Update", policy => policy.RequireClaim("Permission", "Menu.Update"))
    .AddPolicy("Menu.Delete", policy => policy.RequireClaim("Permission", "Menu.Delete"));

// CORS
builder.Services.AddCors(options =>
{
    var origins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? new string[] { };
    options.AddPolicy("Spa", policy =>
    {
        policy.WithOrigins(origins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("Spa");

app.UseAuthorization();

app.MapControllers();

// Seed data
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    await SeedData.Initialize(services);
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred seeding the DB: {ex.Message}");
}

app.Run();
