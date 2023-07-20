using KanbanStyleBackEnd.DataAccess;
using KanbanStyleBackEnd.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using KanbanStyleBackEnd;


var builder = WebApplication.CreateBuilder(args);

//Connection with SQL server express

const string CONNECTIONNAME = "KanbanProjectDB";
var connectionString = builder.Configuration.GetConnectionString(CONNECTIONNAME);

//Add Context
builder.Services.AddDbContext<KanbanProjectDBContext>(options => options.UseSqlServer(connectionString));

//Add services of JWT Autorization
builder.Services.AddJwtTokenServices(builder.Configuration);

// Add services to the container.

builder.Services.AddControllers();

//Add custom services (folder services)
builder.Services.AddScoped<IAssignmentsService, AssignmentsService>();
builder.Services.AddScoped<IProjectsService, ProjectsService>();
builder.Services.AddScoped<IUsersService, UsersService>();

//Add Authorization 
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserOnlyPolicy", policy => policy.RequireClaim("UserOnly", "User"));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//TODO: config Swagger to take car of JWT Autorization
builder.Services.AddSwaggerGen(options =>
    {
        //We define the security for authorization
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name= "Authorization", 
            Type= SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization Header using Bearer Scheme"
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
                new string []{}
            }
        });
    }
);


//CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "CorsPolicy", builder =>
    {
        builder.AllowAnyOrigin();
        builder.AllowAnyMethod();
        builder.AllowAnyHeader();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//Tell app to use CORS
app.UseCors("CorsPolicy");

app.Run();
