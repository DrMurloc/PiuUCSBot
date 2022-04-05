var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>()
    .AddEnvironmentVariables();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) app.UseHsts();

app.UseHttpsRedirection();


app.UseRouting();

app.UseAuthorization();


app.Run();