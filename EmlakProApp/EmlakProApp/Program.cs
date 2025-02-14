using EmlakProApp.Data;
using EmlakProApp.ServiceRegisterations;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Load configuration
var config = builder.Configuration;

// Register services (Dependency Injection)
builder.Services.Register(config);

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAllOrigins",
		builder =>
		{
			builder.AllowAnyOrigin()
				   .AllowAnyHeader()
				   .AllowAnyMethod();
		});
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment()) 
{
	app.UseSwagger();
	app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAllOrigins");

app.UseAuthentication();  // Auth birinci g?lm?lidir
app.UseAuthorization();   // Sonra authorization

app.MapControllers();
app.Run();

