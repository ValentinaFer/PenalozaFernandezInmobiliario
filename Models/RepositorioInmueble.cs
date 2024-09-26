using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;
namespace PenalozaFernandezInmobiliario.Models;

public class RepositorioInmueble
{

    readonly string ConnectionString = "Server=localhost;Database=inmovalepablo;User=root;Password=;";

    //para validar columnas de busqueda en datatables
    private readonly Dictionary<string, string> allowedColumns = new Dictionary<string, string>{
            {"IdInmueble", $"i.{nameof(Inmueble.IdInmueble)}"},
            {"Tipo", $"tipo.{nameof(TipoInmueble.Tipo)}"},
            {"direccion", $"i.{nameof(Inmueble.Direccion)}"},
            {"duenio", $"p.{nameof(Propietario.Nombre)}"},
            {"precio", $"i.{nameof(Inmueble.Precio)}"},
        };

    //trae solo data necesaria para mostrar en index
    public IList<Inmueble> GetAllForIndex(int pageSize, int pageNumber, string estado)
    {
        var inmuebles = new List<Inmueble>();
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"SELECT i.{nameof(Inmueble.IdInmueble)}, i.{nameof(Inmueble.Direccion)}, 
                        i.{nameof(Inmueble.Ambientes)}, i.{nameof(Inmueble.Superficie)}, 
                        i.{nameof(Inmueble.Latitud)}, i.{nameof(Inmueble.Longitud)}, 
                        i.{nameof(Inmueble.Precio)}, i.{nameof(Inmueble.IdPropietario)}, 
                        i.{nameof(Inmueble.Estado)},
                        p.{nameof(Propietario.Nombre)} AS NombrePropietario,
                        p.{nameof(Propietario.Apellido)} AS ApellidoPropietario
                    FROM Inmuebles i 
                    INNER JOIN Propietarios p ON i.IdPropietario = p.IdPropietario
                    WHERE i.{nameof(Inmueble.Estado)} = @Estado   
                    LIMIT @PageSize OFFSET @Offset";

            using (var command = new MySqlCommand(sql, connection))
            {
                // Parámetro dinámico para el filtro del estado
                command.Parameters.AddWithValue("@Estado", estado);
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
                            Estado = reader.GetString(nameof(Inmueble.Estado)),
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
                     WHERE i.{nameof(Inmueble.Estado)} = 'Disponible'";

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
                         {nameof(Inmueble.Precio)} = @Precio,
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
                    command.Parameters.Add("@Precio", MySqlDbType.Decimal).Value = inmueble.Precio;
                    command.Parameters.Add("@IdPropietario", MySqlDbType.Int32).Value = inmueble.IdPropietario;
                    command.Parameters.Add("@IdInmueble", MySqlDbType.Int32).Value = inmueble.IdInmueble;

                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error al actualizar el inmueble: {ex.Message}");
                throw;
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
                              i.Precio, i.IdPropietario, p.Nombre AS Nombre, p.Apellido AS Apellido
                        FROM Inmuebles i
                        INNER JOIN Propietarios p ON i.IdPropietario = p.IdPropietario
                        WHERE i.IdInmueble = @IdInmueble";

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
                                Precio = reader.GetDecimal("Precio"),
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

                Console.WriteLine($"Error al crear el inmueble: {ex.Message}");

            }
        }
        return id;
    }



    public int Delete(int id)
    {
        int result = -1;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"UPDATE Inmuebles 
                     SET {nameof(Inmueble.Estado)} = 'No disponible'  
                     WHERE {nameof(Inmueble.IdInmueble)} = @{nameof(Inmueble.IdInmueble)};";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Inmueble.IdInmueble)}", id);
                connection.Open();
                result = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return result;
    }



    public Inmueble? GetAllDetailsById(int id)
    {
        Inmueble? inmueble = null;
        try
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = @$"SELECT i.*, p.Nombre AS Nombre, p.Apellido AS Apellido, ti.{nameof(TipoInmueble.Tipo)}
                        FROM Inmuebles i
                        JOIN Propietarios p ON i.IdPropietario = p.IdPropietario AND p.estado = 1
                        JOIN TiposInmueble ti ON i.{nameof(Inmueble.IdTipoInmueble)} = ti.id
                        WHERE i.IdInmueble = @IdInmueble AND i.Estado = 'Disponible';";

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
                                IdInmueble = reader.GetInt32(nameof(Inmueble.IdInmueble)),
                                Direccion = reader.GetString(nameof(Inmueble.Direccion)),
                                Ambientes = reader.GetInt32(nameof(Inmueble.Ambientes)),
                                Superficie = reader.GetInt32(nameof(Inmueble.Superficie)),
                                Latitud = reader.GetDecimal(nameof(Inmueble.Latitud)),
                                Longitud = reader.GetDecimal(nameof(Inmueble.Longitud)),
                                Precio = reader.GetDecimal(nameof(Inmueble.Precio)),
                                Duenio = new Propietario
                                {
                                    IdPropietario = reader.GetInt32(nameof(Inmueble.IdPropietario)),
                                    Nombre = reader.GetString(nameof(Propietario.Nombre)),
                                    Apellido = reader.GetString(nameof(Propietario.Apellido))
                                },

                                Tipo = new TipoInmueble 
                                {
                                    Tipo = reader.GetString(nameof(TipoInmueble.Tipo))
                                }
                            };
                        }
                        connection.Close();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al buscar el inmueble: {ex.Message}");
            throw;
        }
        return inmueble;
    }


    //Para obtener todos los inmuebles disponibles dentro de un rango de fechas
    public List<Inmueble> GetAllInRange(DateTime startDate, DateTime endDate, string searchValue, string sortColumn, string sortDirection, int skip, int pageSize)
    {
        List<Inmueble> inmuebles = new List<Inmueble>();

        if (allowedColumns.TryGetValue(sortColumn, out string allowedColumn)){
            sortColumn = allowedColumn;
        } else {
            sortColumn = $"i.{nameof(Inmueble.IdInmueble)}";
        }

        if (!sortDirection.ToLower().Equals("asc") && !sortDirection.ToLower().Equals("desc")){
            sortDirection = "asc";
        }

        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"SELECT 
            i.{nameof(Inmueble.IdInmueble)}, i.{nameof(Inmueble.Precio)}, i.{nameof(Inmueble.Direccion)}, i.{nameof(Inmueble.Ambientes)},
            tipo.{nameof(TipoInmueble.Tipo)},
            p.{nameof(Propietario.IdPropietario)}, p.{nameof(Propietario.Nombre)}, p.{nameof(Propietario.Apellido)}
            
            FROM inmuebles i
            LEFT JOIN contratos c
                ON i.{nameof(Inmueble.IdInmueble)} = c.{nameof(Contrato.InmuebleId)} 
                AND c.{nameof(Contrato.Estado)} = 1
                AND NOT (
                    (IFNULL(c.{nameof(Contrato.FechaFinalizacion)}, c.{nameof(Contrato.FechaHasta)}) < @startDate OR c.{nameof(Contrato.FechaDesde)} > @endDate)
                    )
                AND (c.{nameof(Contrato.FechaFinalizacion)} IS NULL OR (c.{nameof(Contrato.FechaFinalizacion)} >= c.{nameof(Contrato.FechaHasta)}))
            JOIN tiposinmueble tipo
                ON i.{nameof(Inmueble.IdTipoInmueble)} = tipo.id 

            JOIN propietarios p
                ON p.{nameof(Propietario.IdPropietario)} = i.{nameof(Inmueble.IdPropietario)} 
                AND p.{nameof(Propietario.Estado)} = 1
            WHERE i.{nameof(Inmueble.Estado)} = 'Disponible'
                AND (i.{nameof(Inmueble.Direccion)} LIKE @searchValue OR CONCAT(p.{nameof(Propietario.Apellido)}, ' ', p.{nameof(Propietario.Nombre)}) LIKE @searchValue)
            
            GROUP BY i.{nameof(Inmueble.IdInmueble)}
            HAVING COUNT(c.{nameof(Contrato.Id)}) = 0
            ORDER BY {sortColumn} {sortDirection}
            LIMIT @skip, @pageSize;";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@startDate", startDate);
                command.Parameters.AddWithValue("@endDate", endDate);
                command.Parameters.AddWithValue("@searchValue", $"%{searchValue}%");
                command.Parameters.AddWithValue("@skip", skip);
                command.Parameters.AddWithValue("@pageSize", pageSize);

                Console.WriteLine(sql);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        inmuebles.Add(new Inmueble
                        {
                            
                            IdInmueble = reader.GetInt32(nameof(Inmueble.IdInmueble)),
                            Precio = reader.GetDecimal(nameof(Inmueble.Precio)),
                            Direccion = reader.GetString(nameof(Inmueble.Direccion)),
                            Ambientes = reader.GetInt32(nameof(Inmueble.Ambientes)),
                            Tipo = new TipoInmueble
                            {
                                Tipo = reader.GetString(nameof(TipoInmueble.Tipo))
                            },
                            Duenio = new Propietario
                            {
                                IdPropietario = reader.GetInt32(nameof(Propietario.IdPropietario)),
                                Nombre = reader.GetString(nameof(Propietario.Nombre)),
                                Apellido = reader.GetString(nameof(Propietario.Apellido))
                            }
                        });
                    }

                    connection.Close();
                }
            }

        }

        return inmuebles;
    }

    public int GetCountGetAllInRangeFiltered(DateTime startDate, DateTime endDate, string searchValue){

        using (var connection = new MySqlConnection(ConnectionString)){
            var sql = @$"
                SELECT COUNT(DISTINCT i.{nameof(Inmueble.IdInmueble)})

                FROM inmuebles i
                LEFT JOIN contratos c
                    ON i.{nameof(Inmueble.IdInmueble)} = c.{nameof(Contrato.InmuebleId)} 
                    AND c.estado = 1
                    AND NOT (
                    (IFNULL(c.{nameof(Contrato.FechaFinalizacion)}, c.{nameof(Contrato.FechaHasta)}) < @startDate OR c.{nameof(Contrato.FechaDesde)} > @endDate)
                    )
                    AND (c.{nameof(Contrato.FechaFinalizacion)} IS NULL OR (c.{nameof(Contrato.FechaFinalizacion)} >= c.{nameof(Contrato.FechaHasta)}))
                JOIN tiposinmueble tipo
                    ON i.{nameof(Inmueble.IdTipoInmueble)} = tipo.id 
                JOIN propietarios p
                    ON p.{nameof(Propietario.IdPropietario)} = i.{nameof(Inmueble.IdPropietario)} 
                
                WHERE i.{nameof(Inmueble.Estado)} = 'Disponible'

                AND (i.{nameof(Inmueble.Direccion)} LIKE @searchValue OR p.{nameof(Propietario.Nombre)} LIKE @searchValue)
                GROUP BY i.{nameof(Inmueble.IdInmueble)}
                HAVING COUNT(c.{nameof(Contrato.Id)}) = 0";

            using (var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue("@startDate", startDate);
                command.Parameters.AddWithValue("@endDate", endDate);
                command.Parameters.AddWithValue("@searchValue", $"%{searchValue}%");
                connection.Open();
                var result = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
                return result;
            }
        }
    }

    public bool CheckDisponibilidad(int inmuebleId, DateTime startDate, DateTime endDate)
    {
        var disponible = false;
        /*SELECT IFNULL(COUNT(contratos.id),0) 
            FROM `inmuebles`
            LEFT JOIN contratos ON 
            contratos.inmuebleId = inmuebles.idInmueble 
            AND contratos.estado = 1
			AND (contratos.fechaHasta >= "2025-07-02" AND contratos.fechaDesde <= "2025-07-20") 
            AND (contratos.FechaFinalizacion IS NULL OR (contratos.FechaFinalizacion >= contratos.FechaHasta)) 
            WHERE inmuebles.idInmueble = 32 AND inmuebles.estado = 'Disponible';*/
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"SELECT IFNULL(COUNT(contratos.id),0) 
            FROM `inmuebles`
            LEFT JOIN contratos ON 
            contratos.inmuebleId = inmuebles.idInmueble 
            AND contratos.estado = 1
            AND (contratos.fechaHasta >= @startDate AND contratos.fechaDesde <= @endDate) 
            AND (contratos.fechaFinalizacion IS NULL OR (contratos.FechaFinalizacion >= contratos.fechaHasta))
            WHERE inmuebles.idInmueble = @inmuebleId AND inmuebles.estado = 'Disponible';";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@inmuebleId", inmuebleId);
                command.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"));
                connection.Open();
                var result = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
                if (result == 0)
                {
                    disponible = true;
                }

            };

        }
        return disponible;
    }

    
}

