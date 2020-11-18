using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class RepositorioContrato : RepositorioBase, IRepositorio<Contrato>
    {
        public RepositorioContrato(IConfiguration configuration) : base(configuration)
        {

        }

        public int Alta(Contrato c)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"INSERT INTO Contrato ([IdInmueble], [IdInquilino], [FechaInicio], [FechaFin], [Monto], [IdContratoRenovado], [FechaContrato], [Descripcion]) " +
                    $"VALUES ({c.IdInmueble}, {c.IdInquilino}, '{c.FechaInicio.ToString(System.Globalization.CultureInfo.InvariantCulture)}', '{c.FechaFin.ToString(System.Globalization.CultureInfo.InvariantCulture)}', {c.Monto.ToString(System.Globalization.CultureInfo.InvariantCulture)}, null, null, '{c.Descripcion}')";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    command.CommandText = "SELECT SCOPE_IDENTITY()";
                    var id = command.ExecuteScalar();
                    c.IdContrato = Convert.ToInt32(id);
                    connection.Close();
                }
            }
            return res;
        }

        public int Baja(int id)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"DELETE FROM Contrato WHERE IdContrato = {id}";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Contrato> GetAll()
        {
            IList<Contrato> res = new List<Contrato>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT c.[IdContrato], c.[IdInmueble], c.[IdInquilino], c.[FechaInicio], c.[FechaFin], c.[Monto], c.[IdContratoRenovado], c.[FechaContrato], c.[Descripcion], i.Apellido, i.Nombre, p.Tipo, p.Direccion" +
                    $" FROM Contrato c LEFT JOIN Inmueble p on (c.IdInmueble = p.IdInmueble) LEFT JOIN Inquilino i on (c.IdInquilino = i.IdInquilino)";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Contrato c = new Contrato
                        {
                            IdContrato = (int)reader[nameof(c.IdContrato)],
                            Descripcion = reader[nameof(c.Descripcion)].ToString(),
                            IdInmueble = (int)reader[nameof(c.IdInmueble)],
                            IdInquilino = (int)reader[nameof(c.IdInquilino)],
                            FechaInicio = (DateTime)reader[nameof(c.FechaInicio)],
                            FechaFin = (DateTime)reader[nameof(c.FechaFin)],
                            Monto = (decimal)reader[nameof(c.Monto)],
                            Inquilino = new Inquilino
                            {
                                IdInquilino = (int)reader[nameof(Inquilino.IdInquilino)],
                                Nombre = reader[nameof(Inquilino.Nombre)].ToString(),
                                Apellido = reader[nameof(Inquilino.Apellido)].ToString(),
                            },
                            Inmueble = new Inmueble
                            {
                                //p.Tipo, p.Direccion
                                IdInmueble = (int)reader[nameof(Inmueble.IdInmueble)],
                                Tipo = reader[nameof(Inmueble.Tipo)].ToString(),
                                Direccion = reader[nameof(Inmueble.Direccion)].ToString(),
                            }
                        };
                        res.Add(c);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public Contrato GetById(int id)
        {
            Contrato c = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT c.[IdContrato], c.[IdInmueble], c.[IdInquilino], c.[FechaInicio], c.[FechaFin], c.[Monto], c.[IdContratoRenovado], c.[FechaContrato], c.[Descripcion], i.Apellido, i.Nombre, p.Tipo, p.Direccion" +
                    $" FROM Contrato c LEFT JOIN Inmueble p on (c.IdInmueble = p.IdInmueble) LEFT JOIN Inquilino i on (c.IdInquilino = i.IdInquilino)" +
                    $" WHERE IdContrato = @id";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        c = new Contrato
                        {
                            IdContrato = (int)reader[nameof(c.IdContrato)],
                            IdInmueble = (int)reader[nameof(c.IdInmueble)],
                            IdInquilino = (int)reader[nameof(c.IdInquilino)],
                            Descripcion = reader[nameof(c.Descripcion)].ToString(),
                            FechaInicio = (DateTime)reader[nameof(c.FechaInicio)],
                            FechaFin = (DateTime)reader[nameof(c.FechaFin)],
                            Monto = (decimal)reader[nameof(c.Monto)],
                            Inquilino = new Inquilino
                            {
                                IdInquilino = (int)reader[nameof(Inquilino.IdInquilino)],
                                Nombre = reader[nameof(Inquilino.Nombre)].ToString(),
                                Apellido = reader[nameof(Inquilino.Apellido)].ToString(),
                            },
                            Inmueble = new Inmueble
                            {
                                IdInmueble = (int)reader[nameof(Inmueble.IdInmueble)],
                                Tipo = reader[nameof(Inmueble.Tipo)].ToString(),
                                Direccion = reader[nameof(Inmueble.Direccion)].ToString(),
                            }
                        };
                        return c;
                    }
                    connection.Close();
                }
            }
            return c;
        }

        public int Modificacion(Contrato c)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //[IdInmueble], [IdInquilino], [FechaInicio], [FechaFin], [Monto], [IdContratoRenovado], [FechaContrato]
                string sql = $"UPDATE Contrato" + 
                    $" SET IdInmueble={c.IdInmueble}, IdInquilino={c.IdInquilino}, Descripcion='{c.Descripcion}', FechaInicio='{c.FechaInicio.ToString(System.Globalization.CultureInfo.InvariantCulture)}', FechaFin='{c.FechaFin.ToString(System.Globalization.CultureInfo.InvariantCulture)}', Monto={c.Monto.ToString(System.Globalization.CultureInfo.InvariantCulture)} " +
                    $" WHERE IdContrato = {c.IdContrato}";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }
    }
}
