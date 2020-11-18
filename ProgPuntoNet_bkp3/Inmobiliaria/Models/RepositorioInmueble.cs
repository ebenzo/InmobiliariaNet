using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class RepositorioInmueble : RepositorioBase, IRepositorioInmueble<Inmueble>
    {
        public RepositorioInmueble(IConfiguration configuration) : base(configuration)
        {

        }
        public int Alta(Inmueble i)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"INSERT INTO Inmueble ([Tipo], [Direccion], [Uso], [Ambientes], [Disponible], [Precio], [IdPropietario], [Habilitado]) " +
                    $"VALUES ('{i.Tipo}','{i.Direccion}','{i.Uso}',{i.Ambientes}, {i.Disponible}, {i.Precio.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {i.IdPropietario}, 1)";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    command.CommandText = "SELECT SCOPE_IDENTITY()";
                    var id = command.ExecuteScalar();
                    i.IdInmueble = Convert.ToInt32(id);
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
                string sql = $"DELETE FROM Inmueble WHERE IdInmueble = {id}";
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

        /// <summary>
        /// Retorna una lista de inmuebles con los datos basicos
        /// </summary>
        /// <returns></returns>
        public IList<Inmueble> GetAll()
        {
            IList<Inmueble> res = new List<Inmueble>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT i.IdInmueble, i.Tipo, i.Direccion, i.Uso, i.Ambientes, i.Disponible, i.Precio" +
                    $" FROM Inmueble i";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Inmueble i = new Inmueble
                        {
                            IdInmueble = (int)reader[nameof(i.IdInmueble)],
                            Tipo = reader[nameof(i.Tipo)].ToString(),
                            Direccion = reader[nameof(i.Direccion)].ToString(),
                            Uso = reader[nameof(i.Uso)].ToString(),
                            Ambientes = (int)reader[nameof(i.Ambientes)],
                            Disponible = (int)reader[nameof(i.Disponible)],
                            Precio = (decimal)reader[nameof(i.Precio)],
                        };
                        res.Add(i);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        /// <summary>
        /// Retorna una lista de inmuebles junto con los datos del propietario de cada uno
        /// </summary>
        /// <returns></returns>
        public IList<Inmueble> GetAllWithProp()
        {
            IList<Inmueble> res = new List<Inmueble>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT i.IdInmueble, i.Tipo, i.Direccion, i.Uso, i.Ambientes, i.Disponible, i.Precio, p.IdPropietario, p.Nombre, p.Apellido" +
                    $" FROM Inmueble i LEFT JOIN Propietario p on (i.IdPropietario = p.IdPropietario)";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Inmueble i = new Inmueble
                        {
                            IdInmueble = (int)reader[nameof(i.IdInmueble)],
                            Tipo = reader[nameof(i.Tipo)].ToString(),
                            Uso = reader[nameof(i.Uso)].ToString(),
                            Ambientes = (int)reader[nameof(i.Ambientes)],
                            Disponible = (int)reader[nameof(i.Disponible)],
                            Precio = (decimal)reader[nameof(i.Precio)],
                            Direccion = reader[nameof(i.Direccion)].ToString(),
                            IdPropietario = (int)reader[nameof(i.IdPropietario)],
                            Propietario = new Propietario
                            {
                                IdPropietario = (int)reader[nameof(Propietario.IdPropietario)],
                                Nombre = reader[nameof(Propietario.Nombre)].ToString(),
                                Apellido = reader[nameof(Propietario.Apellido)].ToString(),
                            }
                        };
                        res.Add(i);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public Inmueble GetById(int id)
        {
            Inmueble i = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT i.IdInmueble, i.Tipo, i.Direccion, i.Uso, i.Ambientes, i.Disponible, i.Precio, p.IdPropietario, p.Nombre, p.Apellido" +
                    $" FROM Inmueble i LEFT JOIN Propietario p on (i.IdPropietario = p.IdPropietario)" + 
                    $" WHERE IdInmueble = @id";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        i = new Inmueble
                        {
                            IdInmueble = (int)reader[nameof(i.IdInmueble)],
                            Tipo = reader[nameof(i.Tipo)].ToString(),
                            Uso = reader[nameof(i.Uso)].ToString(),
                            Ambientes = (int)reader[nameof(i.Ambientes)],
                            Disponible = (int)reader[nameof(i.Disponible)],
                            Precio = (decimal)reader[nameof(i.Precio)],
                            Direccion = reader[nameof(i.Direccion)].ToString(),
                            IdPropietario = (int)reader[nameof(i.IdPropietario)],
                            Propietario = new Propietario
                            {
                                IdPropietario = (int)reader[nameof(Propietario.IdPropietario)],
                                Nombre = reader[nameof(Propietario.Nombre)].ToString(),
                                Apellido = reader[nameof(Propietario.Apellido)].ToString(),
                            }
                        };
                        return i;
                    }
                    connection.Close();
                }
            }
            return i;
        }

        public int Modificacion(Inmueble i)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                System.Globalization.CultureInfo provider = new System.Globalization.CultureInfo("es-AR");
                System.Globalization.NumberStyles style = System.Globalization.NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.AllowThousands;
                decimal number = decimal.Parse(i.Precio.ToString(), style, provider);
                string sql = $"UPDATE Inmueble SET Tipo='{i.Tipo}', Direccion='{i.Direccion}', Uso='{i.Uso}', Ambientes={i.Ambientes}, Disponible={i.Disponible}, Precio={number.ToString(System.Globalization.CultureInfo.InvariantCulture)}, IdPropietario={i.IdPropietario} " +
                    $" WHERE IdInmueble = {i.IdInmueble}";
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
