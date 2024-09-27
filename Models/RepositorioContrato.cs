using System.Configuration;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace PenalozaFernandezInmobiliario.Models;

public class RepositorioContrato
{

    readonly string ConnectionString = "Server=localhost;Database=inmovalepablo;User=root;Password=;";

    public RepositorioContrato()
    {

    }

    public int Update(int idContrato, int inquilinoId, int inmuebleId, DateTime dFrom, DateTime dTo)
    {
        int result = -1;
        try
        {

            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = @$"UPDATE contratos SET 
                {nameof(Contrato.InquilinoId)} = @inquilinoId,
                {nameof(Contrato.InmuebleId)} = @inmuebleId,
                {nameof(Contrato.FechaDesde)} = @dFrom,
                {nameof(Contrato.FechaHasta)} = @dTo,
                {nameof(Contrato.Monto)} = (SELECT {nameof(Inmueble.Precio)} FROM inmuebles WHERE {nameof(Inmueble.IdInmueble)} = @inmuebleId)
                WHERE {nameof(Contrato.Id)} = @id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", idContrato);
                    command.Parameters.AddWithValue("@inquilinoId", inquilinoId);
                    command.Parameters.AddWithValue("@inmuebleId", inmuebleId);
                    command.Parameters.AddWithValue("@dFrom", dFrom);
                    command.Parameters.AddWithValue("@dTo", dTo);
                    connection.Open();
                    result = command.ExecuteNonQuery();
                    connection.Close();
                }
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw new Exception(ex.Message);
        }
        return result;
    }

    public int UpdateMejorado(int idContrato, int inquilinoId, int inmuebleId, DateTime dFrom, DateTime dTo)
    {
        int result = -1;
        try
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var sql = @$"UPDATE contratos SET 
                    {nameof(Contrato.InquilinoId)} = @inquilinoId,
                    {nameof(Contrato.InmuebleId)} = @inmuebleId,
                    {nameof(Contrato.FechaDesde)} = @dFrom,
                    {nameof(Contrato.FechaHasta)} = @dTo,
                    {nameof(Contrato.Monto)} = (SELECT {nameof(Inmueble.Precio)} FROM inmuebles WHERE {nameof(Inmueble.IdInmueble)} = @inmuebleId)
                    WHERE {nameof(Contrato.Id)} = @id;";
                        using (var command = new MySqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@id", idContrato);
                            command.Parameters.AddWithValue("@inquilinoId", inquilinoId);
                            command.Parameters.AddWithValue("@inmuebleId", inmuebleId);
                            command.Parameters.AddWithValue("@dFrom", dFrom);
                            command.Parameters.AddWithValue("@dTo", dTo);

                            result = command.ExecuteNonQuery();

                            if (result <= 0)
                            {
                                throw new Exception("No rows updated for the contract.");
                            }
                        }

                        //Elimino pagos para evitar poder habilitar pagos con montos de propiedades anteriores o con montos viejos
                        var sql2 = @$"DELETE FROM pagos WHERE {nameof(Pago.IdContrato)} = @id;";
                        using (var command2 = new MySqlCommand(sql2, connection))
                        {
                            command2.Parameters.AddWithValue("@id", idContrato);
                            int result2 = command2.ExecuteNonQuery();

                            Console.WriteLine($"Deleted {result2} pagos from {idContrato}."); 
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine(ex.Message);
                        throw;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw new Exception(ex.Message);
        }

        return result;
    }

    public int Reactivar(int idContrato){
        int result = -1;
        try
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = @$"UPDATE contratos SET 
                {nameof(Contrato.Estado)} = 1
                WHERE {nameof(Contrato.Id)} = @id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", idContrato);
                    connection.Open();
                    result = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return result;
        } 
        catch (Exception ex) {
            Console.WriteLine(ex.Message);
            throw new Exception(ex.Message);
        }
    }

    public int UpdateInquilino(int idContrato, int idInquilino)
    {
        int result = -1;
        try
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = @$"UPDATE contratos SET 
                {nameof(Contrato.InquilinoId)} = @idInquilino
                WHERE {nameof(Contrato.Id)} = @id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", idContrato);
                    command.Parameters.AddWithValue("@idInquilino", idInquilino);
                    connection.Open();
                    result = command.ExecuteNonQuery();
                    connection.Close();
                }
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw new Exception(ex.Message);
        }
        return result;
    }

    public int CambiarAVigente(int idContrato)
    {
        int result = -1;
        try
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = @$"UPDATE contratos SET
                {nameof(Contrato.FechaFinalizacion)} = NULL
                WHERE {nameof(Contrato.Id)} = @id;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", idContrato);
                    connection.Open();
                    result = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw new Exception(ex.Message);
        }
        return result;
    }

    public int CancelarContrato(int id)
    {
        int result = -1;
        try
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = @$"UPDATE contratos SET
                {nameof(Contrato.FechaFinalizacion)} = IF(CURRENT_DATE() >= {nameof(Contrato.FechaHasta)}, DATE_SUB({nameof(Contrato.FechaHasta)}, INTERVAL 1 DAY), CURRENT_DATE())
                WHERE {nameof(Contrato.Id)} = @id;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    result = command.ExecuteNonQuery();
                    connection.Close();
                }
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw new Exception(ex.Message);
        }
        return result;
    }
    public int FinalizarContrato(int id)
    {
        int result = -1;
        try
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = @$"UPDATE contratos SET
                {nameof(Contrato.FechaFinalizacion)} = {nameof(Contrato.FechaHasta)}
                WHERE {nameof(Contrato.Id)} = @id;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    result = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw ex;
        }
        return result;
    }

    public IList<Contrato> GetAllForIndex(int pageSize, int pageNumber, string todos = "activos")
    {
        var contratos = new List<Contrato>();
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var traerEstado = "";
            if (todos.Equals("activos")){
                traerEstado = @$"WHERE c.{nameof(Contrato.Estado)} = 1";
            } else if (todos.Equals("inactivos")){
                traerEstado = @$"WHERE c.{nameof(Contrato.Estado)} = 0";
            }
            var sql = @$"SELECT c.{nameof(Contrato.Id)}, c.{nameof(Contrato.InmuebleId)}, c.{nameof(Contrato.Monto)}, c.{nameof(Contrato.FechaDesde)}, c.{nameof(Contrato.FechaHasta)}, c.{nameof(Contrato.FechaFinalizacion)}, c.{nameof(Contrato.Estado)},
            inq.{nameof(Inquilino.Id)}, inq.{nameof(Inquilino.Apellido)},  inq.{nameof(Inquilino.Nombre)},  inq.{nameof(Inquilino.Dni)},
            p.{nameof(Propietario.IdPropietario)}, p.{nameof(Propietario.Apellido)} AS propApellido, p.{nameof(Propietario.Nombre)} AS propNombre, p.{nameof(Propietario.Dni)} AS propDni,
            inm.{nameof(Inmueble.IdInmueble)}, inm.{nameof(Inmueble.Direccion)}, inm.{nameof(Inmueble.Longitud)}, inm.{nameof(Inmueble.Latitud)}
            FROM Contratos c
            INNER JOIN Inmuebles inm ON c.{nameof(Contrato.InmuebleId)} = inm.{nameof(Inmueble.IdInmueble)}
            INNER JOIN Propietarios p ON inm.{nameof(Inmueble.IdPropietario)} = p.{nameof(Propietario.IdPropietario)}
            INNER JOIN {Inquilino.GetTableName()} inq ON c.{nameof(Contrato.InquilinoId)} = inq.{nameof(Inquilino.Id)}
            {traerEstado}
            LIMIT {pageSize} OFFSET {(pageNumber - 1) * pageSize};";

            using (var command = new MySqlCommand(sql, connection))
            {

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        contratos.Add(new Contrato
                        {
                            Id = reader.GetInt32(nameof(Contrato.Id)),
                            FechaDesde = reader.GetDateTime(nameof(Contrato.FechaDesde)),
                            FechaHasta = reader.GetDateTime(nameof(Contrato.FechaHasta)),
                            FechaFinalizacion = reader[nameof(Contrato.FechaFinalizacion)] == DBNull.Value ? null : reader.GetDateTime(nameof(Contrato.FechaFinalizacion)),
                            Monto = reader.GetDecimal(nameof(Contrato.Monto)),
                            Estado = reader.GetBoolean(nameof(Contrato.Estado)),
                            InmuebleId = reader.GetInt32(nameof(Contrato.InmuebleId)),
                            Inquilino = new Inquilino
                            {
                                Id = reader.GetInt32(nameof(Inquilino.Id)),
                                Nombre = reader.GetString(nameof(Inquilino.Nombre)),
                                Apellido = reader.GetString(nameof(Inquilino.Apellido)),
                                Dni = reader.GetString(nameof(Inquilino.Dni)),
                            },
                            Inmueble = new Inmueble
                            {
                                IdInmueble = reader.GetInt32(nameof(Inmueble.IdInmueble)),
                                Direccion = reader.GetString(nameof(Inmueble.Direccion)),
                                Longitud = reader.GetDecimal(nameof(Inmueble.Longitud)),
                                Latitud = reader.GetDecimal(nameof(Inmueble.Latitud)),
                                Duenio = new Propietario
                                {
                                    IdPropietario = reader.GetInt32(nameof(Propietario.IdPropietario)),
                                    Apellido = reader.GetString("propApellido"),
                                    Nombre = reader.GetString("propNombre"),
                                    Dni = reader.GetString("propDni"),
                                }
                            }

                        });

                    }
                    connection.Close();
                }
            }
        }
        return contratos;
    }

    public Contrato? GetByIdEdit(int id)
    {
        Contrato? c = null;
        return c;
    }

    public Contrato? GetById(int id) //trae contrato sin filtrar por estado o finalizacion
    {
        Contrato? c = null;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"SELECT 
            c.{nameof(Contrato.Id)} as idContrato, c.{nameof(Contrato.InmuebleId)}, c.{nameof(Contrato.Monto)}, c.{nameof(Contrato.FechaDesde)}, c.{nameof(Contrato.FechaHasta)}, c.{nameof(Contrato.FechaFinalizacion)}, c.{nameof(Contrato.Estado)},
            inq.{nameof(Inquilino.Id)}, inq.{nameof(Inquilino.Apellido)},  inq.{nameof(Inquilino.Nombre)},  inq.{nameof(Inquilino.Dni)}, inq.{nameof(Inquilino.Email)}, inq.{nameof(Inquilino.Telefono)}, inq.{nameof(Inquilino.Domicilio)},
            p.{nameof(Propietario.IdPropietario)}, p.{nameof(Propietario.Apellido)} AS propApellido, p.{nameof(Propietario.Nombre)} AS propNombre, p.{nameof(Propietario.Dni)} AS propDni,  p.{nameof(Propietario.Email)} AS propEmail, p.{nameof(Propietario.Telefono)} AS propTelefono, p.{nameof(Propietario.Domicilio)} AS propDomicilio,
            inm.{nameof(Inmueble.IdInmueble)}, inm.{nameof(Inmueble.Direccion)}, inm.{nameof(Inmueble.Longitud)}, inm.{nameof(Inmueble.Latitud)}, inm.{nameof(Inmueble.Superficie)}, inm.{nameof(Inmueble.Ambientes)}
            FROM Contratos c
            LEFT JOIN Inmuebles inm ON c.{nameof(Contrato.InmuebleId)} = inm.{nameof(Inmueble.IdInmueble)}
            LEFT JOIN Propietarios p ON inm.{nameof(Inmueble.IdPropietario)} = p.{nameof(Propietario.IdPropietario)}
            LEFT JOIN {Inquilino.GetTableName()} inq ON c.{nameof(Contrato.InquilinoId)} = inq.{nameof(Inquilino.Id)}
            WHERE c.{nameof(Contrato.Id)} = @{nameof(Contrato.Id)};";
            //consider making internal queries to not mess with perfomance with so many JOINs(at least for Pagos and TiposInmueble)
            Console.WriteLine(sql);
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue(nameof(Contrato.Id), id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        c = new Contrato
                        {
                            Id = reader.GetInt32("idContrato"),
                            InmuebleId = reader.GetInt32(nameof(Contrato.InmuebleId)),
                            Monto = reader.GetDecimal(nameof(Contrato.Monto)),
                            FechaDesde = reader.GetDateTime(nameof(Contrato.FechaDesde)),
                            FechaHasta = reader.GetDateTime(nameof(Contrato.FechaHasta)),
                            FechaFinalizacion = reader[nameof(Contrato.FechaFinalizacion)] == DBNull.Value ? null : reader.GetDateTime(nameof(Contrato.FechaFinalizacion)),
                            Estado = reader.GetBoolean(nameof(Contrato.Estado)),
                        };

                        if (!reader.IsDBNull(nameof(Inquilino.Id)))
                        {
                            c.Inquilino = new Inquilino
                            {
                                Id = reader.GetInt32(nameof(Inquilino.Id)),
                                Apellido = reader.GetString(nameof(Inquilino.Apellido)),
                                Nombre = reader.GetString(nameof(Inquilino.Nombre)),
                                Dni = reader.GetString(nameof(Inquilino.Dni)),
                                Email = reader.GetString(nameof(Inquilino.Email)),
                                Telefono = reader.GetString(nameof(Inquilino.Telefono)),
                                Domicilio = reader.GetString(nameof(Inquilino.Domicilio)),
                            };
                        };
                        if (!reader.IsDBNull(nameof(Inmueble.IdInmueble)))
                        {
                            c.Inmueble = new Inmueble
                            {
                                IdInmueble = reader.GetInt32(nameof(Inmueble.IdInmueble)),
                                Direccion = reader.GetString(nameof(Inmueble.Direccion)),
                                Longitud = reader.GetDecimal(nameof(Inmueble.Longitud)),
                                Latitud = reader.GetDecimal(nameof(Inmueble.Latitud)),
                                Superficie = reader.GetInt32(nameof(Inmueble.Superficie)),
                                Ambientes = reader.GetInt32(nameof(Inmueble.Ambientes)),
                            };
                            if (!reader.IsDBNull(nameof(Inmueble.IdPropietario)))
                            {
                                c.Inmueble.Duenio = new Propietario
                                {
                                    IdPropietario = reader.GetInt32(nameof(Propietario.IdPropietario)),
                                    Apellido = reader.GetString("propApellido"),
                                    Nombre = reader.GetString("propNombre"),
                                    Dni = reader.GetString("propDni"),
                                    Email = reader.GetString("propEmail"),
                                    Telefono = reader.GetString("propTelefono"),
                                    Domicilio = reader.GetString("propDomicilio"),
                                };
                            };

                        }
                    };

                }
                connection.Close();
            }
        }
        return c;
    }


    public int GetTotalEntries()
    {
        var result = 0;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = "SELECT COUNT(*) FROM contratos WHERE Estado = 1;";

            using (var command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                result = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
                return result;
            }
        }
    }

    public int Create(int inquilinoId, Inmueble inmueble, DateTime startDate, DateTime endDate)
    {
        Console.WriteLine(inmueble.IdInmueble);
        int idInsert = 0;
        try
        {
            if (inquilinoId <= 0 || inmueble.IdInmueble <= 0)
            {
                throw new Exception("inquilinoId o inmuebleId invalidos");
            }
            if (startDate > endDate)
            {
                throw new Exception("La fecha de inicio no puede ser posterior a la de fin");
            }

            using (var connection = new MySqlConnection(ConnectionString))
            {
                var slq = @$"INSERT INTO Contratos 
                ({nameof(Contrato.InquilinoId)}, 
                {nameof(Contrato.InmuebleId)}, 
                {nameof(Contrato.FechaDesde)}, 
                {nameof(Contrato.FechaHasta)}, 
                {nameof(Contrato.Estado)}, 
                {nameof(Contrato.Monto)})
                    VALUES 
                    (@{nameof(Contrato.InquilinoId)},
                     @inmuebleId, 
                     @{nameof(Contrato.FechaDesde)}, 
                     @{nameof(Contrato.FechaHasta)}, 
                     1, 
                     @{nameof(Contrato.Monto)});
                    SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(slq, connection))
                {
                    command.Parameters.AddWithValue(nameof(Contrato.InquilinoId), inquilinoId);
                    command.Parameters.AddWithValue("@inmuebleId", inmueble.IdInmueble);
                    command.Parameters.AddWithValue(nameof(Contrato.FechaDesde), startDate);
                    command.Parameters.AddWithValue(nameof(Contrato.FechaHasta), endDate);
                    command.Parameters.AddWithValue(nameof(Contrato.Monto), inmueble.Precio);
                    Console.WriteLine(slq);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            idInsert = reader.GetInt32(0);
                        }
                        connection.Close();
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw new Exception(e.Message);
        }

        return idInsert;
    }

    public List<Contrato> GetOccupiedDates(int inmuebleId)
    {
        var res = new List<Contrato>();

        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"SELECT {nameof(Contrato.FechaDesde)}, IFNULL({nameof(Contrato.FechaFinalizacion)},{nameof(Contrato.FechaHasta)}) AS {nameof(Contrato.FechaHasta)}
            FROM `contratos` 
            JOIN `inmuebles` ON contratos.{nameof(Contrato.InmuebleId)} = inmuebles.{nameof(Inmueble.IdInmueble)} 
            WHERE contratos.{nameof(Contrato.Estado)} = 1
            AND (contratos.{nameof(Contrato.FechaFinalizacion)} IS NULL OR (contratos.{nameof(Contrato.FechaFinalizacion)} >= CURRENT_DATE() AND contratos.{nameof(Contrato.FechaFinalizacion)} >= contratos.{nameof(Contrato.FechaHasta)})) 
            AND contratos.{nameof(Contrato.InmuebleId)} = @inmuebleId
            AND inmuebles.{nameof(Inmueble.Estado)} = 'Disponible';";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@inmuebleId", inmuebleId);
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    res.Add(new Contrato
                    {
                        FechaDesde = reader.GetDateTime(nameof(Contrato.FechaDesde)),
                        FechaHasta = reader.GetDateTime(nameof(Contrato.FechaHasta))
                    });
                }
                connection.Close();
            }
        }
        return res;
    }

    //Trae contratos que ocupen fechas del inmueble por fuera del rango de fechas del contrato proveído
    public List<Contrato> getOccupiedDatesDiscrim(int idContrato, int idInmueble)
    {
        var res = new List<Contrato>();

        try
        {
            if (idContrato <= 0 || idInmueble <= 0)
            {
                throw new Exception("idContrato o idInmueble invalidos");
            }

            using (var connection = new MySqlConnection(ConnectionString))
            {

                var sql = @$"SELECT 
                        {nameof(Contrato.FechaDesde)}, IFNULL({nameof(Contrato.FechaFinalizacion)},{nameof(Contrato.FechaHasta)}) AS {nameof(Contrato.FechaHasta)}
                            FROM `contratos` 
                            JOIN `inmuebles` ON contratos.{nameof(Contrato.InmuebleId)} = inmuebles.{nameof(Inmueble.IdInmueble)} 
                            WHERE contratos.{nameof(Contrato.Estado)} = 1
                            AND (contratos.{nameof(Contrato.FechaFinalizacion)} IS NULL OR (contratos.{nameof(Contrato.FechaFinalizacion)} >= CURRENT_DATE() AND contratos.{nameof(Contrato.FechaFinalizacion)} >= contratos.{nameof(Contrato.FechaHasta)})) 
                            AND contratos.{nameof(Contrato.InmuebleId)} = @inmuebleId
                            AND contratos.{nameof(Contrato.Id)} != @idContrato
                            AND inmuebles.{nameof(Inmueble.Estado)} = 'Disponible'";


                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@inmuebleId", idInmueble);
                    command.Parameters.AddWithValue("@idContrato", idContrato);
                    connection.Open();
                    Console.WriteLine(sql);
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(new Contrato
                        {
                            FechaDesde = reader.GetDateTime(nameof(Contrato.FechaDesde)),
                            FechaHasta = reader.GetDateTime(nameof(Contrato.FechaHasta))
                        });
                    }
                    connection.Close();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw new Exception(ex.Message);
        }

        return res;
    }

    public int Delete(int id)
    {
        int result = -1;

        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"UPDATE contratos
            SET {nameof(Contrato.Estado)} = 0
            WHERE {nameof(Contrato.Id)} = @id;";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                result = command.ExecuteNonQuery();
                connection.Close();
            }
        }

        return result;
    }

}

