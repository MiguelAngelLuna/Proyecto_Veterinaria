using System.Data;
using Microsoft.Data.SqlClient;
using MivetOnline.Data.Interfaces;
using MivetOnline.Models.Usuario;
using MivetOnline.Models.Usuario.Cliente;

namespace MivetOnline.Data.DAO
{
    public class ClienteDAO : IClienteDAO
    {
        private readonly string _connectionString;

        public ClienteDAO(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Connection string 'DefaultConnection' not found");
        }

        // Agregar un nuevo cliente (Registro - Frontend)
        public async Task<bool> AgregarCliente(UsuarioO usuario)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_agregarCliente", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@cor", usuario.cor_usr ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@pwd", usuario.pwd_usr ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@nom", usuario.nom_usr ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ape", usuario.ape_usr ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ndo", usuario.num_doc ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@fna", usuario.fna_usr);
                        cmd.Parameters.AddWithValue("@doc", usuario.ide_doc);

                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en AgregarCliente: {ex.Message}");
                return false;
            }
        }

        // Listar clientes para tabla frontend (sin datos sensibles)
        public async Task<List<Cliente>> ListarClientesFront()
        {
            List<Cliente> clientes = new List<Cliente>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_listarClientesFront", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                clientes.Add(new Cliente
                                {
                                    IdCliente = reader.GetInt64(0),
                                    NombreUsuario = reader.GetString(1),
                                    ApellidoUsuario = reader.GetString(2),
                                    FechaNacimiento = reader.GetDateTime(3),
                                    TipoDocumento = reader.GetString(4),
                                    NumeroDocumento = reader.GetString(5),
                                    Rol = reader.GetString(6)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ListarClientesFront: {ex.Message}");
            }

            return clientes;
        }

        // Listar clientes con datos completos (Backend)
        public async Task<List<ClienteO>> ListarClientesBack()
        {
            List<ClienteO> clientes = new List<ClienteO>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_listarClientesBack", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                clientes.Add(new ClienteO
                                {
                                    ide_usr = reader.GetInt64(0),
                                    ide_cli = reader.GetInt64(1),
                                    cor_usr = reader.GetString(2),
                                    pwd_usr = reader.GetString(3),
                                    nom_usr = reader.GetString(4),
                                    ape_usr = reader.GetString(5),
                                    fna_usr = reader.GetDateTime(6),
                                    num_doc = reader.GetString(7),
                                    ide_doc = reader.GetInt64(8),
                                    ide_rol = reader.GetInt64(9)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ListarClientesBack: {ex.Message}");
            }

            return clientes;
        }

        // Actualizar datos del cliente
        public async Task<bool> ActualizarCliente(long id, UsuarioO usuario)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_actualizarCliente", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@cor", usuario.cor_usr ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@pwd", usuario.pwd_usr ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@nom", usuario.nom_usr ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ape", usuario.ape_usr ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ndo", usuario.num_doc ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@fna", usuario.fna_usr);
                        cmd.Parameters.AddWithValue("@doc", usuario.ide_doc);

                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ActualizarCliente: {ex.Message}");
                return false;
            }
        }

        // Buscar cliente por ID
        public async Task<Cliente?> BuscarCliente(long id)
        {
            Cliente? cliente = null;

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_buscarCliente", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", id);

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                cliente = new Cliente
                                {
                                    IdCliente = reader.GetInt64(0),
                                    NombreUsuario = reader.GetString(1),
                                    ApellidoUsuario = reader.GetString(2),
                                    FechaNacimiento = reader.GetDateTime(3),
                                    TipoDocumento = reader.GetString(4),
                                    NumeroDocumento = reader.GetString(5),
                                    Rol = reader.GetString(6)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en BuscarCliente: {ex.Message}");
            }

            return cliente;
        }

        // Eliminar cliente
        public async Task<bool> EliminarCliente(long id)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_eliminarCliente", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", id);

                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en EliminarCliente: {ex.Message}");
                return false;
            }
        }

        // Listar citas de un cliente específico (por ID de usuario)
        public async Task<List<CitaCliente>> ListarCitasPorCliente(long ideUsuario)
        {
            List<CitaCliente> citas = new List<CitaCliente>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("sp_listarCitasPorCliente", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ide_usr", ideUsuario);

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
    }
}