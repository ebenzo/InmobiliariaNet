using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Hosting;
using Inmobiliaria.Controllers;
using Microsoft.AspNetCore.SignalR;

namespace Inmobiliaria
{
	public class Startup
	{
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
		{
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Home/Login";
                    options.LogoutPath = "/Home/Logout";
                    options.AccessDeniedPath = "/Home/Restringido";
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,//el que emite
                        ValidateAudience = true,//el que la recibe
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["TokenAuthentication:Issuer"],//quien emite
                        ValidAudience = configuration["TokenAuthentication:Audience"],//
                        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(configuration["TokenAuthentication:SecretKey"])),
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Administrador", policy => policy.RequireClaim("Admin"));
            });

            services.AddMvc();
            //services.AddTransient<IRepositorio<Propietario>, RepositorioPropietario>();
            services.AddTransient<IRepositorioPropietario, RepositorioPropietario>();//este es para el HomeController de la pag web
            services.AddTransient<IRepositorio<Inquilino>, RepositorioInquilino>();
            services.AddTransient<IRepositorioInmueble<Inmueble>, RepositorioInmueble>();
            services.AddTransient<IRepositorio<Contrato>, RepositorioContrato>();
            services.AddTransient<IRepositorio<Pago>, RepositorioPago>();
            services.AddDbContext<DataContext>(options => options.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]));
        }

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
            // Habilitar CORS --> cross only request, permitir (o no) que otras apis se comuniquen con esta misma
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.None,
            });
            app.UseAuthentication();

            if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
            app.UseStaticFiles();//esta linea sirve para agregar los archivos css y jquery que estan en wwwroot

			app.UseMvc(routes => {
                routes.MapRoute(
                    name: "login",
                    template: "login/{**accion}",
                    defaults: new { controller = "Home", action = "Login" });
                routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{Id?}");
			});
		}
	}
}
