using Inmobiliaria;
using Inmobiliaria.Api;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;
//using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace InmobiliariaTests
{
    public class PropietariosControllerTest
    {
		Helper helper = new Helper();
		PropietariosController controller;
		public PropietariosControllerTest()
		{
			controller = new PropietariosController(helper.DataContext, helper.Config);
		}

		[Fact]
		public void MiPerfil()
		{
			string email = "ebenco@gmail.com";
			controller.ControllerContext = new ControllerContext()
			{
				HttpContext = new DefaultHttpContext() { User = helper.MockLogin(email, "Propietario") }
			};
			var res = controller.Get().Result as Inmobiliaria.Models.Propietario;
			Assert.Equal(email, res.Email);
			Assert.Equal("Emmanuel", res.Nombre);
		}

		[Fact]
		public void PerfilAnonimoInexistente()
		{
			controller.ControllerContext = new ControllerContext()
			{
				HttpContext = new DefaultHttpContext() { User = new ClaimsPrincipal(new ClaimsIdentity()) }
			};
			var res = controller.Get().Result as Inmobiliaria.Models.Propietario;
			Assert.Null(res);
		}

		[Fact]
		public void ControladorRequiereAutenticacion()
		{
			var atributos = controller.GetType().GetCustomAttributes(
				typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true);
			var auth = atributos[0] as Microsoft.AspNetCore.Authorization.AuthorizeAttribute;
			Assert.NotNull(auth);
			Assert.Equal(JwtBearerDefaults.AuthenticationScheme, auth.AuthenticationSchemes);
			//Assert.Equal("Propietario", auth.Policy);
		}

		[Fact]
		public async Task PerfilProhibidoSinAutenticar()
		{
			// Arrange
			var server = new TestServer(new WebHostBuilder()
				.UseConfiguration(helper.Config)
				.UseStartup<Startup>()
			);
			var client = server.CreateClient();
			var url = "api/propietarios";
			var codigo = HttpStatusCode.Unauthorized;

			// Act
			var response = await client.GetAsync(url);

			// Assert
			Assert.Equal(codigo, response.Sta.tusCode);
			Assert.Contains("Bearer", response.Headers.WwwAuthenticate.First().ToString(), StringComparison.InvariantCultureIgnoreCase);
		
		}
	}
}
