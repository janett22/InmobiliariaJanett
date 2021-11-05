using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaJanett.Models
{
    public class RepositorioPago : RepositorioBase, IRepositorioPago
    {
        public RepositorioPago(IConfiguration configuration) : base(configuration)
        {

        }

        public int Alta(Pago entidad, int id)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"INSERT INTO Pagos (IdContrato,NroPago,Fecha,Importe) " +
                    "VALUES (@idContrato, @nroPago, @fecha, @importe);" +
                    "SELECT SCOPE_IDENTITY();";//devuelve el id insertado (LAST_INSERT_ID para mysql)
                using (var command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idContrato", entidad.IdContrato);
                    command.Parameters.AddWithValue("@nroPago", entidad.NroPago);
                    command.Parameters.AddWithValue("@fecha", entidad.Fecha);
                    command.Parameters.AddWithValue("@importe", entidad.Fecha);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    entidad.Id = res;
                    connection.Close();
                }
            }
            return res;

        }

        public int Baja(int id)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"DELETE FROM Pago WHERE Id = {id}";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public int Modificacion(Pago entidad)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "UPDATE Pagos SET " +
                    "IdContrato=@idContrato, NroPago=@NroPago, Fecha=@fecha, Importe=@importe " +
                    "WHERE Id = @id";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idContrato", entidad.IdContrato);
                    command.Parameters.AddWithValue("@NroPago", entidad.NroPago);
                    command.Parameters.AddWithValue("@fecha", entidad.Fecha);
                    command.Parameters.AddWithValue("@Importe", entidad.Importe);
                    command.Parameters.AddWithValue("@id", entidad.Id);
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Pago> ObtenerTodos()
        {
            IList<Pago> res = new List<Pago>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "Select c.Id, c.FechaInicio, c.FechaFin, c.InquilinoId, c.InmuebleId, i.Nombre, i.Apellido, " +
                             " inm.Direccion, inm.IdPropietario, " +
                             " p.Nombre, p.Apellido, pa.id, pa.NroPago, pa.Fecha, pa.Importe, pa.IdContrato, c.Precio " +
                             " FROM Contratos c " +
                             " INNER JOIN Inquilinos i ON i.IdInquilino = c.InquilinoId " +
                             " INNER JOIN Inmuebles inm ON inm.Id = c.InmuebleId " +
                             " INNER JOIN Propietarios p ON p.IdPropietario = inm.IdPropietario " +
                             " INNER JOIN Pagos pa ON pa.IdContrato = c.Id ";

 using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Pago pago = new Pago
                        {
                            Id = reader.GetInt32(11),
                            NroPago = reader.GetInt32(12),
                            Fecha = reader.GetDateTime(13),
                            Importe = reader.GetDecimal(14),
                            IdContrato = reader.GetInt32(0),

                            Contrato = new Contrato
                            {
                                Id = reader.GetInt32(0),
                                FechaInicio = reader.GetDateTime(1),
                                FechaFin = reader.GetDateTime(2),
                                Precio = reader.GetDecimal(16),
                                InquilinoId = reader.GetInt32(3),
                                InmuebleId = reader.GetInt32(4),

                                Inquilino = new Inquilino
                                {
                                    Id = reader.GetInt32(3),
                                    Nombre = reader.GetString(5),
                                    Apellido = reader.GetString(6),
                                },

                                Inmueble = new Inmueble
                                {
                                    Id = reader.GetInt32(4),
                                    Direccion = reader.GetString(7),
                                    IdPropietario = reader.GetInt32(8),

                                    Duenio = new Propietario
                                    {
                                        Id = reader.GetInt32(8),
                                        Nombre = reader.GetString(9),
                                        Apellido = reader.GetString(10),
                                    }
                                }
                            },




                        };
                        res.Add(pago);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public Pago ObtenerPorId(int id)
        {

            Pago entidad = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "Select c.Id, c.FechaInicio, c.FechaFin, c.InquilinoId, c.InmuebleId, i.Nombre, i.Apellido, " +
                                " inm.Direccion, inm.IdPropietario, " +
                                " p.Nombre, p.Apellido, pa.id, pa.NroPago, pa.Fecha, pa.Importe, pa.IdContrato" +
                                " FROM Contratos c " +
                                " INNER JOIN Inquilinos i ON i.IdInquilino = c.InquilinoId " +
                                " INNER JOIN Inmuebles inm ON inm.Id = c.InmuebleId " +
                                " INNER JOIN Propietarios p ON p.IdPropietario = inm.IdPropietario " +
                                " INNER JOIN Pagos pa ON pa.IdContrato = c.Id ";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        entidad = new Pago
                        {
                            Id = reader.GetInt32(11),
                            NroPago = reader.GetInt32(12),
                            Fecha = reader.GetDateTime(13),
                            Importe = reader.GetDecimal(14),
                            IdContrato = reader.GetInt32(0),

                            Contrato = new Contrato
                            {
                                Id = reader.GetInt32(0),
                                FechaInicio = reader.GetDateTime(1),
                                FechaFin = reader.GetDateTime(2),
                                InquilinoId = reader.GetInt32(3),
                                InmuebleId = reader.GetInt32(4),

                                Inquilino = new Inquilino
                                {
                                    Id = reader.GetInt32(3),
                                    Nombre = reader.GetString(5),
                                    Apellido = reader.GetString(6),
                                },

                                Inmueble = new Inmueble
                                {
                                    Id = reader.GetInt32(4),
                                    Direccion = reader.GetString(7),
                                    IdPropietario = reader.GetInt32(8),

                                    Duenio = new Propietario
                                    {
                                        Id= reader.GetInt32(8),
                                        Nombre = reader.GetString(9),
                                        Apellido = reader.GetString(10),
                                    }
                                }
                            },
                        };
                    }
                    connection.Close();
                }
            }
            return entidad;
        }

        public IList<Pago> BuscarPorContrato(int id)
        {
            List<Pago> res = new List<Pago>();
            Pago entidad = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "Select c.Id, c.FechaInicio, c.FechaFin, c.InquilinoId, c.InmuebleId, i.Nombre, i.Apellido, " +
                                " inm.Direccion, inm.IdPropietario, " +
                                " p.Nombre, p.Apellido, pa.id, pa.NroPago, pa.Fecha, pa.Importe, pa.IdContrato" +
                                " FROM Contratos c " +
                                " INNER JOIN Inquilinos i ON i.IdInquilino = c.InquilinoId " +
                                " INNER JOIN Inmuebles inm ON inm.Id = c.InmuebleId " +
                                " INNER JOIN Propietarios p ON p.IdPropietario = inm.IdPropietario " +
                                " INNER JOIN Pagos pa ON pa.IdContrato = c.Id " +
                                " WHERE c.id = @id";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        entidad = new Pago
                        {
                            Id = reader.GetInt32(11),
                            NroPago = reader.GetInt32(12),
                            Fecha = reader.GetDateTime(13),
                            Importe = reader.GetDecimal(14),
                            IdContrato = reader.GetInt32(0),

                            Contrato = new Contrato
                            {
                                Id = reader.GetInt32(0),
                                FechaInicio = reader.GetDateTime(1),
                                FechaFin = reader.GetDateTime(2),
                                InquilinoId = reader.GetInt32(3),
                                InmuebleId = reader.GetInt32(4),

                                Inquilino = new Inquilino
                                {
                                    Id = reader.GetInt32(3),
                                    Nombre = reader.GetString(5),
                                    Apellido = reader.GetString(6),
                                },

                                Inmueble = new Inmueble
                                {
                                    Id = reader.GetInt32(4),
                                    Direccion = reader.GetString(7),
                                    IdPropietario = reader.GetInt32(8),

                                    Duenio = new Propietario
                                    {
                                        Id = reader.GetInt32(8),
                                        Nombre = reader.GetString(9),
                                        Apellido = reader.GetString(10),
                                    }
                                }
                            },
                        };
                    res.Add(entidad);
                    }
                    }
                    connection.Close();
            }
            return res;
        }

        public int Alta(Pago p)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"INSERT INTO Pagos (IdContrato,NroPago,Fecha,Importe) " +
                    "VALUES (@idContrato, @nroPago, @fecha, @importe);" +
                    "SELECT SCOPE_IDENTITY();";//devuelve el id insertado (LAST_INSERT_ID para mysql)
                using (var command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idContrato", p.IdContrato);
                    command.Parameters.AddWithValue("@nroPago", p.NroPago);
                    command.Parameters.AddWithValue("@fecha", p.Fecha);
                    command.Parameters.AddWithValue("@importe", p.Importe);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    p.Id = res;
                    connection.Close();
                }
            }
            return res;

        }
    }
}

