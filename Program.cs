using Microsoft.AspNetCore.Authorization;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://127.0.0.1:5500")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});


builder.Services.AddScoped<TokenService>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => 
{
    options.TokenValidationParameters = new TokenValidationParameters
    {

            ValidateIssuer = true,

            ValidIssuer = AuthOptions.ISSUER,

            ValidateAudience = true,

            ValidAudience = AuthOptions.AUDIENCE,

            ValidateLifetime = true,

            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),

            ValidateIssuerSigningKey = true,
         };
});
builder.Services.AddAuthorization(options => options.DefaultPolicy =
    new AuthorizationPolicyBuilder
            (JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser().Build());

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<UserContext>()
            .AddDefaultTokenProviders();



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo {Title = "Test", Version = "v1"});
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});


builder.Services.AddSingleton<UserContext>(provider => 
{
    var options = new DbContextOptionsBuilder<UserContext>();
    options.UseSqlite("Data Source=AccountDatabase.db");
    UserContext context = new UserContext(options.Options);
    return context;

});

builder.Services.AddTransient<ITokenService>(provider => 
{
    ITokenService tokenService = new TokenService();
    return tokenService;
});

builder.Services.AddScoped<IEmailService>(provider =>
{
    IEmailService emailService = new EmailService();
    return emailService;
});


builder.Services.AddScoped<ILessonService>(provider =>
{
    var options = new DbContextOptionsBuilder<LessonContext>();
    options.UseSqlite("Data Source=LessonDatabase.db");
    var lessonContext = new LessonContext(options.Options);
    lessonContext.Database.EnsureCreated();
    ILessonRepository lessonRepository = new LessonRepository(lessonContext);
    ILessonService lessonService = new LessonService(lessonRepository);
    return lessonService;
});

builder.Services.AddSingleton<IUserValidatorService>(provider =>
{
    IUserValidatorService userValidatorService = new UserValidatorService();
    return userValidatorService;
});





var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseCors();
app.Run();
