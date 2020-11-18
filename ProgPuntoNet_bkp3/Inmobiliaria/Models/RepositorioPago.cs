using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class RepositorioPago : RepositorioBase, IRepositorio<Pago>
    {
        public RepositorioPago(IConfiguration configuration) : base(configuration)
        {

        }

        public int Alta(Pago p)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"INSERT INTO Pagos ([IdContrato], [IdInquilino], [NroPago], [FechaPago], [Importe]) " +
                    $"VALUES ({p.IdContrato}, {p.IdInquilino}, {p.NroPago}, '{p.FechaPago.ToString(System.Globalization.CultureInfo.InvariantCulture)}', {p.Importe.ToString(System.Globalization.CultureInfo.InvariantCulture)})";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    command.CommandText = "SELECT SCOPE_IDENTITY()";
                    var id = command.ExecuteScalar();
                    p.IdPago = Convert.ToInt32(id);
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
                string sql = $"DELETE FROM Pagos WHERE IdPago = {id}";
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

        public IList<Pago> GetAll()
        {
            IList<Pago> res = new List<Pago>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT p.IdPago, p.IdContrato, p.IdInquilino, p.NroPago, p.FechaPago, p.Importe, c.Descripcion, i.Nombre, i.Apellido" +
                    $" FROM Pagos p LEFT JOIN Contrato c on (p.IdContrato = c.IdContrato) LEFT JOIN Inquilino i on (p.IdInquilino = i.IdInquilino)";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Pago p = new Pago
                        {
                            IdPago = (int)reader[nameof(p.IdPago)],
                            IdContrato = (int)reader[nameof(p.IdContrato)],
                            IdInquilino = (int)reader[nameof(p.IdInquilino)],
                            FechaPago = (DateTime)reader[nameof(p.FechaPago)],
                            NroPago = (int)reader[nameof(p.NroPago)],
                            Importe = (decimal)reader[nameof(p.Importe)],
                            
                            Contrato = new Contrato
                            {
                                IdContrato = (int)reader[nameof(Contrato.IdContrato)],
                                Descripcion = reader[nameof(Contrato.Descripcion)].ToString(),
                            },
                            Inquilino = new Inquilino
                            {
                                IdInquilino = (int)reader[nameof(Inquilino.IdInquilino)],
                                Nombre = (string)reader[nameof(Inquilino.Nombre)],
                                Apellido = (string)reader[nameof(Inquilino.Apellido)],
                            }
                        };
                        res.Add(p);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public Pago GetById(int id)
        {
            Pago p = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT p.IdPago, p.IdContrato, p.IdInquilino, p.NroPago, p.FechaPago, p.Importe, c.Descripcion, i.Nombre, i.Apellido" +
                    $" FROM Pagos p LEFT JOIN Contrato c on (p.IdContrato = c.IdContrato) LEFT JOIN Inquilino i on (p.IdInquilino = i.IdInquilino)" +
                    $" WHERE IdPago = @id";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        p = new Pago
                        {
                            IdPago = (int)reader[nameof(p.IdPago)],
                            IdContrato = (int)reader[nameof(p.IdContrato)],
                            IdInquilino = (int)reader[nameof(p.IdInquilino)],
                            FechaPago = (DateTime)reader[nameof(p.FechaPago)],
                            NroPago = (int)reader[nameof(p.NroPago)],
                            Importe = (decimal)reader[nameof(p.Importe)],

                            Contrato = new Contrato
                            {
                                IdContrato = (int)reader[nameof(Contrato.IdContrato)],
                                Descripcion = reader[nameof(Contrato.Descripcion)].ToString(),
                            },
                            Inquilino = new Inquilino
                            {
                                IdInquilino = (int)reader[nameof(Inquilino.IdInquilino)],
                                Nombre = (string)reader[nameof(Inquilino.Nombre)],
                                Apellido = (string)reader[nameof(Inquilino.Apellido)],
                            }
                        };
                        return p;
                    }
                    connection.Close();
                }
            }
            return p;
        }

        public int Modificacion(Pago p)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //[IdContrato], [IdInquilino], [NroPago], [FechaPago], [Importe]
                string sql = $"UPDATE Pagos" +
                    $" SET IdContrato={p.IdContrato}, IdInquilino={p.IdInquilino}, NroPago='{p.NroPago}', FechaPago='{p.FechaPago.ToString(System.Globalization.CultureInfo.InvariantCulture)}', Importe={p.Importe.ToString(System.Globalization.CultureInfo.InvariantCulture)} " +
                    $" WHERE IdPago = {p.IdPago}";
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
