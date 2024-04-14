using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace PenalozaFernandezInmobiliario.Models;

public class RepositorioContrato
{
    
    readonly string ConnectionString = "Server=localhost;Database=inmovalepablo;User=root;Password=;";

    public RepositorioContrato(){

    }

    public IList<Contrato> GetAllForIndex(int pageSize, int pageNumber)
    {
        var contratos = new List<Contrato>();
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"SELECT c.{nameof(Contrato.Id)}, c.{nameof(Contrato.InmuebleId)}, c.{nameof(Contrato.Monto)}, c.{nameof(Contrato.FechaDesde)}, c.{nameof(Contrato.FechaHasta)},
            inq.{nameof(Inquilino.Id)}, inq.{nameof(Inquilino.Apellido)},  inq.{nameof(Inquilino.Nombre)},  inq.{nameof(Inquilino.Dni)},
            p.{nameof(Propietario.IdPropietario)}, p.{nameof(Propietario.Apellido)}, p.{nameof(Propietario.Nombre)}, p.{nameof(Propietario.Dni)},
            inm.{nameof(Inmueble.Id)}, inm.{nameof(Inmueble.Direccion)},
            FROM {nameof(Contrato)} c
            INNER JOIN {nameof(Inmueble)} inm ON c.{nameof(Contrato.InmuebleId)} = inm.{nameof(Inmueble.Id)}
            INNER JOIN {nameof(Propietario)} p ON inm.{nameof(Inmueble.PropietarioId)} = p.{nameof(Propietario.IdPropietario)}
            INNER JOIN {nameof(Inquilino)} inq ON c.{nameof(Contrato.InquilinoId)} = inq.{nameof(Inquilino.Id)}
            WHERE {nameof(Contrato.Estado)} = 1
            LIMIT {pageSize} OFFSET {(pageNumber - 1) * pageSize};";

            using (var command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        contratos.Add(new Contrato{
                            Id = reader.GetInt32(string.Concat("c.", nameof(Contrato.Id))),
                            FechaDesde = reader.GetDateTime(string.Concat("c.", nameof(Contrato.FechaDesde))),
                            FechaHasta = reader.GetDateTime(string.Concat("c.", nameof(Contrato.FechaHasta))),
                            Monto = reader.GetDecimal(string.Concat("c.", nameof(Contrato.Monto))),
                            //InmuebleId = reader.GetInt32(string.Concat("c.", nameof(Contrato.InmuebleId))),
                            Inquilino = new Inquilino{
                                Nombre = reader.GetString(string.Concat("inq.", nameof(Inquilino.Nombre))),
                                Apellido = reader.GetString(string.Concat("inq.", nameof(Inquilino.Apellido))),
                                Dni = reader.GetString(string.Concat("inq.", nameof(Inquilino.Dni))),
                            },
                            Inmueble = new Inmueble{
                                Id = reader.GetInt32(string.Concat("inm.", nameof(Inmueble.Id))),
                                Direccion = reader.GetString(string.Concat("inm.", nameof(Inmueble.Direccion))),
                                /*Propietario = new Propietario{
                                    IdPropietario = reader.GetInt32(string.Concat("p.", nameof(Propietario.IdPropietario))),
                                    Apellido = reader.GetString(string.Concat("p.", nameof(Propietario.Apellido))),
                                    Nombre = reader.GetString(string.Concat("p.", nameof(Propietario.Nombre))),
                                    Dni = reader.GetString(string.Concat("p.", nameof(Propietario.Dni))),
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
