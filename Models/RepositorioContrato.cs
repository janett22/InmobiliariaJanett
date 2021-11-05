using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;



namespace InmobiliariaJanett.Models
{
    public class RepositorioContrato : RepositorioBase, IRepositorioContrato
    {
        public RepositorioContrato(IConfiguration configuration) : base(configuration)
        {

        }

        public int Alta(Contrato c)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"INSERT INTO Contratos (InquilinoId,InmuebleId,FechaInicio,FechaFin,Precio) " +
                        "VALUES ( @inquilinoId, @inmuebleId,@fechaInIcio,@fechaFin,@precio);" +
                        "SELECT SCOPE_IDENTITY();";//devuelve el id insertado (LAST_INSERT_ID para mysql)
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@inquilinoId", c.InquilinoId);
                    command.Parameters.AddWithValue("@inmuebleId", c.InmuebleId);
                    command.Parameters.AddWithValue("@fechaInicio", c.FechaInicio);
                    command.Parameters.AddWithValue("@fechaFin", c.FechaFin);
                    command.Parameters.AddWithValue("@precio", c.Precio);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    c.Id = res;
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
                string sql = $"DELETE FROM Contratos WHERE Id = @id";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public int Modificacion(Contrato entidad)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "UPDATE Contratos SET " +
                           "FechaFin=@fechaFin, InquilinoId=@inquilinoId, InmuebleId=@inmuebleId " +
                           "WHERE Id = @id";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@fechaFin", entidad.FechaFin);
                    command.Parameters.AddWithValue("@inquilinoId", entidad.InquilinoId);
                    command.Parameters.AddWithValue("@inmuebleId", entidad.InmuebleId);
                    command.Parameters.AddWithValue("@id", entidad.Id);
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            } 
        
                return res;
        }

        public IList<Contrato> ObtenerTodos()
        {
            IList<Contrato> res = new List<Contrato>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT c.Id, c.FechaInicio, c.FechaFin, c.Precio, c.InquilinoId, I.Nombre, I.Apellido, c.Id, Inmuebles.direccion, Inmuebles.IdPropietario  " +
                    $" FROM Contratos c INNER JOIN Inquilinos i ON i.Id = C.InquilinoId " +
                    " INNER JOIN Inmuebles Inmuebles ON Inmuebles.id = c.InmuebleId ";


                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Contrato c = new Contrato
                        {
                            Id = reader.GetInt32(0),
                            FechaInicio = reader.GetDateTime(1),
                            FechaFin = reader.GetDateTime(2),
                            Precio = reader.GetDecimal(3),

                            InquilinoId = reader.GetInt32(4),
                            Inquilino = new Inquilino
                            {
                                Id = reader.GetInt32(4),
                                Nombre = reader.GetString(5),
                                Apellido = reader.GetString(6),
                            },
                            InmuebleId = reader.GetInt32(7),
                            Inmueble = new Inmueble
                            {
                                Id = reader.GetInt32(7),
                                Direccion = reader.GetString(8),
                                IdPropietario = reader.GetInt32(9),
                            },
                        };
                        res.Add(c);

                    };
                    connection.Close();
                }
                return res;
            }
        }

    public Contrato ObtenerPorId(int id)
        {
            Contrato entidad = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "SELECT c.Id, c.FechaInicio, c.FechaFin, c.InquilinoId, i.Nombre, i.Apellido, c.InmuebleId, inmuebles.direccion, inmuebles.IdPropietario, c.Precio" +
                              " FROM Contratos c INNER JOIN Inquilinos i ON i.IdInquilino = c.InquilinoId" +
                              " INNER JOIN Inmuebles inmuebles ON  inmuebles.Id = c.InmuebleId" +
                              " WHERE c.Id=@id";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        entidad = new Contrato
                        {
                            Id = reader.GetInt32(0),
                            FechaInicio = reader.GetDateTime(1),
                            FechaFin = reader.GetDateTime(2),
                            Precio = reader.GetDecimal(9),

                            InquilinoId = reader.GetInt32(3),
                            Inquilino = new Inquilino
                            {
                                Id = reader.GetInt32(3),
                                Nombre = reader.GetString(4),
                                Apellido = reader.GetString(5),
                            },

                            InmuebleId = reader.GetInt32(6),
                            Inmueble = new Inmueble
                            {
                                Id = reader.GetInt32(6),
                                Direccion = reader.GetString(7),
                                IdPropietario = reader.GetInt32(8),
                            }

                        };
                    }
                    connection.Close();
                }
            }
            return entidad;
        }

        public IList<Contrato> ContratosVigentes(DateTime fechaInicio, DateTime fechaFin)
        {
            IList<Contrato> res = new List<Contrato>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "SELECT c.Id, c.FechaInicio, c.FechaFin, c.InquilinoId, i.Nombre, i.Apellido, c.InmuebleId, inm.direccion, inm.IdPropietario" +
                              " FROM Contratos c INNER JOIN Inquilinos i ON i.IdInquilino = c.InquilinoId" +
                              " INNER JOIN Inmuebles inm ON  inm.Id = c.InmuebleId" +
                              " WHERE FechaInicio <= @fechaFin AND FechaFin >= @fechaInicio;";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@fechaInicio", SqlDbType.Date).Value = fechaInicio;
                    command.Parameters.Add("@fechaFin", SqlDbType.Date).Value = fechaFin;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Contrato entidad = new Contrato
                        {
                            Id = reader.GetInt32(0),
                            FechaInicio = reader.GetDateTime(1),
                            FechaFin = reader.GetDateTime(2),

                            InquilinoId = reader.GetInt32(3),
                            Inquilino = new Inquilino
                            {
                                Id = reader.GetInt32(3),
                                Nombre = reader.GetString(4),
                                Apellido = reader.GetString(5),
                            },

                            InmuebleId = reader.GetInt32(6),
                            Inmueble = new Inmueble
                            {
                                Id = reader.GetInt32(6),
                                Direccion = reader.GetString(7),
                                IdPropietario = reader.GetInt32(8),
                            }
                        };

                        res.Add(entidad);
                    }
                    connection.Close();
                }
            }


            return res;
        }


        public Inquilino ObtenerPorEmail(string email)
        {
            throw new NotImplementedException();
        }


    }

}
    

    
