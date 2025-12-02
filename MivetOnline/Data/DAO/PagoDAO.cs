using System.Data;
using Microsoft.Data.SqlClient;
using MivetOnline.Data.Interfaces;
using MivetOnline.Models.Pago;
using MivetOnline.Models.Usuario;

namespace MivetOnline.Data.DAO
{
    public class PagoDAO : IPagoDAO
    {
        private readonly string _connectionString;

        public PagoDAO(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Connection string 'DefaultConnection' not found");
        }

        // Listar métodos de pago disponibles (Yape, Efectivo, etc.)
        public async Task<List<PayOpts>> ListarPaymentOptions()
        {
            List<PayOpts> paymentOptions = new List<PayOpts>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_listarPaymentOptions", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                paymentOptions.Add(new PayOpts
                                {
                                    ide_pay = reader.GetInt64(0),
                                    nom_pay = reader.GetString(1)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ListarPaymentOptions: {ex.Message}");
            }

            return paymentOptions;
        }

        // Agregar un nuevo pago y devolver su ID
        public async Task<long?> AgregarPago(DateTime hora, decimal monto, long tipoPago, long idUsuario)
        {
            long? idPago = null;

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_agregarPago", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@hora", hora);
                        cmd.Parameters.AddWithValue("@monto", monto);
                        cmd.Parameters.AddWithValue("@tipopago", tipoPago);
                        cmd.Parameters.AddWithValue("@usuario", idUsuario);

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                idPago = Convert.ToInt64(reader["IdPago"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en AgregarPago: {ex.Message}");
            }

            return idPago;
        }

        // Obtener pago por ID (datos técnicos - Backend)
        public async Task<PagoO?> ObtenerPagoPorId(long id)
        {
            PagoO? pago = null;

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_obtenerPagoPorId", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", id);

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                pago = new PagoO
                                {
                                    IdPago = reader.GetInt64(0),
                                    HoraPago = reader.GetDateTime(1),
                                    MontoPago = reader.GetDecimal(2),
                                    TipoPago = reader.GetInt64(3),
                                    IdCliente = reader.GetInt64(4)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerPagoPorId: {ex.Message}");
            }

            return pago;
        }

        // Obtener pago por ID con información completa (Frontend)
        public async Task<Pago?> ObtenerPagoPorIdFront(long id)
        {
            Pago? pago = null;

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_obtenerPagoPorIdFront", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", id);

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                pago = new Pago
                                {
                                    IdPago = reader.GetInt64(0),
                                    HoraPago = reader.GetDateTime(1),
                                    MontoPago = reader.GetDecimal(2),
                                    TipoPago = reader.GetString(3),
                                    NombreCliente = reader.GetString(4),
                                    CorreoCliente = reader.GetString(5)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerPagoPorIdFront: {ex.Message}");
            }

            return pago;
        }
    }
}