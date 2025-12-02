using System.Data;
using Microsoft.Data.SqlClient;
using MivetOnline.Data.Interfaces;
using MivetOnline.Models.Mascota;
using MivetOnline.Models.Usuario;

namespace MivetOnline.Data.DAO
{
    public class MascotaDAO : IMascotaDAO
    {
        private readonly string _connectionString;

        public MascotaDAO(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Connection string 'DefaultConnection' not found");
        }

        // Agregar una nueva mascota (Cliente registra su mascota)
        public async Task<bool> AgregarMascota(string nombre, string especie, string raza, DateTime fechaNac, long idUsuario)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_agregarMascota", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@nombre", nombre);
                        cmd.Parameters.AddWithValue("@especie", especie);
                        cmd.Parameters.AddWithValue("@raza", raza ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@fecha_nac", fechaNac);
                        cmd.Parameters.AddWithValue("@id_usuario", idUsuario);

                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                        return true;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error en AgregarMascota: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general en AgregarMascota: {ex.Message}");
                return false;
            }
        }

        // Listar mascotas de un cliente específico (por ID de usuario)
        public async Task<List<Mascota>> ListarMascotasPorCliente(long idUsuario)
        {
            List<Mascota> mascotas = new List<Mascota>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_listarMascotasPorCliente", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_usuario", idUsuario);

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                mascotas.Add(new Mascota
                                {
                                    IdMascota = reader.GetInt64(0),
                                    Nombre = reader.GetString(1),
                                    Especie = reader.GetString(2),
                                    Raza = reader.IsDBNull(3) ? null : reader.GetString(3),
                                    FechaNacimiento = reader.GetDateTime(4)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ListarMascotasPorCliente: {ex.Message}");
            }

            return mascotas;
        }

        // Actualizar datos de una mascota
        public async Task<bool> ActualizarMascota(long idMascota, string nombre, string especie, string raza, DateTime fechaNac)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_actualizarMascota", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@id_mascota", idMascota);
                        cmd.Parameters.AddWithValue("@nombre", nombre);
                        cmd.Parameters.AddWithValue("@especie", especie);
                        cmd.Parameters.AddWithValue("@raza", raza ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@fecha_nac", fechaNac);

                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ActualizarMascota: {ex.Message}");
                return false;
            }
        }

        // Eliminar una mascota
        public async Task<string> EliminarMascota(long idMascota)
        {
            string mensaje = "Error al eliminar la mascota";

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_eliminarMascota", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_mascota", idMascota);

                        await conn.OpenAsync();
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                mensaje = reader.GetString(0);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en EliminarMascota: {ex.Message}");
                mensaje = $"Error: {ex.Message}";
            }

            return mensaje;
        }
    }
}
