using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;




namespace PenalozaFernandezInmobiliario.Models;

public class RepositorioUsuario
{
    readonly string ConnectionString = "Server=localhost;Database=inmovalepablo;User=root;Password=;";






    public IList<Usuario> GetAllForIndex(int pageSize, int pageNumber, string nombre)
    {
        var usuarios = new List<Usuario>();
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"SELECT u.{nameof(Usuario.IdUsuario)}, u.{nameof(Usuario.Nombre)}, 
                        u.{nameof(Usuario.Apellido)}, u.{nameof(Usuario.Email)}, 
                        u.{nameof(Usuario.Avatar)}, u.{nameof(Usuario.Rol)}
                    FROM Usuarios u
                    WHERE u.{nameof(Usuario.Estado)} = 1";

            if (!string.IsNullOrEmpty(nombre))
            {
                sql += " AND (u.Nombre LIKE @Nombre OR u.Apellido LIKE @Nombre)";
            }

            sql += " LIMIT @PageSize OFFSET @Offset";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@PageSize", pageSize);
                command.Parameters.AddWithValue("@Offset", (pageNumber - 1) * pageSize);

                if (!string.IsNullOrEmpty(nombre))
                {
                    command.Parameters.AddWithValue("@Nombre", $"%{nombre}%");
                }

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var usuario = new Usuario
                        {
                            IdUsuario = reader.GetInt32(nameof(Usuario.IdUsuario)),
                            Nombre = reader.GetString(nameof(Usuario.Nombre)),
                            Apellido = reader.GetString(nameof(Usuario.Apellido)),
                            Email = reader.GetString(nameof(Usuario.Email)),
                            Avatar = reader.GetString(nameof(Usuario.Avatar)),
                            Rol = reader.GetInt32(nameof(Usuario.Rol)),
                        };
                        usuarios.Add(usuario);
                    }
                }
            }
        }
        return usuarios;
    }

    public int getTotalEntries(string nombre = "")
    {
        var result = 0;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = "SELECT COUNT(*) FROM usuarios WHERE Estado = 1";

            if (!string.IsNullOrEmpty(nombre))
            {
                sql += " AND (Nombre LIKE @Nombre OR Apellido LIKE @Nombre)";
            }

            using (var command = new MySqlCommand(sql, connection))
            {
                if (!string.IsNullOrEmpty(nombre))
                {
                    command.Parameters.AddWithValue("@Nombre", $"%{nombre}%");
                }

                connection.Open();
                result = Convert.ToInt32(command.ExecuteScalar());
            }
        }
        return result;
    }




    public int Create(Usuario usuario)
    {
        int id = 0;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = $@"
                INSERT INTO Usuarios ({nameof(usuario.Nombre)}, {nameof(usuario.Apellido)}, {nameof(usuario.Email)},
                                      {nameof(usuario.Clave)}, {nameof(usuario.Avatar)}, {nameof(usuario.Rol)}) 
                VALUES (@Nombre, @Apellido, @Email, @Clave, @Avatar, @Rol);
                SELECT LAST_INSERT_ID();";

            try
            {
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Nombre", MySqlDbType.VarChar).Value = usuario.Nombre;
                    command.Parameters.Add("@Apellido", MySqlDbType.VarChar).Value = usuario.Apellido;
                    command.Parameters.Add("@Email", MySqlDbType.VarChar).Value = usuario.Email;
                    command.Parameters.Add("@Clave", MySqlDbType.VarChar).Value = usuario.Clave;
                    command.Parameters.Add("@Avatar", MySqlDbType.VarChar).Value = usuario.Avatar ?? string.Empty;
                    command.Parameters.Add("@Rol", MySqlDbType.Int32).Value = usuario.Rol;

                    connection.Open();
                    id = Convert.ToInt32(command.ExecuteScalar());
                    usuario.IdUsuario = id;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear el usuario: {ex.Message}");
            }
        }
        return id;
    }
    public int Update(Usuario usuario)
    {
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = $@"UPDATE Usuarios 
                     SET {nameof(Usuario.Nombre)} = @Nombre,
                         {nameof(Usuario.Apellido)} = @Apellido,
                         {nameof(Usuario.Email)} = @Email,
                         {nameof(Usuario.Clave)} = @Clave,
                         {nameof(Usuario.Avatar)} = @Avatar,
                         {nameof(Usuario.Rol)} = @Rol
                     WHERE {nameof(Usuario.IdUsuario)} = @IdUsuario;";
            try
            {
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Nombre", MySqlDbType.VarChar).Value = usuario.Nombre;
                    command.Parameters.Add("@Apellido", MySqlDbType.VarChar).Value = usuario.Apellido;
                    command.Parameters.Add("@Email", MySqlDbType.VarChar).Value = usuario.Email;
                    command.Parameters.Add("@Clave", MySqlDbType.VarChar).Value = usuario.Clave;
                    command.Parameters.Add("@Avatar", MySqlDbType.VarChar).Value = usuario.Avatar;
                    command.Parameters.Add("@Rol", MySqlDbType.Int32).Value = usuario.Rol;
                    command.Parameters.Add("@IdUsuario", MySqlDbType.Int32).Value = usuario.IdUsuario;

                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar el usuario: {ex.Message}");
                throw;
            }
        }
    }


    public Usuario GetById(int id)
    {
        Usuario usuario = null;
        try
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = @"SELECT u.IdUsuario, u.Nombre, u.Apellido, u.Email, u.Clave, u.Avatar, u.Rol
                        FROM Usuarios u
                        WHERE u.IdUsuario = @IdUsuario";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdUsuario", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuario
                            {
                                IdUsuario = reader.GetInt32("IdUsuario"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Email = reader.GetString("Email"),
                                Clave = reader.GetString("Clave"),
                                Avatar = reader.IsDBNull(reader.GetOrdinal("Avatar")) ? "" : reader.GetString("Avatar"),
                                Rol = reader.GetInt32("Rol")
                            };
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al buscar el usuario: {ex.Message}");
        }
        return usuario;
    }


    public int Delete(int id)
    {
        int result = -1;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"UPDATE usuarios 
                     SET {nameof(Usuario.Estado)} = 0  
                     WHERE {nameof(Usuario.IdUsuario)} = @{nameof(Usuario.IdUsuario)};";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Usuario.IdUsuario)}", id);
                connection.Open();
                result = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return result;
    }



    public Usuario ObtenerUsuarioPorEmail(string email)
    {
        Usuario usuario = null;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @"SELECT IdUsuario, Nombre, Apellido, Email, Clave,  Avatar, Rol 
                    FROM Usuarios 
                    WHERE Email = @Email AND Estado = 1"; // Considera el estado del usuario

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Email", email);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        usuario = new Usuario
                        {
                            IdUsuario = reader.GetInt32("IdUsuario"),
                            Nombre = reader.GetString("Nombre"),
                            Apellido = reader.GetString("Apellido"),
                            Email = reader.GetString("Email"),
                            Clave = reader.GetString("Clave"),

                            Avatar = reader.IsDBNull(reader.GetOrdinal("Avatar")) ? null : reader.GetString("Avatar"),
                            Rol = reader.GetInt32("Rol")
                        };
                    }
                }
            }
        }
        return usuario;
    }

}