using System.Configuration;
using System.Data;
using System.Transactions;
using MySql.Data.MySqlClient;

namespace PenalozaFernandezInmobiliario.Models;

public class RepositorioPago
{

    readonly string ConnectionString = "Server=localhost;Database=inmovalepablo;User=root;Password=;";

    public RepositorioPago()
    {

    }

    public int HabilitarPago(int idContrato, int idPago)
    {
        var res = -1;
        try
        {

            using (var connection = new MySqlConnection(ConnectionString))
            {

                var sql = @$"UPDATE pagos
                    SET {nameof(Pago.Estado)} = 1
                    WHERE {nameof(Pago.IdContrato)} = @idContrato
                    AND {nameof(Pago.Id)} = @id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idContrato", idContrato);
                    command.Parameters.AddWithValue("@id", idPago);

                    connection.Open();
                    res = command.ExecuteNonQuery();
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

    public CalculosContratoResponse ObtenerCalculos(Contrato contrato)
    {
        var res = new CalculosContratoResponse();
        bool contratoPagado = false;
        decimal mitadContrato = 0;
        decimal mesesHastaFinalizacion = 0;
        decimal APagar = 0;
        bool Vencido = false;
        bool cancelado = false;
        decimal loQueQueda = 0; //lo que queda por pagar
        int mesesPagados = contrato.Pagos != null ? contrato.Pagos.Count(p => p.Estado) : 0; //se asume que no tiene pagos en este caso
        var mesesTotales = ((contrato.FechaHasta.Year - contrato.FechaDesde.Year) * 12) + contrato.FechaHasta.Month - contrato.FechaDesde.Month + 1;
        decimal multa = 0;
        Console.WriteLine($"Meses totales: {mesesTotales}");
        Console.WriteLine($"Meses pagados: {mesesPagados}");
        if (contrato.FechaFinalizacion == null)
        {
            Console.WriteLine("contrato no finalizado");
            var mesesAPagar = mesesTotales - mesesPagados;
            loQueQueda = mesesAPagar * contrato.Monto;
            Console.WriteLine($"Meses a pagar: {mesesAPagar}");
            Console.WriteLine($"loQueQueda: {loQueQueda}");
        }
        else if (contrato.FechaFinalizacion < contrato.FechaHasta)
        {
            cancelado = true;
            Console.WriteLine("contrato finalizado");
            mesesHastaFinalizacion = CalculateMonthsAsDecimal(contrato.FechaDesde, contrato.FechaFinalizacion.Value);
            mitadContrato = (decimal)mesesTotales / 2;

            Console.WriteLine($"Meses hasta finalizacion: {mesesHastaFinalizacion}");
            Console.WriteLine($"Mitad del contrato: {mitadContrato}");
            if (mesesHastaFinalizacion < mitadContrato)
            {
                multa = contrato.Monto * 2;
            }
            else
            {
                multa = contrato.Monto;
            }
            if (mesesHastaFinalizacion < 0)
            {
                mesesHastaFinalizacion = 0;
            }
            var mesesAPagarRestantes = Math.Ceiling(mesesHastaFinalizacion) - mesesPagados;
            Console.WriteLine($"Meses a pagar restantes: {mesesAPagarRestantes}");

            loQueQueda = (mesesAPagarRestantes * contrato.Monto) + multa;
            Console.WriteLine($"loQueQueda: {loQueQueda}");
            Console.WriteLine($"Multa: {multa}");
        }

        if (loQueQueda <= 0)
        {
            contratoPagado = true;
        }
        else if (contrato.FechaHasta >= contrato.FechaFinalizacion)
        {
            Vencido = true;
        }

        res = new CalculosContratoResponse
        {
            Vencido = Vencido,
            MesesPagadosSinMulta = mesesPagados,
            MesesTotales = mesesTotales,
            TotalPagado = mesesPagados * contrato.Monto,
            TotalAPagar = mesesTotales * contrato.Monto,
            //lo de arriba es para un contrato normal(aka. vigente)
            FaltanteAPagar = loQueQueda,

            MesesHastaFinalizacion = mesesHastaFinalizacion,
            MesesMulta = (mesesHastaFinalizacion >= mitadContrato) ? 2 : 1,
            Multa = multa,

            Cancelado = cancelado,
            ContratoPagado = contratoPagado
        };

        return res;
    }

    public int Eliminar(Contrato contrato, int idPago)
    {
        var res = -1;
        var contratoPagado = false;
        try
        {
            if (contrato == null || contrato.Id <= 0)
            {
                throw new ArgumentException("No se recibio el ID del contrato");
            }
            if (contrato.Monto <= 0)
            {
                throw new ArgumentException("No se recibio el monto del contrato");
            }
            if (contrato.FechaDesde > contrato.FechaHasta)
            {
                throw new Exception("El contrato contiene una fecha de inicio mayor a la de fin");
            }
            decimal APagar = 0;
            decimal loQueQueda = 0; //lo que queda por pagar
            int mesesPagados = contrato.Pagos != null ? contrato.Pagos.Count(p => p.Estado) : 0; //se asume que no tiene pagos en este caso
            var mesesTotales = ((contrato.FechaHasta.Year - contrato.FechaDesde.Year) * 12) + contrato.FechaHasta.Month - contrato.FechaDesde.Month + 1;
            decimal multa = 0;
            Console.WriteLine($"Meses totales: {mesesTotales}");
            Console.WriteLine($"Meses pagados: {mesesPagados}");
            if (contrato.FechaFinalizacion == null)
            {
                Console.WriteLine("contrato no finalizado");
                var mesesAPagar = mesesTotales - mesesPagados;
                loQueQueda = mesesAPagar * contrato.Monto;
                Console.WriteLine($"Meses a pagar: {mesesAPagar}");
                Console.WriteLine($"loQueQueda: {loQueQueda}");
            }
            else if (contrato.FechaFinalizacion < contrato.FechaHasta)
            {
                Console.WriteLine("contrato finalizado");
                decimal mesesHastaFinalizacion = CalculateMonthsAsDecimal(contrato.FechaDesde, contrato.FechaFinalizacion.Value);
                decimal mitadContrato = mesesTotales / 2;
                Console.WriteLine($"Meses hasta finalizacion: {mesesHastaFinalizacion}");
                Console.WriteLine($"Mitad del contrato: {mitadContrato}");
                if (mesesHastaFinalizacion < mitadContrato)
                {
                    multa = contrato.Monto * 2;
                }
                else
                {
                    multa = contrato.Monto;
                }
                if (mesesHastaFinalizacion < 0)
                {
                    mesesHastaFinalizacion = 0;
                }
                var mesesAPagarRestantes = Math.Ceiling(mesesHastaFinalizacion) - mesesPagados;
                Console.WriteLine($"Meses a pagar restantes: {mesesAPagarRestantes}");

                loQueQueda = (mesesAPagarRestantes * contrato.Monto) + multa;
                Console.WriteLine($"loQueQueda: {loQueQueda}");
                Console.WriteLine($"Multa: {multa}");
            }

            if (loQueQueda <= 0)
            {
                contratoPagado = true;
            }

            using (var connection = new MySqlConnection(ConnectionString))
            {

                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var sql = @$"UPDATE Pagos SET
                            {nameof(Pago.Estado)} = 0
                            WHERE {nameof(Pago.IdContrato)} = @contratoId 
                            AND {nameof(Pago.Id)} = @id;";
                        using (var command = new MySqlCommand(sql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@contratoId", contrato.Id);
                            command.Parameters.AddWithValue("@id", idPago);

                            res = command.ExecuteNonQuery();
                            if (res <= 0)
                            {
                                transaction.Rollback();
                                return -1;
                            }
                            mesesPagados--;
                            decimal TotalPagado = mesesPagados * contrato.Monto;

                            if (contratoPagado == true && contrato.FechaFinalizacion != null)
                            {
                                if (contrato.FechaFinalizacion >= contrato.FechaHasta)
                                {
                                    //si anteriormente el contrato tenia una fecha de finalizacion >= a la de fin del contrato
                                    //aka: si estaba finalizado, al restar un pago, deja de estarlo
                                    //por ello, seteo de vuelta a vigente(NULL)
                                    var sql2 = @$"UPDATE contratos SET
                                    {nameof(Contrato.FechaFinalizacion)} = NULL
                                    WHERE {nameof(Contrato.Id)} = @contratoId;";
                                    using (var command2 = new MySqlCommand(sql2, connection, transaction))
                                    {
                                        command2.Parameters.AddWithValue("@contratoId", contrato.Id);
                                        command2.ExecuteNonQuery();
                                    }
                                }


                            }

                            transaction.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine(ex);
                        return -1;
                    }
                }

            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw new Exception(ex.Message);
        }
        return res;
    }


    public int Editar(int idContrato, string detalleNuevo, int idPago)
    {
        var res = -1;
        try
        {

            using (var connection = new MySqlConnection(ConnectionString))
            {

                var sql = @$"UPDATE pagos
                    SET {nameof(Pago.DetallePago)} = @detalle 
                    WHERE {nameof(Pago.IdContrato)} = @idContrato
                    AND {nameof(Pago.Id)} = @id;";

                using (var command = new MySqlCommand(sql, connection))
                {

                    command.Parameters.AddWithValue("@idContrato", idContrato);
                    command.Parameters.AddWithValue("@id", idPago);
                    command.Parameters.AddWithValue("@detalle", detalleNuevo);

                    connection.Open();
                    res = command.ExecuteNonQuery();
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

    public Contrato? ObtenerPagosPorContrato(int idContrato)
    {

        Contrato? contrato = null;

        try
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = @$"SELECT
                c.{nameof(Contrato.Id)} as idContrato,
                c.{nameof(Contrato.FechaDesde)},
                c.{nameof(Contrato.FechaHasta)}, 
                c.{nameof(Contrato.Monto)}, 
                c.{nameof(Contrato.FechaFinalizacion)},
                c.{nameof(Contrato.Estado)} as contratoEstado,
                p.*
                FROM contratos c
                LEFT JOIN pagos p ON p.{nameof(Pago.IdContrato)} = c.{nameof(Contrato.Id)}
                WHERE c.{nameof(Contrato.Id)} = @idContrato;";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idContrato", idContrato);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {

                        if (contrato == null)
                        {
                            contrato = new Contrato
                            {
                                Id = reader.GetInt32("idContrato"),
                                Monto = reader.GetDecimal(nameof(Contrato.Monto)),
                                FechaFinalizacion = reader.IsDBNull(reader.GetOrdinal(nameof(Contrato.FechaFinalizacion))) ? null : reader.GetDateTime(nameof(Contrato.FechaFinalizacion)),
                                FechaHasta = reader.GetDateTime(nameof(Contrato.FechaHasta)),
                                FechaDesde = reader.GetDateTime(nameof(Contrato.FechaDesde)),
                                Estado = reader.GetBoolean("contratoEstado"),
                                Pagos = new List<Pago>()
                            };

                        }
                        Console.WriteLine("dentro de if: " + idContrato);
                        if (!reader.IsDBNull(reader.GetOrdinal(nameof(Pago.Id))) && contrato.Pagos != null)
                        {

                            contrato.Pagos.Add(new Pago
                            {
                                Id = reader.GetInt32(nameof(Pago.Id)),
                                IdContrato = reader.GetInt32(nameof(Pago.IdContrato)),
                                Importe = reader.GetDecimal(nameof(Pago.Importe)),
                                DetallePago = reader.GetString(nameof(Pago.DetallePago)),
                                FechaPago = reader.GetDateTime(nameof(Pago.FechaPago)),
                                NroPago = reader.GetInt32(nameof(Pago.NroPago)),
                                Estado = reader.GetBoolean(nameof(Pago.Estado)),
                            });
                        }
                    }


                };

                connection.Close();
            }

            return contrato;

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw ex;
        }

    }

    //si la finalizacion es null, esta vigente el contrato(aka: aun no ha terminado de pagar)
    //else si la finalizacion es menor a al fecha estipulado, el contrato fue cancelado antes de tiempo
    //else si la finalizacion es mayor a la fecha estipulada(o igual), el contrato fue finalizado correctamente(aka: ya fue pagado)
    private bool IsPaymentFinished(Contrato contrato)
    {
        if (contrato.FechaFinalizacion == null || contrato.FechaFinalizacion >= contrato.FechaHasta)
        {
            var totalAPagar = contrato.Monto * getMesesBetween(contrato.FechaDesde, contrato.FechaHasta);
            var totalPagado = contrato.Pagos.Count(p => p.Estado) * contrato.Monto;
            if (totalAPagar == totalPagado)
            {
                return true;
            }
            else if (totalAPagar < totalPagado)
            {
                throw new InvalidOperationException("Error: Se ha pagado más de lo requerido. A pagar:" + totalAPagar + ", Pagado:" + totalPagado + ". Consulte con un administrador");
            }
            return false;
        }
        if (contrato.FechaFinalizacion < contrato.FechaHasta)
        { //cancelado earlier
            var totalAPagar = (CalcularMesesFaltantesSinMulta(contrato) * contrato.Monto) + CalcularMulta(contrato);

            var mesesPagado = contrato.Pagos.Count(p => p.Estado);
            var totalPagado = mesesPagado * contrato.Monto;
            if (totalAPagar == totalPagado)
            {
                return true;
            }
            else if (totalAPagar < totalPagado)
            {
                throw new InvalidOperationException("Error: Se ha pagado más de lo requerido. A pagar:" + totalAPagar + ", Pagado:" + totalPagado + ". Consulte con un administrador");
            }
            return false;
        }

        return false;
    }

    private int getMesesBetween(DateTime dateFrom, DateTime dateTo)
    {
        int months = 12 * (dateTo.Year - dateFrom.Year) + dateTo.Month - dateFrom.Month;
        if (dateTo.Day >= dateFrom.Day)
        {
            months++;
        }
        return months;
    }

    //calculo de multa sin considerar meses faltantes de pago
    //Si no se provee una fechaFinalizacion, se tomara la fecha de hoy (para calculos de multa antes de cancelar)
    private decimal CalcularMulta(Contrato contrato)
    {
        decimal multa = 0;

        //meses total del contrato desde inicio a hasta
        var mesesTotales = getMesesBetween(contrato.FechaDesde, contrato.FechaHasta);
        //meses transcurrido desde inicio de contrato a finalizacion (u hasta hoy si no finalizo)
        var mesesTranscurridos = CalculateMonthsAsDecimal(contrato.FechaDesde, contrato.FechaFinalizacion != null ? contrato.FechaFinalizacion.Value : DateTime.Now);
        //meses pagados en base a los pagos del contrato

        //si se cancela antes de la mitad del contrato
        if (mesesTranscurridos >= (mesesTotales / 2))
        {
            multa = contrato.Monto * 2; //2 meses de multa
        }
        else
        {
            multa = contrato.Monto; //1 mes de multa
        }

        return multa;
    }


    //rounded up! (i.e. round up to the nearest whole number/month. 
    //e.g. if 1.3 months have passed, it will round up to 2 months(rounded up for the month of cancellation))
    //Returns months missing to pay(if fechaFinalizacion is null, it will take today as default)
    private int CalcularMesesFaltantesSinMulta(Contrato contrato)
    {
        decimal mesesFaltantes = 0;
        var mesesTranscurridos = CalculateMonthsAsDecimal(contrato.FechaDesde, contrato.FechaFinalizacion != null ? contrato.FechaFinalizacion.Value : DateTime.Now);

        if (contrato.Pagos != null)
        {
            var mesesPagados = contrato.Pagos.Count(p => p.Estado);
            //resto meses pagados de meses transcurridos
            mesesFaltantes = mesesTranscurridos - mesesPagados;
        }
        if (mesesFaltantes < 0)
        { //para no devolver negativos(shouldn't happen actually)
            mesesFaltantes = 0;
        }
        return (int)Math.Ceiling(mesesFaltantes);
    }

    private decimal CalculateMonthsAsDecimal(DateTime startDate, DateTime endDate)
    {
        int totalMonths = (endDate.Year - startDate.Year) * 12 + endDate.Month - startDate.Month;
        decimal daysInStartMonth = DateTime.DaysInMonth(startDate.Year, startDate.Month);
        decimal daysInEndMonth = DateTime.DaysInMonth(endDate.Year, endDate.Month);

        decimal partialStartMonth = (daysInStartMonth - startDate.Day + 1) / daysInStartMonth;
        decimal partialEndMonth = endDate.Day / daysInEndMonth;

        return totalMonths + partialStartMonth + partialEndMonth - 1;
    }

    public int Crear(string detalle, Contrato contrato)
    {
        var contratoPagado = false;
        var nroPago = 0;
        try
        {
            if (contrato == null || contrato.Id <= 0)
            {
                throw new ArgumentException("No se recibio el ID del contrato");
            }
            if (contrato.Pagos != null)
            {
                nroPago = contrato.Pagos.Count(p => p.Estado);
            }
            if (contrato.Monto <= 0)
            {
                throw new ArgumentException("No se recibio el monto del contrato");
            }
            if (contrato.FechaDesde > contrato.FechaHasta)
            {
                throw new Exception("El contrato contiene una fecha de inicio mayor a la de fin");
            }
            decimal APagar = 0;
            decimal loQueQueda = 0; //lo que queda por pagar
            int mesesPagados = contrato.Pagos != null ? contrato.Pagos.Count(p => p.Estado) : 0; //se asume que no tiene pagos en este caso
            var mesesTotales = ((contrato.FechaHasta.Year - contrato.FechaDesde.Year) * 12) + contrato.FechaHasta.Month - contrato.FechaDesde.Month + 1;
            decimal multa = 0;
            Console.WriteLine($"Meses totales: {mesesTotales}");
            Console.WriteLine($"Meses pagados: {mesesPagados}");
            if (contrato.FechaFinalizacion == null)
            {
                Console.WriteLine("contrato no finalizado");
                var mesesAPagar = mesesTotales - mesesPagados;
                loQueQueda = mesesAPagar * contrato.Monto;
                Console.WriteLine($"Meses a pagar: {mesesAPagar}");
                Console.WriteLine($"loQueQueda: {loQueQueda}");
            }
            else if (contrato.FechaFinalizacion < contrato.FechaHasta)
            {
                Console.WriteLine("contrato finalizado");
                decimal mesesHastaFinalizacion = CalculateMonthsAsDecimal(contrato.FechaDesde, contrato.FechaFinalizacion.Value);
                decimal mitadContrato = mesesTotales / 2;
                Console.WriteLine($"Meses hasta finalizacion: {mesesHastaFinalizacion}");
                Console.WriteLine($"Mitad del contrato: {mitadContrato}");
                if (mesesHastaFinalizacion < mitadContrato)
                {
                    multa = contrato.Monto * 2;
                }
                else
                {
                    multa = contrato.Monto;
                }
                if (mesesHastaFinalizacion < 0)
                {
                    mesesHastaFinalizacion = 0;
                }
                var mesesAPagarRestantes = Math.Ceiling(mesesHastaFinalizacion) - mesesPagados;
                Console.WriteLine($"Meses a pagar restantes: {mesesAPagarRestantes}");

                loQueQueda = (mesesAPagarRestantes * contrato.Monto) + multa;
                Console.WriteLine($"loQueQueda: {loQueQueda}");
                Console.WriteLine($"Multa: {multa}");

            }
            else
            {
                throw new Exception("No hay pagos pendientes");
            }

            if (loQueQueda == 0)
            {
                throw new Exception("No hay pagos pendientes");
            }

            using (var connection = new MySqlConnection(ConnectionString))
            {

                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var sql = @$"INSERT INTO pagos ({nameof(Pago.IdContrato)},
                    {nameof(Pago.Importe)}, {nameof(Pago.DetallePago)},
                    {nameof(Pago.NroPago)}, {nameof(Pago.Estado)},
                    {nameof(Pago.FechaPago)})
                    VALUES
                    (@contratoId, @importe,
                    @detalle, @nroPago,
                    @estado, CURDATE());
                    SELECT LAST_INSERT_ID();";
                        using (var command = new MySqlCommand(sql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@contratoId", contrato.Id);
                            command.Parameters.AddWithValue("@importe", contrato.Monto);
                            command.Parameters.AddWithValue("@detalle", detalle);
                            command.Parameters.AddWithValue("@nroPago", nroPago + 1);
                            command.Parameters.AddWithValue("@estado", 1);

                            nroPago = Convert.ToInt32(command.ExecuteScalar());
                            mesesPagados++;
                            decimal TotalPagado = mesesPagados * contrato.Monto;
                            if (contrato.FechaFinalizacion == null && TotalPagado >= (mesesTotales * contrato.Monto))
                            {
                                contratoPagado = true; //se termino de pagar el contrato
                            }
                            else if (contrato.FechaFinalizacion < contrato.FechaHasta && TotalPagado >= (mesesTotales * contrato.Monto))
                            {
                                contratoPagado = true; //se termino de pagar el contrato con multas
                            }

                            if (contratoPagado)
                            {
                                if (contrato.FechaFinalizacion == null)
                                {
                                    var sql2 = @$"UPDATE contratos SET
                                {nameof(Contrato.FechaFinalizacion)} = {nameof(Contrato.FechaHasta)}
                                WHERE {nameof(Contrato.Id)} = @contratoId;";
                                    using (var command2 = new MySqlCommand(sql2, connection, transaction))
                                    {
                                        command2.Parameters.AddWithValue("@contratoId", contrato.Id);
                                        command2.ExecuteNonQuery();
                                    }
                                }
                            }

                            transaction.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine(ex);
                        return -1;
                    }
                }

            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw new Exception(ex.Message);
        }
        return nroPago;
    }

}

