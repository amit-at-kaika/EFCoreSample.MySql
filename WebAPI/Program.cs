using System.Text; //Encoding
using EFCoreSample.MySql.Config;
using EFCoreSample.MySql.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer; //JwtBearerDefaults
using Microsoft.AspNetCore.Identity; //IdentityUser
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens; //TokenValidationParameters

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApiDbContext>(options =>
{
    options.UseMySQL(builder.Configuration.GetConnectionString("MySQLConnection")!);
});

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtConfig:Secret").Value!);
var tokenValidationParameters = new TokenValidationParameters()
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = false, //TODO: for dev
    ValidateAudience = false, //TODO: for dev 
    RequireExpirationTime = false, //TODO: for dev - needs to be changed when refresh tokens are updated
    ValidateLifetime = true
};

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(jwt =>
{
    var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtConfig:Secret").Value!);
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = tokenValidationParameters;
});
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedEmail = false) //TODO: for dev - mail verfication is to be added
    .AddEntityFrameworkStores<ApiDbContext>();
builder.Services.AddSingleton(tokenValidationParameters);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
