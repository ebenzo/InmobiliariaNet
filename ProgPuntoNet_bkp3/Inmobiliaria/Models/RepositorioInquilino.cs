using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class RepositorioInquilino : RepositorioBase, IRepositorio<Inquilino>
    {
        public RepositorioInquilino(IConfiguration configuration) : base(configuration)
        {

        }

        public int Alta(Inquilino p)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"INSERT INTO Inquilino (Nombre, Apellido, Dni, Telefono, Email, Direccion) " +
                    $"VALUES ('{p.Nombre}', '{p.Apellido}','{p.Dni}','{p.Telefono}','{p.Email}','{p.Direccion}')";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    command.CommandText = "SELECT SCOPE_IDENTITY()";
                    var id = command.ExecuteScalar();
                    p.IdInquilino = Convert.ToInt32(id);
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
                string sql = $"DELETE FROM Inquilino WHERE IdInquilino = {id}";
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
        public int Modificacion(Inquilino p)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"UPDATE Inquilino SET Nombre='{p.Nombre}', Apellido='{p.Apellido}', Dni='{p.Dni}', Telefono='{p.Telefono}', Email='{p.Email}', Direccion='{p.Direccion}' " +
                    $"WHERE IdInquilino = {p.IdInquilino}";
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

        public IList<Inquilino> GetAll()
        {
            IList<Inquilino> res = new List<Inquilino>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT IdInquilino, Nombre, Apellido, Dni, Telefono, Email, Direccion" +
                    $" FROM Inquilino";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Inquilino p = new Inquilino
                        {
                            IdInquilino = (int)reader[nameof(p.IdInquilino)],
                            Nombre = reader[nameof(p.Nombre)].ToString(),
                            Apellido = reader[nameof(p.Apellido)].ToString(),
                            Dni = reader[nameof(p.Dni)].ToString(),
                            Telefono = reader[nameof(p.Telefono)].ToString(),
                            Email = reader[nameof(p.Email)].ToString(),
                            Direccion = reader[nameof(p.Direccion)].ToString(),
                        };
                        res.Add(p);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public Inquilino GetById(int id)
        {
            Inquilino p = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT IdInquilino, Nombre, Apellido, Dni, Telefono, Email, Direccion FROM Inquilino" +
                    $" WHERE IdInquilino=@id";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        p = new Inquilino
                        {
                            IdInquilino = (int)reader[nameof(p.IdInquilino)],
                            Nombre = reader[nameof(p.Nombre)].ToString(),
                            Apellido = reader[nameof(p.Apellido)].ToString(),
                            Dni = reader[nameof(p.Dni)].ToString(),
                            Telefono = reader[nameof(p.Telefono)].ToString(),
                            Email = reader[nameof(p.Email)].ToString(),
                            Direccion = reader[nameof(p.Direccion)].ToString(),
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
