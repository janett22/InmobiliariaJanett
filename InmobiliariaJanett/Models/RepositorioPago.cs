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

        public int Alta(Pago entidad)
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
                    string sql = $"SELECT Id, NroPago, Fecha, Importe, IdContrato, c.FechaInicio, c.FechaFin" +
                    $" FROM Pagos p INNER JOIN Contratos c ON p.IdContrato= c,Id" +
                    $" WHERE Id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;
                        connection.Open();
                        var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            
                            Pago entidad = new Pago
                            {
                                Id = reader.GetInt32(0),
                                NroPago = reader.GetInt32(1),
                                Fecha = reader.GetDateTime(2),
                                Importe = reader.GetInt32(3),
                                IdContrato = reader.GetInt32(4),
                                contrato = new Contrato
                                {
                                    Id = reader.GetInt32(4),
                                    FechaInicio = reader.GetDateTime(5),
                                    FechaFin = reader.GetDateTime(6),
                                }
                            };
                            res.Add(entidad);
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
                string sql = $"SELECT Id, NroPago, Fecha, Importe, IdContrato, c.FechaInicio, c.FechaFin" +
                    $" FROM Pagos p INNER JOIN Contratos c ON p.IdContrato= c,Id" +
                    $" WHERE Id=@id";
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
                            Id = reader.GetInt32(0),
                           NroPago = reader.GetInt32(1),
                           Fecha = reader.GetDateTime(2),
                            Importe = reader.GetInt32(3),
                            IdContrato = reader.GetInt32(4),
                            contrato = new Contrato
                            {
                                Id= reader.GetInt32(4),
                                FechaInicio = reader.GetDateTime(5),
                                FechaFin = reader.GetDateTime(6),
                            }
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
                string sql = $"SELECT p.Id,p.NroPago, p.FechaPago,p.Importe,p.ContratoId,c.InquilinoId,InmuebleId" +
                    $" FROM Pago p INNER JOIN Contrato c ON p.ContratoId = c.Id" +
                    $" WHERE ContratoId=@id";
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
                            Id = reader.GetInt32(0),
                            NroPago = reader.GetInt32(1),
                            Fecha = reader.GetDateTime(2),
                            Importe = reader.GetDecimal(3),
                            IdContrato = reader.GetInt32(4),
                             contrato = new Contrato
                            {
                                Id = reader.GetInt32(4),
                                InquilinoId = reader.GetInt32(5),
                                InmuebleId = reader.GetInt32(6),
                            }
                        };
                        res.Add(entidad);
                    }
                    connection.Close();
                }
            }
            return res;
        }
    }
}
