using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria.Models;

namespace Inmobiliaria.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        /*
         * las propiedades llevan el mismo nombre que la tabla en la BD, en este caso son nombres en singular
         * Propitario --> NO Propietarios
         */

        public DbSet<Propietario> Propietario { get; set; }
        public DbSet<Inquilino> Inquilino { get; set; }
        public DbSet<Inmueble> Inmueble { get; set; }
        public DbSet<Contrato> Contrato { get; set; }
    }
}
