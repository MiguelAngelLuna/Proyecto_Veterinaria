using Microsoft.Data.SqlClient;
using VeterinariaWebApp.Data.Interfaces;
using VeterinariaWebApp.Models.Usuario;
using System.Data;

namespace VeterinariaWebApp.Data.DAO
{
    public class UsuarioDAO : IUsuarioDAO
    {
        private readonly string _connectionString;

        public UsuarioDAO(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Connection string 'DefaultConnection' not found");
        }

        // Verificar login y obtener rol del usuario
        public async Task<string?> VerificarLogin(string correo, string contraseña)
        {
            string? rol = null;

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_verificarLogin", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@correo", correo);
                        cmd.Parameters.AddWithValue("@contraseña", contraseña);

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                rol = reader.GetString(0);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en VerificarLogin: {ex.Message}");
            }

            return rol;
        }

        // Obtener ID de usuario por correo
        public async Task<long?> ObtenerIdUsuario(string correo)
        {
            long? idUsuario = null;

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_obtenerIdUsuario", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@correo", correo);

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                idUsuario = reader.GetInt64(0);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerIdUsuario: {ex.Message}");
            }

            return idUsuario;
        }

        // Listar tipos de documento 
        public async Task<List<UserDoc>> ListarDocumentos()
        {
            List<UserDoc> documentos = new List<UserDoc>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_listarDocumentos", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                documentos.Add(new UserDoc
                                {
                                    ide_doc = reader.GetInt64(0),
                                    nom_doc = reader.GetString(1)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ListarDocumentos: {ex.Message}");
            }

            return documentos;
        }
    }
}