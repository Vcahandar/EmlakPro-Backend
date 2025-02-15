using EmlakProApp.Data;
using EmlakProApp.ServiceRegisterations;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// Load configuration
var config = builder.Configuration;

// Register services (Dependency Injection)
builder.Services.Register(config);

// CORS siyas?ti ?lav? et
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll",
		policy =>
		{
			policy.AllowAnyOrigin()
				  .AllowAnyMethod()
				  .AllowAnyHeader();
		});
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment() || true) 
{
	app.UseSwagger();
	app.UseSwaggerUI();
}



//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAll");

app.UseAuthentication();  // Auth birinci g?lm?lidir
app.UseAuthorization();   // Sonra authorization

app.MapControllers();
app.Run();

