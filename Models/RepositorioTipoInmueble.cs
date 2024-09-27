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
                var sql = "SELECT DISTINCT id, tipo FROM tiposinmueble WHERE estado = 1;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tipoInmuebles.Add(new TipoInmueble
                            {
                                IdTipoInmueble = reader.GetInt32("id"),

                                Tipo = reader.GetString(reader.GetOrdinal(nameof(TipoInmueble.Tipo))),
                            });
                        }
                    }
                    connection.Close();
                }
            }

            return tipoInmuebles;
        }

        public int AddTipoInmueble(string tipo)
        {
            int result = -1;
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = @"INSERT INTO tiposinmueble (tipo, estado) VALUES (@tipo, 1); SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@tipo", tipo);
                    connection.Open();
                    result = Convert.ToInt32(command.ExecuteScalar()); // devuelvo el id del nuevo registro
                    connection.Close();
                }
            }
            return result;
        }


        public int Delete(int id)
        {
            int result = -1;
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = @"UPDATE tiposinmueble 
                    SET estado = 0  
                    WHERE id = @id;";

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
}
