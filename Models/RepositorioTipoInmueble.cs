using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace PenalozaFernandezInmobiliario.Models
{
    public class RepositorioTipoInmueble
    {
        readonly string ConnectionString = "Server=localhost;Database=inmovalepablo;User=root;Password=;";

        public IList<TipoInmueble> GetTipoInmuebles()
        {
            var tipoInmuebles = new List<TipoInmueble>();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = "SELECT id, tipo FROM tiposinmueble;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tipoInmuebles.Add(new TipoInmueble
                            {
                                idTipoInmueble = reader.GetInt32("id"),

                                tipo = reader.GetString(reader.GetOrdinal(nameof(TipoInmueble.tipo))),
                            });
                        }
                    }
                    connection.Close();
                }
            }

            return tipoInmuebles;
        }
    }
}
