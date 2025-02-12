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
if (app.Environment.IsDevelopment() || true) // H?r mühitd? Swagger aktiv olsun
{
	app.UseSwagger();
	app.UseSwaggerUI();
}


app.UseStaticFiles();
app.UseAuthentication();
app.UseCors("AllowAllOrigins");


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
