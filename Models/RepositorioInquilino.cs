using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace PenalozaFernandezInmobiliario.Models;

public class RepositorioInquilino
{
    
    readonly string ConnectionString = "Server=localhost;Database=inmovalepablo;User=root;Password=;";

    public RepositorioInquilino(){

    }

    //trae solo data necesaria para mostrar en index
    public IList<Inquilino> GetAllForIndex(int pageSize, int pageNumber)
    {
        var inquilinos = new List<Inquilino>();
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"SELECT {nameof(Inquilino.Id)}, {nameof(Inquilino.Nombre)}, {nameof(Inquilino.Apellido)},
             {nameof(Inquilino.Dni)}, {nameof(Inquilino.Telefono)}, {nameof(Inquilino.Email)} 
             FROM inquilinos 
             WHERE {nameof(Inquilino.Estado)} = 1
             LIMIT {pageSize} OFFSET {(pageNumber - 1) * pageSize};";

            using (var command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        inquilinos.Add(new Inquilino{
                            Id = reader.GetInt32(nameof(Inquilino.Id)),
                            Nombre = reader.GetString(nameof(Inquilino.Nombre)),
                            Apellido = reader.GetString(nameof(Inquilino.Apellido)),
                            Dni = reader.GetString(nameof(Inquilino.Dni)),
                            Telefono = reader.GetString(nameof(Inquilino.Telefono)),
                            Email = reader.GetString(nameof(Inquilino.Email))
                        });
                        
                    }
                    connection.Close();
                }
            }
        }
        return inquilinos;
    }

    public int getTotalEntries(){
        var result = 0;
        using (var connection = new MySqlConnection(ConnectionString)){
            var sql = "SELECT COUNT(*) FROM Inquilinos WHERE Estado = 1;";

            using (var command = new MySqlCommand(sql, connection)){
                connection.Open();
                result = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
                return result;
            }
        }
    }

    public bool getHasMorePages(int pageNumber, int pageSize){
        var result = false;
        using(var connection = new MySqlConnection(ConnectionString)){
            using (var command = new MySqlCommand("GET_HASMOREPAGES", connection)){
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@TableName", "Inquilinos"); //check out how to handle using nameOf here
                command.Parameters.AddWithValue("@PageNumber", pageNumber);
                command.Parameters.AddWithValue("@PageSize", pageSize);

                MySqlParameter hasMorePagesParam = new MySqlParameter("@HasMorePages", MySqlDbType.Bit);
                hasMorePagesParam.Direction = ParameterDirection.Output;
                command.Parameters.Add(hasMorePagesParam);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                result = Convert.ToBoolean(hasMorePagesParam.Value);
                
                return result;
            }
        }
    }

    public IList<Inquilino> GetAll()
    {
        var inquilinos = new List<Inquilino>();
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"SELECT {nameof(Inquilino.Id)}, {nameof(Inquilino.Nombre)}, {nameof(Inquilino.Apellido)},
             {nameof(Inquilino.Dni)}, {nameof(Inquilino.Domicilio)}, {nameof(Inquilino.Telefono)},
              {nameof(Inquilino.Email)} FROM inquilinos WHERE {nameof(Inquilino.Estado)} = 1";

            using (var command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        inquilinos.Add(new Inquilino{
                            Id = reader.GetInt32(nameof(Inquilino.Id)),
                            Nombre = reader.GetString(nameof(Inquilino.Nombre)),
                            Apellido = reader.GetString(nameof(Inquilino.Apellido)),
                            Dni = reader.GetString(nameof(Inquilino.Dni)),
                            Domicilio = reader.GetString(nameof(Inquilino.Domicilio)),
                            Telefono = reader.GetString(nameof(Inquilino.Telefono)),
                            Email = reader.GetString(nameof(Inquilino.Email))
                        });
                        
                    }
                    connection.Close();
                }
            }
        }
        return inquilinos;
    }

    public int Update(Inquilino inquilino){
        int result = -1;
        using(var connection = new MySqlConnection(ConnectionString)){
            var sql = @$"UPDATE inquilinos SET {nameof(Inquilino.Nombre)} = @{nameof(Inquilino.Nombre)},
            {nameof(Inquilino.Apellido)} = @{nameof(Inquilino.Apellido)}, {nameof(Inquilino.Dni)} = @{nameof(Inquilino.Dni)},
            {nameof(Inquilino.Domicilio)} = @{nameof(Inquilino.Domicilio)}, {nameof(Inquilino.Telefono)} = @{nameof(Inquilino.Telefono)}, 
            {nameof(Inquilino.Email)} = @{nameof(Inquilino.Email)} WHERE {nameof(Inquilino.Id)} = @{nameof(Inquilino.Id)}";
            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue(nameof(Inquilino.Id), inquilino.Id);
                command.Parameters.AddWithValue(nameof(Inquilino.Nombre), inquilino.Nombre);
                command.Parameters.AddWithValue(nameof(Inquilino.Apellido), inquilino.Apellido);
                command.Parameters.AddWithValue(nameof(Inquilino.Dni), inquilino.Dni);
                command.Parameters.AddWithValue(nameof(Inquilino.Domicilio), inquilino.Domicilio);
                command.Parameters.AddWithValue(nameof(Inquilino.Telefono), inquilino.Telefono);
                command.Parameters.AddWithValue(nameof(Inquilino.Email), inquilino.Email);
                connection.Open();
                result = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return result;
    }

    public Inquilino? GetById(int id){
        Inquilino? inquilino = null;
        using(var connection = new MySqlConnection(ConnectionString)){
            var sql = @$"SELECT {nameof(Inquilino.Id)}, {nameof(Inquilino.Nombre)}, {nameof(Inquilino.Apellido)},
            {nameof(Inquilino.Dni)}, {nameof(Inquilino.Domicilio)}, {nameof(Inquilino.Telefono)},
            {nameof(Inquilino.Email)} FROM inquilinos WHERE {nameof(Inquilino.Id)} = @{nameof(Inquilino.Id)}";
        
            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue(nameof(Inquilino.Id), id);
                connection.Open();
                using(var reader = command.ExecuteReader()){
                    if(reader.Read()){
                        inquilino = new Inquilino{
                            Id = reader.GetInt32(nameof(Inquilino.Id)),
                            Nombre = reader.GetString(nameof(Inquilino.Nombre)),
                            Apellido = reader.GetString(nameof(Inquilino.Apellido)),
                            Dni = reader.GetString(nameof(Inquilino.Dni)),
                            Domicilio = reader.GetString(nameof(Inquilino.Domicilio)),
                            Telefono = reader.GetString(nameof(Inquilino.Telefono)),
                            Email = reader.GetString(nameof(Inquilino.Email))
                        };
                    }
                    connection.Close();
                }
                
            }
            return inquilino;
        }
    }

    public int Create(Inquilino inquilino){
        int id = 0;
        using(var connection = new MySqlConnection(ConnectionString)){
            var sql = @$"INSERT INTO inquilinos ({nameof(Inquilino.Nombre)}, {nameof(Inquilino.Apellido)},
            {nameof(Inquilino.Dni)}, {nameof(Inquilino.Domicilio)}, {nameof(Inquilino.Telefono)},
            {nameof(Inquilino.Email)}) VALUES (@{nameof(Inquilino.Nombre)}, @{nameof(Inquilino.Apellido)},
            @{nameof(Inquilino.Dni)}, @{nameof(Inquilino.Domicilio)}, @{nameof(Inquilino.Telefono)},
            @{nameof(Inquilino.Email)});
            SELECT LAST_INSERT_ID();";
            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue(nameof(Inquilino.Nombre), inquilino.Nombre);
                command.Parameters.AddWithValue(nameof(Inquilino.Apellido), inquilino.Apellido);
                command.Parameters.AddWithValue(nameof(Inquilino.Dni), inquilino.Dni);
                command.Parameters.AddWithValue(nameof(Inquilino.Domicilio), inquilino.Domicilio);
                command.Parameters.AddWithValue(nameof(Inquilino.Telefono), inquilino.Telefono);
                command.Parameters.AddWithValue(nameof(Inquilino.Email), inquilino.Email);
                connection.Open();
                id = Convert.ToInt32(command.ExecuteScalar());
                inquilino.Id = id;
                connection.Close();
            }
        }
        return id;
    }

    public int Delete(int id){
        int result = -1;
        using(var connection = new MySqlConnection(ConnectionString)){
            var sql = @$"UPDATE inquilinos SET {nameof(Inquilino.Estado)} = 0 WHERE {nameof(Inquilino.Id)} = @{nameof(Inquilino.Id)}";
            using(var command = new MySqlCommand(sql, connection)){
                command.Parameters.AddWithValue(nameof(Inquilino.Id), id);
                connection.Open();
                result = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return result;
    }
        
}
