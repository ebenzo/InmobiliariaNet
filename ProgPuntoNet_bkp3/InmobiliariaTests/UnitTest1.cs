using Inmobiliaria.Controllers;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using Xunit;

namespace InmobiliariaTests
{
    public class UnitTest1
    {
		[Fact]
		public void ViewEditPorId()
		{
			//Arrange
			var mockRepo = new Mock<IRepositorioPropietario>();
			//los métodos que se hagan mock, deben ser virtuales
			mockRepo.Setup(x => x.GetById(1)).Returns(
				new Propietario { 
					IdPropietario = 1, 
					Nombre = "Pepe", 
					Apellido = "Perez" 
				});
			var mockConfig = new Mock<IConfiguration>();
			var controlador = new PropietariosController(mockRepo.Object);
			var httpContext = new DefaultHttpContext();
			var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
			controlador.TempData = tempData;

			//Act
			var res = controlador.Edit(1) as ViewResult;

			//Assert
			Assert.NotNull(res);
			mockRepo.Verify(x => x.GetById(1), Times.Exactly(1), "No se llamó a GetById exactamente 1 vez");
			Assert.NotNull(res.Model);
			Assert.IsType<Propietario>(res.Model);
			Propietario p = res.Model as Propietario;
			Assert.Equal(1, p.IdPropietario);
		}
	}
}
