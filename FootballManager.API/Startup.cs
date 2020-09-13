using AutoMapper;
using FluentValidation.AspNetCore;
using FootballManager.Data;
using FootballManager.Data.Repositories;
using FootballManager.Data.Repositories.Concrete;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FootballManager.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddDbContext<FootballManagerContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString(nameof(FootballManagerContext)))
                    .EnableSensitiveDataLogging());
            services.AddSwaggerGen();
            services.AddMvc().AddFluentValidation();
            
            var mappingConfig = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new DtoMapperProfile());
                });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddScoped<IClubRepository, ClubRepository>();
            services.AddScoped<ICoachRepository, CoachRepository>();
            services.AddScoped<IFootballerRepository, FootballerRepository>();
            services.AddScoped<IStadiumRepository, StadiumRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }
    }
}