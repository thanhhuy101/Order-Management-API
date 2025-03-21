using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Order_Management.Data
{
    public class DbCommandRepository : IDisposable
    {
        private readonly OrderDbContext _context;

        public DbCommandRepository(OrderDbContext context)
        {
            _context = context;
        }

        public async Task<DataTable> ExecuteStoredProcedure(string procedureName, params SqlParameter[] parameters)
        {
            var connection = _context.Database.GetDbConnection() as SqlConnection;
            var command = new SqlCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddRange(parameters);

            var dataTable = new DataTable();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            using (var reader = await command.ExecuteReaderAsync())
            {
                dataTable.Load(reader);
            }

            return dataTable;
        }

        public async Task<int> ExecuteNonQuery(string procedureName, params SqlParameter[] parameters)
        {
            var connection = _context.Database.GetDbConnection() as SqlConnection;
            var command = new SqlCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddRange(parameters);

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            return await command.ExecuteNonQueryAsync();
        }
        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
