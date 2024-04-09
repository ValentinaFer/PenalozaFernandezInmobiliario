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
            var sql = @$"SELECT i.{nameof(Inmueble.Id)}, i.{nameof(Inmueble.Direccion)}, 
                    i.{nameof(Inmueble.Ambientes)}, i.{nameof(Inmueble.Superficie)}, 
                    i.{nameof(Inmueble.Latitud)}, i.{nameof(Inmueble.Longitud)}, 
                    i.{nameof(Inmueble.PropietarioId)}, p.{nameof(Propietario.Nombre)} AS NombrePropietario,
                    p.{nameof(Propietario.Apellido)} AS ApellidoPropietario
            FROM Inmuebles i 
            INNER JOIN Propietarios p ON i.PropietarioId = p.IdPropietario
            WHERE i.{nameof(Inmueble.Estado)} = 1
            LIMIT @PageSize OFFSET @Offset";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@PageSize", pageSize);
                command.Parameters.AddWithValue("@Offset", (pageNumber - 1) * pageSize);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var inmueble = new Inmueble
                        {
                            Id = reader.GetInt32(nameof(Inmueble.Id)),
                            Direccion = reader.GetString(nameof(Inmueble.Direccion)),
                            Ambientes = reader.GetInt32(nameof(Inmueble.Ambientes)),
                            Superficie = reader.GetInt32(nameof(Inmueble.Superficie)),
                            Latitud = reader.GetDecimal(nameof(Inmueble.Latitud)),
                            Longitud = reader.GetDecimal(nameof(Inmueble.Longitud)),
                            PropietarioId = reader.GetInt32(nameof(Inmueble.PropietarioId)),
                            Duenio = new Propietario
                            {
                                IdPropietario = reader.GetInt32("PropietarioId"),
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
            var sql = "SELECT COUNT(*) FROM Inmuebles WHERE Estado = 1;";

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
            var sql = @$"SELECT i.{nameof(Inmueble.Id)}, i.{nameof(Inmueble.Direccion)}, 
                            i.{nameof(Inmueble.Ambientes)}, i.{nameof(Inmueble.Latitud)}, 
                            i.{nameof(Inmueble.Longitud)}, i.{nameof(Inmueble.PropietarioId)},
                            p.{nameof(Propietario.Nombre)} AS NombrePropietario,
                            p.{nameof(Propietario.Apellido)} AS ApellidoPropietario
                     FROM Inmuebles i 
                     INNER JOIN Propietarios p ON i.{nameof(Inmueble.PropietarioId)} = p.{nameof(Propietario.IdPropietario)}
                     WHERE i.{nameof(Inmueble.Estado)} = 1";

            using (var command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var inmueble = new Inmueble
                        {
                            Id = reader.GetInt32(nameof(Inmueble.Id)),
                            Direccion = reader.GetString(nameof(Inmueble.Direccion)),
                            Ambientes = reader.GetInt32(nameof(Inmueble.Ambientes)),
                            Latitud = reader.GetDecimal(nameof(Inmueble.Latitud)),
                            Longitud = reader.GetDecimal(nameof(Inmueble.Longitud)),
                            PropietarioId = reader.GetInt32(nameof(Inmueble.PropietarioId)),
                            Duenio = new Propietario
                            {
                                IdPropietario = reader.GetInt32("PropietarioId"),
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
        int result = -1;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = $@"UPDATE {nameof(Inmueble)} 
                    SET {nameof(Inmueble.Direccion)} = @{nameof(Inmueble.Direccion)},
                        {nameof(Inmueble.Ambientes)} = @{nameof(Inmueble.Ambientes)},
                        {nameof(Inmueble.Latitud)} = @{nameof(Inmueble.Latitud)},
                        {nameof(Inmueble.Longitud)} = @{nameof(Inmueble.Longitud)},
                        {nameof(Inmueble.PropietarioId)} = @{nameof(Inmueble.PropietarioId)}
                    WHERE {nameof(Inmueble.Id)} = @{nameof(Inmueble.Id)}";
            try
            {
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue($"@{nameof(Inmueble.Direccion)}", inmueble.Direccion);
                    command.Parameters.AddWithValue($"@{nameof(Inmueble.Ambientes)}", inmueble.Ambientes);
                    command.Parameters.AddWithValue($"@{nameof(Inmueble.Latitud)}", inmueble.Latitud);
                    command.Parameters.AddWithValue($"@{nameof(Inmueble.Longitud)}", inmueble.Longitud);
                    command.Parameters.AddWithValue($"@{nameof(Inmueble.PropietarioId)}", inmueble.PropietarioId);
                    command.Parameters.AddWithValue($"@{nameof(Inmueble.Id)}", inmueble.Id);
                    connection.Open();
                    result = command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción, por ejemplo, registrarla o lanzarla nuevamente
                Console.WriteLine($"Error al actualizar el inmueble: {ex.Message}");
            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }
        return result;
    }




    public Inmueble? GetById(int id)
    {
        Inmueble? inmueble = null;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = $@"SELECT {nameof(Inmueble.Id)}, {nameof(Inmueble.Direccion)}, {nameof(Inmueble.Ambientes)}, 
                            {nameof(Inmueble.Superficie)}, {nameof(Inmueble.Latitud)}, {nameof(Inmueble.Longitud)}, 
                            {nameof(Inmueble.PropietarioId)}, p.{nameof(Propietario.Nombre)} AS PropietarioNombre, 
                            p.{nameof(Propietario.Apellido)} AS PropietarioApellido
                     FROM {nameof(Inmueble)} i
                     INNER JOIN {nameof(Propietario)} p ON i.{nameof(Inmueble.PropietarioId)} = p.{nameof(Propietario.IdPropietario)}
                     WHERE i.{nameof(Inmueble.Id)} = @{nameof(Inmueble.Id)}";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Id)}", id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        inmueble = new Inmueble
                        {
                            Id = reader.GetInt32(nameof(Inmueble.Id)),
                            Direccion = reader.GetString(nameof(Inmueble.Direccion)),
                            Ambientes = reader.GetInt32(nameof(Inmueble.Ambientes)),
                            Superficie = reader.GetInt32(nameof(Inmueble.Superficie)),
                            Latitud = reader.GetDecimal(nameof(Inmueble.Latitud)),
                            Longitud = reader.GetDecimal(nameof(Inmueble.Longitud)),
                            PropietarioId = reader.GetInt32(nameof(Inmueble.PropietarioId)),
                            Duenio = new Propietario
                            {
                                IdPropietario = reader.GetInt32("PropietarioId"),
                                Nombre = reader.GetString("NombrePropietario"),
                                Apellido = reader.GetString("ApellidoPropietario"),
                            }

                        };
                    }
                }
            }
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
                     {nameof(inmueble.PropietarioId)}) 
                     VALUES (@Direccion, @Ambientes, @Superficie, @Latitud, @Longitud, @PropietarioId);
                     SELECT LAST_INSERT_ID();";
            try
            {
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Direccion", inmueble.Direccion);
                    command.Parameters.AddWithValue("@Ambientes", inmueble.Ambientes);
                    command.Parameters.AddWithValue("@Superficie", inmueble.Superficie);
                    command.Parameters.AddWithValue("@Latitud", inmueble.Latitud);
                    command.Parameters.AddWithValue("@Longitud", inmueble.Longitud);
                    command.Parameters.AddWithValue("@PropietarioId", inmueble.PropietarioId);
                    connection.Open();
                    id = Convert.ToInt32(command.ExecuteScalar());
                    inmueble.Id = id;
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción, por ejemplo, registrarla o lanzarla nuevamente
                Console.WriteLine($"Error al crear el inmueble: {ex.Message}");
            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }
        return id;
    }


    public int Delete(int id)
    {
        int result = -1;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"UPDATE Inmuebles SET {nameof(Inmueble.Estado)} = 0 WHERE {nameof(Inmueble.Id)} = @{nameof(Inmueble.Id)}";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue(nameof(Inmueble.Id), id);
                connection.Open();
                result = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return result;
    }

}
