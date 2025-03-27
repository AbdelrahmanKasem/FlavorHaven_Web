using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RMSProjectAPI.Database;
using RMSProjectAPI.Database.Entity;
using System.Text;
using RMSProjectAPI.Hubs;

namespace RMSProjectAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(o =>
            {
                // Online Database
                //o.UseSqlServer("Server=db14415.databaseasp.net; Database=db14415; User Id=db14415; Password=5Tj@z8+M3_Ex; Encrypt=False; MultipleActiveResultSets=True;");

                // Local Database
                o.UseSqlServer("Data Source=.;Initial Catalog=DB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");
            });

            // QR Code Service
            builder.Services.AddScoped<TableRepository>();
            builder.Services.AddSingleton<QRCodeService>();
            builder.Services.AddSignalR();


            // Add services to the container.
            builder.Services.AddIdentity<User, IdentityRole<Guid>>()
                     .AddEntityFrameworkStores<AppDbContext>()
                     .AddDefaultTokenProviders();

            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    ClockSkew = TimeSpan.Zero
                };
            });
            builder.Services.AddAuthorization();
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(i =>
                {
                    i.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            // Uploading Images
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors();

            app.MapControllers();

            app.MapHub<ChatHub>("/chatHub");

            app.Run();
        }
    }
}
