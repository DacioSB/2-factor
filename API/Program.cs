using Repository;
using Service;
using Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();


// Register the UserRepository instance with the dependency injection container
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IRepository<User>, UserRepository>();

var app = builder.Build();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

await app.RunAsync();
