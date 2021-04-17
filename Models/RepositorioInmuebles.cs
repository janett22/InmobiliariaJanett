using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaJanett.Models
{
	public class RepositorioInmuebles : RepositorioBase
	{
		public RepositorioInmuebles(IConfiguration configuration) : base(configuration)
		{

		}

	public int Alta(Inmueble entidad)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"INSERT INTO Inmuebles (Direccion, Ambientes, Superficie, IdPropietario) " +
					"VALUES (@direccion, @ambientes, @superficie, @Idpropietario);" +
					"SELECT SCOPE_IDENTITY();";//devuelve el id insertado (LAST_INSERT_ID para mysql)
				using (var command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@direccion", entidad.Direccion);
					command.Parameters.AddWithValue("@ambientes", entidad.Ambientes);
					command.Parameters.AddWithValue("@superficie", entidad.Superficie);
					command.Parameters.AddWithValue("@IdPropietario", entidad.IdPropietario);
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
				string sql = $"DELETE FROM Inmuebles WHERE Id = {id}";
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
		public int Modificacion(Inmueble entidad)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = "UPDATE Inmuebles SET " +
					"Direccion=@direccion, Ambientes=@ambientes, Superficie=@superficie, IdPropietario=@Idpropietario " +
					"WHERE Id = @id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@direccion", entidad.Direccion);
					command.Parameters.AddWithValue("@ambientes", entidad.Ambientes);
					command.Parameters.AddWithValue("@superficie", entidad.Superficie);
					command.Parameters.AddWithValue("@IdPropietario", entidad.IdPropietario);
					command.Parameters.AddWithValue("@id", entidad.Id);
					command.CommandType = CommandType.Text;
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public IList<Inmueble> ObtenerTodos()
		{
			IList<Inmueble> res = new List<Inmueble>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = "SELECT Id, Direccion, Ambientes, Superficie, i.IdPropietario," +
					" p.Nombre, p.Apellido" +
					" FROM Inmuebles i INNER JOIN Propietarios p ON i.IdPropietario = p.IdPropietario";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inmueble entidad = new Inmueble
						{
							Id = reader.GetInt32(0),
							Direccion = reader.GetString(1),
							Ambientes = reader.GetInt32(2),
							Superficie = reader.GetInt32(3),
							IdPropietario = reader.GetInt32(4),
							Duenio = new Propietario
							{
								IdPropietario = reader.GetInt32(4),
								Nombre = reader.GetString(5),
								Apellido = reader.GetString(6),
							}
						};
						res.Add(entidad);
					}
					connection.Close();
				}
			}
			return res;
		}

		public Inmueble ObtenerPorId(int id)
		{
			Inmueble entidad = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT Id, Direccion, Ambientes, Superficie, I.IdPropietario, p.Nombre, p.Apellido" +
					$" FROM Inmuebles i INNER JOIN Propietarios p ON i.IdPropietario = p.IdPropietario" +
					$" WHERE Id=@id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@id", SqlDbType.Int).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						entidad = new Inmueble
						{
							Id = reader.GetInt32(0),
							Direccion = reader.GetString(1),
							Ambientes = reader.GetInt32(2),
							Superficie = reader.GetInt32(3),
							IdPropietario = reader.GetInt32(4),
							Duenio = new Propietario
							{
								IdPropietario = reader.GetInt32(4),
								Nombre = reader.GetString(5),
								Apellido = reader.GetString(6),
							}
						};
					}
					connection.Close();
				}
			}
			return entidad;
		}


	}
}
