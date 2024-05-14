using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TeduBlog.Api;
using TeduBlog.Core.Domain.Identity;
using TeduBlog.Core.Models.Content;
using TeduBlog.Core.SeedWorks;
using TeduBlog.Data;
using TeduBlog.Data.Repositories;
using TeduBlog.Data.SeedWorks;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("DB");

//Config DB Context and ASP .NET Core Identity
builder.Services.AddDbContext<TeduBlogContext>(options =>
				options.UseSqlServer(connectionString));

builder.Services.AddIdentity<AppUser, AppRole>(options => options.SignIn.RequireConfirmedAccount = false)
	.AddEntityFrameworkStores<TeduBlogContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
	//Password setting.
	options.Password.RequireDigit = true;
	options.Password.RequireLowercase = true;
	options.Password.RequireNonAlphanumeric = true;
	options.Password.RequireUppercase = true;
	options.Password.RequiredLength = 6;
	options.Password.RequiredUniqueChars = 1;

	//Lockout settings.
	options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
	options.Lockout.MaxFailedAccessAttempts = 5;
	options.Lockout.AllowedForNewUsers = true;

	//User setting.
	options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
	options.User.RequireUniqueEmail = false;
});

// Add services to the container.
builder.Services.AddScoped(typeof(IRepository<,>), typeof(RepositoryBase<,>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//Business services and repositories.
//builder.Services.AddScoped<IPostRepository, PostRepository>();
var services = typeof(PostRepository).Assembly.GetTypes()
	.Where(x => x.GetInterfaces().Any(i => i.Name == typeof(IRepository<,>).Name)
	&& !x.IsAbstract && x.IsClass && !x.IsGenericType);
	
foreach (var service in services)
{
	var allInterfaces = service.GetInterfaces();
	var directInterface = allInterfaces.Except(allInterfaces.SelectMany(t => t.GetInterfaces())).FirstOrDefault();
	if(directInterface != null)
	{
		builder.Services.Add(new ServiceDescriptor(directInterface, service, ServiceLifetime.Scoped));
	}
}

builder.Services.AddAutoMapper(typeof(PostInListDTO));


//Default config for ASP .NET Core
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

//Seeding data
app.MigrateDatabase();

app.Run();
