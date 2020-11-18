using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class RepositorioPropietario : RepositorioBase, IRepositorioPropietario
    {
        public RepositorioPropietario(IConfiguration configuration) : base(configuration)
        {

        }

        public int Alta(Propietario p)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"INSERT INTO Propietario (Nombre, Apellido, Dni, Telefono, Email, Direccion, Clave) " +
                    $"VALUES ('{p.Nombre}', '{p.Apellido}','{p.Dni}','{p.Telefono}','{p.Email}','{p.Direccion}','{p.Clave}')";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    command.CommandText = "SELECT SCOPE_IDENTITY()";
                    var id = command.ExecuteScalar();
                    p.IdPropietario = Convert.ToInt32(id);
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
                string sql = $"DELETE FROM Propietario WHERE IdPropietario = {id}";
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
        public int Modificacion(Propietario p)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"UPDATE Propietario SET Nombre='{p.Nombre}', Apellido='{p.Apellido}', Dni='{p.Dni}', Telefono='{p.Telefono}', Email='{p.Email}', Direccion='{p.Direccion}', Clave='{p.Clave}' " +
                    $"WHERE IdPropietario = {p.IdPropietario}";
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

        public IList<Propietario> GetAll()
        {
            IList<Propietario> res = new List<Propietario>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT IdPropietario, Nombre, Apellido, Dni, Telefono, Email, Direccion, Clave" +
                    $" FROM Propietario";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Propietario p = new Propietario
                        {
                            IdPropietario = (int)reader[nameof(p.IdPropietario)],
                            Nombre = reader[nameof(p.Nombre)].ToString(),
                            Apellido = reader[nameof(p.Apellido)].ToString(),
                            Dni = reader[nameof(p.Dni)].ToString(),
                            Telefono = reader[nameof(p.Telefono)].ToString(),
                            Email = reader[nameof(p.Email)].ToString(),
                            Direccion = reader[nameof(p.Direccion)].ToString(),
                            Clave = reader[nameof(p.Clave)].ToString(),
                        };
                        res.Add(p);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public Propietario GetById(int id)
        {
            Propietario p = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT IdPropietario, Nombre, Apellido, Dni, Telefono, Email, Direccion, Clave FROM Propietario" +
                    $" WHERE IdPropietario=@id";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        p = new Propietario
                        {
                            IdPropietario = (int)reader[nameof(p.IdPropietario)],
                            Nombre = reader[nameof(p.Nombre)].ToString(),
                            Apellido = reader[nameof(p.Apellido)].ToString(),
                            Dni = reader[nameof(p.Dni)].ToString(),
                            Telefono = reader[nameof(p.Telefono)].ToString(),
                            Email = reader[nameof(p.Email)].ToString(),
                            Direccion = reader[nameof(p.Direccion)].ToString(),
                            Clave = reader[nameof(p.Clave)].ToString(),
                        };
                        return p;
                    }
                    connection.Close();
                }
            }
            return p;
        }

        public Propietario GetByEmail(string email)
        {
            Propietario p = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT IdPropietario, Nombre, Apellido, Dni, Telefono, Email, Direccion, Clave FROM Propietario" +
                    $" WHERE Email = @email";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@email", SqlDbType.VarChar).Value = email;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        p = new Propietario
                        {
                            IdPropietario = (int)reader[nameof(p.IdPropietario)],
                            Nombre = reader[nameof(p.Nombre)].ToString(),
                            Apellido = reader[nameof(p.Apellido)].ToString(),
                            Dni = reader[nameof(p.Dni)].ToString(),
                            Telefono = reader[nameof(p.Telefono)].ToString(),
                            Email = reader[nameof(p.Email)].ToString(),
                            Direccion = reader[nameof(p.Direccion)].ToString(),
                            Clave = reader[nameof(p.Clave)].ToString(),
                        };
                        return p;
                    }
                    connection.Close();
                }
            }
            return p;
        }
    }
}
