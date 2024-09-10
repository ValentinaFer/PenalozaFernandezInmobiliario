using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;
namespace PenalozaFernandezInmobiliario.Models;

public class RepositorioInmueble
{

    readonly string ConnectionString = "Server=localhost;Database=inmovalepablo;User=root;Password=;";



    //trae solo data necesaria para mostrar en index
    public IList<Inmueble> GetAllForIndex(int pageSize, int pageNumber)
    {
        var inmuebles = new List<Inmueble>();
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"SELECT i.{nameof(Inmueble.IdInmueble)}, i.{nameof(Inmueble.Direccion)}, 
                        i.{nameof(Inmueble.Ambientes)}, i.{nameof(Inmueble.Superficie)}, 
                        i.{nameof(Inmueble.Latitud)}, i.{nameof(Inmueble.Longitud)}, i.{nameof(Inmueble.Precio)}, 
                        i.{nameof(Inmueble.IdPropietario)}, i.{nameof(Inmueble.Estado)},  -- <--- Agregar aquí Estado
                        p.{nameof(Propietario.Nombre)} AS NombrePropietario,
                        p.{nameof(Propietario.Apellido)} AS ApellidoPropietario
                    FROM Inmuebles i 
                    INNER JOIN Propietarios p ON i.IdPropietario = p.IdPropietario
                    WHERE i.{nameof(Inmueble.Estado)} = @Estado   
                    LIMIT @PageSize OFFSET @Offset";

            using (var command = new MySqlCommand(sql, connection))
            {
                // Parámetro para el filtro del estado
                command.Parameters.AddWithValue("@Estado", "Disponible"); // Cambia "Disponible" por el valor deseado
                                                                          // command.Parameters.AddWithValue("@Estado", "No disponible");
                                                                          // Parámetros de paginación
                command.Parameters.AddWithValue("@PageSize", pageSize);
                command.Parameters.AddWithValue("@Offset", (pageNumber - 1) * pageSize);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var inmueble = new Inmueble
                        {
                            IdInmueble = reader.GetInt32(nameof(Inmueble.IdInmueble)),
                            Direccion = reader.GetString(nameof(Inmueble.Direccion)),
                            Ambientes = reader.GetInt32(nameof(Inmueble.Ambientes)),
                            Superficie = reader.GetInt32(nameof(Inmueble.Superficie)),
                            Latitud = reader.GetDecimal(nameof(Inmueble.Latitud)),
                            Longitud = reader.GetDecimal(nameof(Inmueble.Longitud)),
                            Precio = reader.GetDecimal(nameof(Inmueble.Precio)),
                            IdPropietario = reader.GetInt32(nameof(Inmueble.IdPropietario)),
                            Estado = reader.GetString(nameof(Inmueble.Estado)),  // Ahora puedes obtener Estado
                            Duenio = new Propietario
                            {
                                IdPropietario = reader.GetInt32("IdPropietario"),
                                Nombre = reader.GetString("NombrePropietario"),
                                Apellido = reader.GetString("ApellidoPropietario"),
                            }
                        };
                        inmuebles.Add(inmueble);
                    }
                }
            }
        }
        return inmuebles;
    }


    public int getTotalEntries()
    {
        var result = 0;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = "SELECT COUNT(*) FROM Inmuebles WHERE Estado = Disponible;";

            using (var command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                result = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
                return result;
            }
        }
    }

    public IList<Inmueble> GetAll()
    {
        var inmuebles = new List<Inmueble>();
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"SELECT i.{nameof(Inmueble.IdInmueble)}, i.{nameof(Inmueble.Direccion)}, 
                            i.{nameof(Inmueble.Ambientes)}, i.{nameof(Inmueble.Latitud)}, 
                            i.{nameof(Inmueble.Longitud)}, i.{nameof(Inmueble.IdPropietario)},
                            p.{nameof(Propietario.Nombre)} AS NombrePropietario,
                            p.{nameof(Propietario.Apellido)} AS ApellidoPropietario
                     FROM Inmuebles i 
                     INNER JOIN Propietarios p ON i.{nameof(Inmueble.IdPropietario)} = p.{nameof(Propietario.IdPropietario)}
                     WHERE i.{nameof(Inmueble.Estado)} = Disponible";

            using (var command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var inmueble = new Inmueble
                        {
                            IdInmueble = reader.GetInt32(nameof(Inmueble.IdInmueble)),
                            Direccion = reader.GetString(nameof(Inmueble.Direccion)),
                            Ambientes = reader.GetInt32(nameof(Inmueble.Ambientes)),
                            Latitud = reader.GetDecimal(nameof(Inmueble.Latitud)),
                            Longitud = reader.GetDecimal(nameof(Inmueble.Longitud)),
                            IdPropietario = reader.GetInt32(nameof(Inmueble.IdPropietario)),
                            Duenio = new Propietario
                            {
                                IdPropietario = reader.GetInt32("IdPropietario"),
                                Nombre = reader.GetString("NombrePropietario"),
                                Apellido = reader.GetString("ApellidoPropietario"),
                            }
                        };
                        inmuebles.Add(inmueble);
                    }
                }
            }
        }
        return inmuebles;
    }


    public int Update(Inmueble inmueble)
    {
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = $@"UPDATE Inmuebles 
                     SET {nameof(Inmueble.Direccion)} = @Direccion,
                         {nameof(Inmueble.Ambientes)} = @Ambientes,
                         {nameof(Inmueble.Latitud)} = @Latitud,
                         {nameof(Inmueble.Longitud)} = @Longitud,
                         {nameof(Inmueble.IdPropietario)} = @IdPropietario
                     WHERE {nameof(Inmueble.IdInmueble)} = @IdInmueble;";
            try
            {
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Direccion", MySqlDbType.VarChar).Value = inmueble.Direccion;
                    command.Parameters.Add("@Ambientes", MySqlDbType.Int32).Value = inmueble.Ambientes;
                    command.Parameters.Add("@Latitud", MySqlDbType.Decimal).Value = inmueble.Latitud;
                    command.Parameters.Add("@Longitud", MySqlDbType.Decimal).Value = inmueble.Longitud;
                    command.Parameters.Add("@IdPropietario", MySqlDbType.Int32).Value = inmueble.IdPropietario;
                    command.Parameters.Add("@IdInmueble", MySqlDbType.Int32).Value = inmueble.IdInmueble;

                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // Registrar la excepción o lanzarla nuevamente para manejarla en el controlador
                Console.WriteLine($"Error al actualizar el inmueble: {ex.Message}");
                throw; // Lanzar la excepción nuevamente para que el controlador pueda manejarla
            }
        }
    }




    public Inmueble GetById(int id)
    {
        Inmueble inmueble = null;
        try
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = @"SELECT i.IdInmueble, i.Direccion, i.Ambientes, i.Superficie, i.Latitud, i.Longitud,i.Uso, 
                               i.IdPropietario, p.Nombre AS Nombre, p.Apellido AS Apellido
                        FROM Inmuebles i
                        INNER JOIN Propietarios p ON i.IdPropietario = p.IdPropietario
                        WHERE i.IdInmueble = @IdInmueble AND i.Estado = Disponible;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdInmueble", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            inmueble = new Inmueble
                            {
                                IdInmueble = reader.GetInt32("IdInmueble"),
                                Direccion = reader.GetString("Direccion"),
                                Ambientes = reader.GetInt32("Ambientes"),
                                Superficie = reader.GetInt32("Superficie"),
                                Latitud = reader.GetDecimal("Latitud"),
                                Longitud = reader.GetDecimal("Longitud"),
                                Uso = reader.GetString("Uso"),
                                IdPropietario = reader.GetInt32("IdPropietario"),
                                Duenio = new Propietario
                                {
                                    Nombre = reader.GetString("Nombre"),
                                    Apellido = reader.GetString("Apellido")
                                }
                            };
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Manejar la excepción, por ejemplo, registrarla o lanzarla nuevamente
            Console.WriteLine($"Error al buscar el inmueble: {ex.Message}");
        }
        return inmueble;
    }




    public int Create(Inmueble inmueble)
    {
        int id = 0;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = $@"INSERT INTO Inmuebles ({nameof(inmueble.Direccion)}, {nameof(inmueble.Ambientes)},
                 {nameof(inmueble.Superficie)}, {nameof(inmueble.Latitud)}, {nameof(inmueble.Longitud)},
                 {nameof(inmueble.IdPropietario)}, {nameof(inmueble.IdTipoInmueble)}, {nameof(inmueble.Uso)}, {nameof(inmueble.Precio)}) 
                 VALUES (@Direccion, @Ambientes, @Superficie, @Latitud, @Longitud, @IdPropietario, @IdTipoInmueble, @Uso, @Precio);
                 SELECT LAST_INSERT_ID();";

            try
            {
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Direccion", MySqlDbType.VarChar).Value = inmueble.Direccion;
                    command.Parameters.Add("@Ambientes", MySqlDbType.Int32).Value = inmueble.Ambientes;
                    command.Parameters.Add("@Superficie", MySqlDbType.Decimal).Value = inmueble.Superficie;
                    command.Parameters.Add("@Latitud", MySqlDbType.Decimal).Value = inmueble.Latitud;
                    command.Parameters.Add("@Longitud", MySqlDbType.Decimal).Value = inmueble.Longitud;
                    command.Parameters.Add("@IdPropietario", MySqlDbType.Int32).Value = inmueble.IdPropietario;
                    command.Parameters.Add("@IdTipoInmueble", MySqlDbType.Int32).Value = inmueble.IdTipoInmueble;
                    command.Parameters.Add("@Uso", MySqlDbType.VarChar).Value = inmueble.Uso;
                    command.Parameters.Add("@Precio", MySqlDbType.Decimal).Value = inmueble.Precio;

                    connection.Open();
                    id = Convert.ToInt32(command.ExecuteScalar());
                    inmueble.IdInmueble = id;
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción de forma más robusta
                Console.WriteLine($"Error al crear el inmueble: {ex.Message}");
                // Considerar lanzar la excepción o usar un sistema de logging
            }
        }
        return id;
    }



    public int Delete(int id)
    {
        int result = -1;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"UPDATE Inmuebles SET {nameof(Inmueble.Estado)} = 0 WHERE {nameof(Inmueble.IdInmueble)} = @{nameof(Inmueble.IdInmueble)};";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue(nameof(Inmueble.IdInmueble), id);
                connection.Open();
                result = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return result;
    }

}
