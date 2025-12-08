using System.Data;
using Microsoft.Data.SqlClient;
using VeterinariaWebApp.Data.Interfaces;
using VeterinariaWebApp.Models.Cita;
using VeterinariaWebApp.Models.Usuario;

namespace VeterinariaWebApp.Data.DAO
{
    public class CitaDAO : ICitaDAO
    {
        private readonly string _connectionString;

        public CitaDAO(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Connection string 'DefaultConnection' not found");
        }

        // Listar citas (datos técnicos - Backend)
        public async Task<List<CitaO>> ListarCitasBack()
        {
            List<CitaO> citas = new List<CitaO>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_listarCitasBack", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                citas.Add(new CitaO
                                {
                                    IdCita = reader.GetInt64(0),
                                    CalendarioCita = reader.GetDateTime(1),
                                    Consultorio = reader.GetInt64(2),
                                    IdVeterinario = reader.GetInt64(3),
                                    IdMascota = reader.GetInt64(4),
                                    IdPago = reader.GetInt64(5)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ListarCitasBack: {ex.Message}");
            }

            return citas;
        }

        // Listar citas (con información completa - Frontend)
        public async Task<List<Cita>> ListarCitasFront()
        {
            List<Cita> citas = new List<Cita>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_listarCitasFront", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                citas.Add(new Cita
                                {
                                    IdCita = reader.GetInt64(0),
                                    CalendarioCita = reader.GetDateTime(1),
                                    Consultorio = reader.GetInt64(2),
                                    NombreVeterinario = reader.GetString(3),
                                    NombreMascota = reader.GetString(4),
                                    MontoPago = reader.GetDecimal(5)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ListarCitasFront: {ex.Message}");
            }

            return citas;
        }

        // Listar citas de un cliente específico
        public async Task<List<CitaCliente>> ListarCitasPorCliente(int idUsuario)
        {
            List<CitaCliente> citas = new List<CitaCliente>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_listarCitasPorCliente", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_usuario", idUsuario);

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                citas.Add(new CitaCliente
                                {
                                    ide_cit = reader.GetInt64(0),
                                    cal_cit = reader.GetDateTime(1),
                                    con_cit = reader.GetInt32(2),
                                    veterinario = reader.GetString(3),
                                    especialidad = reader.GetString(4),
                                    mascota = reader.GetString(5),
                                    especie = reader.GetString(6),
                                    mon_pag = reader.GetDecimal(7)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ListarCitasPorCliente: {ex.Message}");
            }

            return citas;
        }

        // Agregar una nueva cita
        public async Task<bool> AgregarCita(DateTime calendario, int consultorio, long idVeterinario, long idMascota, long idPago)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_agregarCita", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@calendario", calendario);
                        cmd.Parameters.AddWithValue("@consultorio", consultorio);
                        cmd.Parameters.AddWithValue("@veterinario", idVeterinario);
                        cmd.Parameters.AddWithValue("@mascota", idMascota);
                        cmd.Parameters.AddWithValue("@pago", idPago);

                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en AgregarCita: {ex.Message}");
                return false;
            }
        }

        // Actualizar una cita existente
        public async Task<bool> ActualizarCita(long idCita, DateTime calendario, int consultorio, long idVeterinario, long idMascota, long idPago)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_actualizarCita", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@idCita", idCita);
                        cmd.Parameters.AddWithValue("@calendario", calendario);
                        cmd.Parameters.AddWithValue("@consultorio", consultorio);
                        cmd.Parameters.AddWithValue("@veterinario", idVeterinario);
                        cmd.Parameters.AddWithValue("@mascota", idMascota);
                        cmd.Parameters.AddWithValue("@pago", idPago);

                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ActualizarCita: {ex.Message}");
                return false;
            }
        }

        // Eliminar una cita
        public async Task<bool> EliminarCita(long idCita)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_eliminarCita", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@idCita", idCita);

                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en EliminarCita: {ex.Message}");
                return false;
            }
        }

        // Obtener citas por fecha específica
        public async Task<List<Cita>> ObtenerCitasPorFecha(int dia, int mes, int año)
        {
            List<Cita> citas = new List<Cita>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_obtenerCitasPorFecha", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@dia", dia);
                        cmd.Parameters.AddWithValue("@mes", mes);
                        cmd.Parameters.AddWithValue("@año", año);

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                citas.Add(new Cita
                                {
                                    IdCita = reader.GetInt64(0),
                                    CalendarioCita = reader.GetDateTime(1),
                                    Consultorio = reader.GetInt64(2),
                                    NombreVeterinario = reader.GetString(3),
                                    NombreMascota = reader.GetString(4),
                                    MontoPago = reader.GetDecimal(5)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerCitasPorFecha: {ex.Message}");
            }

            return citas;
        }
    }
}