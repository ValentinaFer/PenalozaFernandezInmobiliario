using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace PenalozaFernandezInmobiliario.Models;

public class RepositorioPropietario
{
    readonly string ConnectionString = "Server=localhost;Database=inmovalepablo;User=root;Password=;";

    public IList<Propietario> GetAllForIndex(int pageSize, int pageNumber)
    {
        var Propietarios = new List<Propietario>();
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var offset = (pageNumber - 1) * pageSize;
            offset = Math.Max(offset, 0);
            var sql = @$"SELECT {nameof(Propietario.IdPropietario)}, {nameof(Propietario.Nombre)}, {nameof(Propietario.Apellido)},
                             {nameof(Propietario.Dni)}, {nameof(Propietario.Telefono)}, {nameof(Propietario.Email)} 
                             FROM Propietarios 
                             WHERE {nameof(Propietario.Estado)} = 1
                             LIMIT {pageSize} OFFSET {offset};";

            using (var command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Propietarios.Add(new Propietario
                        {
                            IdPropietario = reader.GetInt32(nameof(Propietario.IdPropietario)),
                            Nombre = reader.GetString(nameof(Propietario.Nombre)),
                            Apellido = reader.GetString(nameof(Propietario.Apellido)),
                            Dni = reader.GetString(nameof(Propietario.Dni)),
                            Telefono = reader.GetString(nameof(Propietario.Telefono)),
                            Email = reader.GetString(nameof(Propietario.Email))
                        });

                    }
                    connection.Close();
                }
            }
        }
        return Propietarios;
    }

    public int Update(Propietario Propietario)
    {
        int result = -1;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"UPDATE Propietarios SET {nameof(Propietario.Nombre)} = @{nameof(Propietario.Nombre)},
            {nameof(Propietario.Apellido)} = @{nameof(Propietario.Apellido)}, {nameof(Propietario.Dni)} = @{nameof(Propietario.Dni)},
            {nameof(Propietario.Domicilio)} = @{nameof(Propietario.Domicilio)}, {nameof(Propietario.Telefono)} = @{nameof(Propietario.Telefono)}, 
            {nameof(Propietario.Email)} = @{nameof(Propietario.Email)} WHERE {nameof(Propietario.IdPropietario)} = @{nameof(Propietario.IdPropietario)}";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue(nameof(Propietario.IdPropietario), Propietario.IdPropietario);
                command.Parameters.AddWithValue(nameof(Propietario.Nombre), Propietario.Nombre);
                command.Parameters.AddWithValue(nameof(Propietario.Apellido), Propietario.Apellido);
                command.Parameters.AddWithValue(nameof(Propietario.Dni), Propietario.Dni);
                command.Parameters.AddWithValue(nameof(Propietario.Domicilio), Propietario.Domicilio);
                command.Parameters.AddWithValue(nameof(Propietario.Telefono), Propietario.Telefono);
                command.Parameters.AddWithValue(nameof(Propietario.Email), Propietario.Email);
                connection.Open();
                result = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return result;
    }

    public Propietario? GetById(int id)
    {
        Propietario? Propietario = null;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"SELECT {nameof(Propietario.IdPropietario)}, {nameof(Propietario.Nombre)}, {nameof(Propietario.Apellido)},
            {nameof(Propietario.Dni)}, {nameof(Propietario.Domicilio)}, {nameof(Propietario.Telefono)},
            {nameof(Propietario.Email)} FROM Propietarios WHERE {nameof(Propietario.IdPropietario)} = @{nameof(Propietario.IdPropietario)}";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue(nameof(Propietario.IdPropietario), id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Propietario = new Propietario
                        {
                            IdPropietario = reader.GetInt32(nameof(Propietario.IdPropietario)),
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
            return Propietario;
        }
    }

    public int Create(Propietario Propietario)
    {
        int id = 0;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"INSERT INTO Propietarios ({nameof(Propietario.Nombre)}, {nameof(Propietario.Apellido)},
            {nameof(Propietario.Dni)}, {nameof(Propietario.Domicilio)}, {nameof(Propietario.Telefono)},
            {nameof(Propietario.Email)}) VALUES (@{nameof(Propietario.Nombre)}, @{nameof(Propietario.Apellido)},
            @{nameof(Propietario.Dni)}, @{nameof(Propietario.Domicilio)}, @{nameof(Propietario.Telefono)},
            @{nameof(Propietario.Email)});
            SELECT LAST_INSERT_ID();";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue(nameof(Propietario.Nombre), Propietario.Nombre);
                command.Parameters.AddWithValue(nameof(Propietario.Apellido), Propietario.Apellido);
                command.Parameters.AddWithValue(nameof(Propietario.Dni), Propietario.Dni);
                command.Parameters.AddWithValue(nameof(Propietario.Domicilio), Propietario.Domicilio);
                command.Parameters.AddWithValue(nameof(Propietario.Telefono), Propietario.Telefono);
                command.Parameters.AddWithValue(nameof(Propietario.Email), Propietario.Email);
                connection.Open();
                id = Convert.ToInt32(command.ExecuteScalar());
                Propietario.IdPropietario = id;
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
            var sql = @$"UPDATE Propietarios SET {nameof(Propietario.Estado)} = 0 WHERE {nameof(Propietario.IdPropietario)} = @{nameof(Propietario.IdPropietario)}";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue(nameof(Propietario.IdPropietario), id);
                connection.Open();
                result = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return result;
    }

    public IList<Propietario> GetAll()
    {
        var propietarios = new List<Propietario>();
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = "SELECT IdPropietario, Nombre, Apellido, Dni, Telefono, Email FROM Propietarios WHERE Estado = 1";

            using (var command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        propietarios.Add(new Propietario
                        {
                            IdPropietario = reader.GetInt32(reader.GetOrdinal(nameof(Propietario.IdPropietario))),
                            Nombre = reader.GetString(reader.GetOrdinal(nameof(Propietario.Nombre))),
                            Apellido = reader.GetString(reader.GetOrdinal(nameof(Propietario.Apellido))),
                            Dni = reader.GetString(reader.GetOrdinal(nameof(Propietario.Dni))),
                            Telefono = reader.GetString(reader.GetOrdinal(nameof(Propietario.Telefono))),
                            Email = reader.GetString(reader.GetOrdinal(nameof(Propietario.Email)))
                        });
                    }
                }
            }
        }
        return propietarios;
    }

}