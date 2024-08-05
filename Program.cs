using Serilog;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using trading_app.data;
using System.Text;
using trading_app.models;
using trading_app.interfaces.IServices;
using trading_app.services;
using trading_app.interfaces;
using trading_app.Middleware;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllers();
builder.Services.AddHttpClient();

builder.Services.AddTransient<IAuthServices,AuthService>();
builder.Services.AddTransient<IWireService,WireService>();
builder.Services.AddTransient<ITradeService,TradeService>();
builder.Services.AddTransient<IProfileService,ProfileService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

//identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options=>{
options.Password.RequiredLength=5;
options.Password.RequireDigit=false;
options.Password.RequireLowercase=false;
options.Password.RequireUppercase=false;
options.Password.RequireNonAlphanumeric=false;
options.SignIn.RequireConfirmedEmail=false;
});
string key = builder.Configuration.GetSection("JWT:Key").Value ?? "";


// authentication
builder.Services.AddAuthentication(options=>
{

    options.DefaultAuthenticateScheme=JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme=JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options=>
{   
    options.SaveToken = true;
    options.RequireHttpsMetadata= false;
    options.TokenValidationParameters=new TokenValidationParameters(){
        ValidateActor=false,
        ValidateIssuer=false,
        ValidateAudience=false,
        RequireExpirationTime=true,
        ValidateIssuerSigningKey=true,
        ValidIssuer=builder.Configuration.GetSection("JWT:Issuer").Value,
        ValidAudience=builder.Configuration["JWT:Audience"],
        IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))

    };
});

//swagger
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddCors(options =>
  options.AddPolicy("first", builder=>
  {
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
  })
);
//serilog
Log.Logger =new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

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

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.Run();
