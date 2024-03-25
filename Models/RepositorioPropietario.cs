using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace PenalozaFernandezInmobiliario.Models;

public class RepositorioPropietario
{

    readonly string ConnectionString = "Server=localhost;Database=inmovalepablo;User=root;Password=;";

    public RepositorioPropietario()
    {

    }

    public IList<Propietario> GetAll()
    {
        var propietarios = new List<Propietario>();
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"SELECT {nameof(Propietario.Id)}, {nameof(Propietario.Nombre)}, {nameof(Propietario.Apellido)},
             {nameof(Propietario.Dni)}, {nameof(Propietario.Domicilio)}, {nameof(Propietario.Telefono)},
              {nameof(Propietario.Email)} FROM propietarios";

            using (var command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        propietarios.Add(new Propietario
                        {
                            Id = reader.GetInt32(nameof(Propietario.Id)),
                            Nombre = reader.GetString(nameof(Propietario.Nombre)),
                            Apellido = reader.GetString(nameof(Propietario.Apellido)),
                            Dni = reader.GetString(nameof(Propietario.Dni)),
                            Domicilio = reader.GetString(nameof(Propietario.Domicilio)),
                            Telefono = reader.GetString(nameof(Propietario.Telefono)),
                            Email = reader.GetString(nameof(Propietario.Email))
                        });

                    }
                    connection.Close();
                }
            }
        }
        return propietarios;
    }

    public int Update(Propietario propietario)
    {
        int result = -1;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"UPDATE propietarios SET {nameof(Propietario.Nombre)} = @{nameof(Propietario.Nombre)},
            {nameof(Propietario.Apellido)} = @{nameof(Propietario.Apellido)}, {nameof(Propietario.Dni)} = @{nameof(Propietario.Dni)},
            {nameof(Propietario.Domicilio)} = @{nameof(Propietario.Domicilio)}, {nameof(Propietario.Telefono)} = @{nameof(Propietario.Telefono)}, 
            {nameof(Propietario.Email)} = @{nameof(Propietario.Email)} WHERE {nameof(Propietario.Id)} = @{nameof(Propietario.Id)}";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue(nameof(Propietario.Id), propietario.Id);
                command.Parameters.AddWithValue(nameof(Propietario.Nombre), propietario.Nombre);
                command.Parameters.AddWithValue(nameof(Propietario.Apellido), propietario.Apellido);
                command.Parameters.AddWithValue(nameof(Propietario.Dni), propietario.Dni);
                command.Parameters.AddWithValue(nameof(Propietario.Domicilio), propietario.Domicilio);
                command.Parameters.AddWithValue(nameof(Propietario.Telefono), propietario.Telefono);
                command.Parameters.AddWithValue(nameof(Propietario.Email), propietario.Email);
                connection.Open();
                result = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return result;
    }

    public Propietario? GetById(int id)
    {
        Propietario? propietario = null;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"SELECT {nameof(Propietario.Id)}, {nameof(Propietario.Nombre)}, {nameof(Propietario.Apellido)},
            {nameof(Propietario.Dni)}, {nameof(Propietario.Domicilio)}, {nameof(Propietario.Telefono)},
            {nameof(Propietario.Email)} FROM propietarios WHERE {nameof(Propietario.Id)} = @{nameof(Propietario.Id)}";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue(nameof(Propietario.Id), id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        propietario = new Propietario
                        {
                            Id = reader.GetInt32(nameof(Propietario.Id)),
                            Nombre = reader.GetString(nameof(Propietario.Nombre)),
                            Apellido = reader.GetString(nameof(Propietario.Apellido)),
                            Dni = reader.GetString(nameof(Propietario.Dni)),
                            Domicilio = reader.GetString(nameof(Propietario.Domicilio)),
                            Telefono = reader.GetString(nameof(Propietario.Telefono)),
                            Email = reader.GetString(nameof(Propietario.Email))
                        };
                    }
                    connection.Close();
                }

            }
            return propietario;
        }
    }

    public int Create(Propietario propietario)
    {
        int id = 0;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"INSERT INTO propietarios ({nameof(Propietario.Nombre)}, {nameof(Propietario.Apellido)},
            {nameof(Propietario.Dni)}, {nameof(Propietario.Domicilio)}, {nameof(Propietario.Telefono)},
            {nameof(Propietario.Email)}) VALUES (@{nameof(Propietario.Nombre)}, @{nameof(Propietario.Apellido)},
            @{nameof(Propietario.Dni)}, @{nameof(Propietario.Domicilio)}, @{nameof(Propietario.Telefono)},
            @{nameof(Propietario.Email)});
            SELECT LAST_INSERT_ID();";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue(nameof(Propietario.Nombre), propietario.Nombre);
                command.Parameters.AddWithValue(nameof(Propietario.Apellido), propietario.Apellido);
                command.Parameters.AddWithValue(nameof(Propietario.Dni), propietario.Dni);
                command.Parameters.AddWithValue(nameof(Propietario.Domicilio), propietario.Domicilio);
                command.Parameters.AddWithValue(nameof(Propietario.Telefono), propietario.Telefono);
                command.Parameters.AddWithValue(nameof(Propietario.Email), propietario.Email);
                connection.Open();
                id = Convert.ToInt32(command.ExecuteScalar());
                propietario.Id = id;
                connection.Close();
            }
        }
        return id;
    }

    public int Delete(int id)
    {
        int result = -1;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"DELETE FROM propietarios WHERE {nameof(Propietario.Id)} = @{nameof(Propietario.Id)}";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue(nameof(Propietario.Id), id);
                connection.Open();
                result = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return result;
    }

}
