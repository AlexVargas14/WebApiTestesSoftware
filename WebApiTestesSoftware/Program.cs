using WebApiTestesSoftware.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<ITransactionService, TransactionService>();
builder.Services.AddSingleton<IExternalValidationService, ExternalValidationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
