using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using webApiCRM.Data;
using webApiCRM.Halpers;
using webApiCRM.Models;
using webApiCRM.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.
    Configuration.
    GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddTransient<ConnectionService>();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
		builder.
		Configuration.
		GetConnectionString("DefaultConnection")
	)
);

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(op =>
{
	op.RequireHttpsMetadata = false;
	op.SaveToken = false;
	op.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuerSigningKey = true,
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidIssuer = builder.Configuration["JWT:Issuer"],
		ValidAudience= builder.Configuration["JWT:Audience"],
	};
}
);

builder.Services.AddControllers();

// Configure the IConfiguration instance
builder.Configuration.AddJsonFile("appsettings.json");

// Add Swagger services
builder.Services.AddSwaggerGen();


/*
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters()
	{
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidateActor = true,
		ValidIssuer = builder.Configuration["Connection:Issuer"],
		ValidAudience = builder.Configuration["Connection:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Connection:AppKey"])),
	};
});*/


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseAuthorization();
	app.UseAuthorization();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
	});


}



app.UseCors(builder =>
{
    builder
        .AllowAnyOrigin() 
        .AllowAnyMethod()
        .AllowAnyHeader();
});



app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
