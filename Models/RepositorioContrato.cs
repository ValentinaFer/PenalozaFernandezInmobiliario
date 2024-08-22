using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace PenalozaFernandezInmobiliario.Models;

public class RepositorioContrato
{

    readonly string ConnectionString = "Server=localhost;Database=inmovalepablo;User=root;Password=;";

    public RepositorioContrato()
    {

    }

    public IList<Contrato> GetAllForIndex(int pageSize, int pageNumber)
    {
        var contratos = new List<Contrato>();
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"SELECT c.{nameof(Contrato.Id)}, c.{nameof(Contrato.InmuebleId)}, c.{nameof(Contrato.Monto)}, c.{nameof(Contrato.FechaDesde)}, c.{nameof(Contrato.FechaHasta)},
            inq.{nameof(Inquilino.Id)}, inq.{nameof(Inquilino.Apellido)},  inq.{nameof(Inquilino.Nombre)},  inq.{nameof(Inquilino.Dni)},
            p.idPropietario, p.{nameof(Propietario.Apellido)}, p.{nameof(Propietario.Nombre)}, p.{nameof(Propietario.Dni)},
            inm.{nameof(Inmueble.IdInmueble)}, inm.{nameof(Inmueble.Direccion)}
            FROM Contratos c
            INNER JOIN Inmuebles inm ON c.{nameof(Contrato.InmuebleId)} = inm.{nameof(Inmueble.IdInmueble)}
            INNER JOIN Propietarios p ON inm.{nameof(Inmueble.IdPropietario)} = p.idPropietario
            INNER JOIN {Inquilino.GetTableName()} inq ON c.{nameof(Contrato.InquilinoId)} = inq.{nameof(Inquilino.Id)}
            WHERE c.{nameof(Contrato.Estado)} = 1
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
                            Monto = reader.GetDecimal(nameof(Contrato.Monto)),
                            //InmuebleId = reader.GetInt32(nameof(Contrato.InmuebleId).ToLower()),
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
                                /*Longitud = reader.GetDecimal( nameof(Inmueble.Longitud)),
                                Latitud = reader.GetDecimal(nameof(Inmueble.Latitud)),
                                Propietario = new Propietario{
                                    IdPropietario = reader.GetInt32(nameof(Propietario.IdPropietario)),
                                    Apellido = reader.GetString(nameof(Propietario.Apellido)),
                                    Nombre = reader.GetString(nameof(Propietario.Nombre)),
                                    Dni = reader.GetString(nameof(Propietario.Dni)),
                                }*/
                            }

                        });

                    }
                    connection.Close();
                }
            }
        }
        return contratos;
    }


}
