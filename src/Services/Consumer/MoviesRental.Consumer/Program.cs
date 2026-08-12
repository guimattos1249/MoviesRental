using MoviesRental.Consumer.Setup;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddConsumerConfig(builder.Configuration);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.Run();