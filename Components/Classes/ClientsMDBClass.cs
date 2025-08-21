using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Text;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ERPClinic.Components.Classes
{
    public class ClientsMDBClass
    {

        private readonly IConfiguration _configuration;
        private readonly string _MDBConnString;



        public ClientsMDBClass(IConfiguration configuration) // Updated constructor to accept List<Clients>
        {

            _MDBConnString = configuration.GetConnectionString("MDBConnStringJet");

        }

        public async Task<List<Clients>> GetAllClients()
        {
            var storedProcedureName = "spGetAllClients";

            var clientsList = new List<Clients>();

            // Establish connection to DB, define command to execute stored procedure
            using (OleDbConnection conn = new OleDbConnection(_MDBConnString))
            using (OleDbCommand cmd = new OleDbCommand(storedProcedureName, conn))
            {
                try
                {
                    await conn.OpenAsync();
                    // Set type of command to stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    DataSet ds = new DataSet();

                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                    {
                        adapter.Fill(ds, "table");
                        // Access the DataTable
                        var table = ds.Tables["table"];

                        foreach (DataRow row in table.Rows)
                        {
                            clientsList.Add(new Clients
                            {
                                id = row.Field<int>("id"),
                                Name = row.Field<string>("FullName"),
                                Phone = row.Field<string>("Phone"),
                                DOB = DateOnly.FromDateTime(row.Field<DateTime>("DOB")),
                                Sex = row.Field<string>("Sex"),
                                Address = row.Field<string>("Address"),
                                City = row.Field<string>("City"),
                                PostalCode = row.Field<string>("PostalCode"),
                                State = row.Field<string>("State"),
                                ClientStatus = row.Field<string>("ClientStatus"),
                                Odontograma = row.Field<string>("Odontograma")
                            });
                        }
                    }
                    conn.Close();
                    conn.Dispose();
                    cmd.Dispose();

                    return clientsList;
                }
                catch (OleDbException ex)
                {
                    Console.WriteLine(ex);
                    return new List<Clients>();
                }
            }
        }



        public async Task<Clients?> AddClient(Clients client)
        {
            // Await the task to get the actual list of clients
            var allClients = await GetAllClients();

            // Check if the client already exists in the list
            if (!allClients.Any(c => c.Name == client.Name))
            {
                var storedProcedureName = "spAddClient";


                // Establish connection to DB, define command to execute stored procedure
                using (OleDbConnection conn = new OleDbConnection(_MDBConnString))
                using (OleDbCommand cmd = new OleDbCommand(storedProcedureName, conn))
                {
                    try
                    {
                        await conn.OpenAsync();
                        // Set type of command to stored procedure
                        cmd.CommandType = CommandType.StoredProcedure;

                        var clientParameters = new Dictionary<string, object>
                        {
                            { "@Name", client.Name },
                            { "@Phone", client.Phone },
                            { "@DOB", client.DOB.HasValue ? client.DOB.Value.ToDateTime(TimeOnly.MinValue) : DBNull.Value },
                            { "@Sex", client.Sex },
                            { "@Address", client.Address },
                            { "@City", client.City },
                            { "@PostalCode", client.PostalCode },
                            { "@State", client.State },
                            { "@ClientStatus", client.ClientStatus },
                            { "@Odontograma", client.Odontograma }
                        };

                        // Now add them to the command
                        foreach (var p in clientParameters)
                        {
                            cmd.Parameters.Add(new OleDbParameter(p.Key, p.Value ?? DBNull.Value));
                        }
                        // Execute insert
                        cmd.ExecuteNonQuery();


                        conn.Close();
                        conn.Dispose();
                        cmd.Dispose();

                        return client;
                    }
                    catch (OleDbException ex)
                    {
                        Console.WriteLine(ex);
                        return null;
                    }
                }
            }

            return null;
        }


        public async Task<int> DeleteClient(int id)
        {


            var storedProcedureName = "spDeleteClient";


            // Establish connection to DB, define command to execute stored procedure
            using (OleDbConnection conn = new OleDbConnection(_MDBConnString))
            using (OleDbCommand cmd = new OleDbCommand(storedProcedureName, conn))
            {
                try
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new OleDbParameter("@id", id));
                    await conn.OpenAsync();
                    // Set type of command to stored procedure
                    int rowsDeleted = cmd.ExecuteNonQuery();


                    conn.Close();
                    conn.Dispose();
                    cmd.Dispose();

                    return rowsDeleted;
                }
                catch (OleDbException ex)
                {
                    Console.WriteLine(ex);
                    return 0;
                }
            }

        }



        public async Task<Clients?> UpdateClient(Clients client)
        {


            var storedProcedureName = "spUpdateClient";


            // Establish connection to DB, define command to execute stored procedure
            using (OleDbConnection conn = new OleDbConnection(_MDBConnString))
            using (OleDbCommand cmd = new OleDbCommand(storedProcedureName, conn))
            {
                try
                {
                    await conn.OpenAsync();
                    // Set type of command to stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    var clientParameters = new Dictionary<string, object>
                        {
                            { "@idpar", client.id },
                            { "@Namepar", client.Name },
                            { "@Phonepar", client.Phone },
                            { "@DOBpar", client.DOB.HasValue ? client.DOB.Value.ToDateTime(TimeOnly.MinValue) : DBNull.Value },
                            { "@Sexpar", client.Sex },
                            { "@Addresspar", client.Address },
                            { "@Citypar", client.City },
                            { "@PostalCodepar", client.PostalCode },
                            { "@Statepar", client.State },
                            { "@ClientStatuspar", client.ClientStatus },
                            { "@Odontogramapar", client.Odontograma }
                        };

                    // Now add them to the command
                    foreach (var p in clientParameters)
                    {
                        cmd.Parameters.Add(new OleDbParameter(p.Key, p.Value ?? DBNull.Value));
                    }
                    // Execute insert
                    cmd.ExecuteNonQuery();



                    conn.Close();
                    conn.Dispose();
                    cmd.Dispose();

                    return client;
                }
                catch (OleDbException ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
            }

        }


        public async Task<Clients?> GetClientbyID(int id)
        {
            var storedProcedureName = "spGetClientbyID";

            Clients client = new Clients();

            // Establish connection to DB, define command to execute stored procedure
            using (OleDbConnection conn = new OleDbConnection(_MDBConnString))
            using (OleDbCommand cmd = new OleDbCommand(storedProcedureName, conn))
            {
                try
                {
                    await conn.OpenAsync();
                    // Set type of command to stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    var clientParameters = new Dictionary<string, object>
                    {
                        { "@id", id },
                    };

                    // Now add them to the command
                    foreach (var p in clientParameters)
                    {
                        cmd.Parameters.Add(new OleDbParameter(p.Key, p.Value ?? DBNull.Value));
                    }

                    DataSet ds = new DataSet();

                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                    {
                        adapter.Fill(ds, "table");
                        // Access the DataTable
                        var table = ds.Tables["table"];

                        if (table.Rows.Count > 0)
                        {
                            client.id = table.Rows[0].Field<int>("id");
                            client.Name = table.Rows[0].Field<string>("FullName");
                            client.Phone = table.Rows[0].Field<string>("Phone");
                            client.DOB = DateOnly.FromDateTime(table.Rows[0].Field<DateTime>("DOB"));
                            client.Sex = table.Rows[0].Field<string>("Sex");
                            client.Address = table.Rows[0].Field<string>("Address");
                            client.City = table.Rows[0].Field<string>("City");
                            client.PostalCode = table.Rows[0].Field<string>("PostalCode");
                            client.State = table.Rows[0].Field<string>("State");
                            client.ClientStatus = table.Rows[0].Field<string>("ClientStatus");
                            client.Odontograma = table.Rows[0].Field<string>("Odontograma");
                        }
                    }
                    conn.Close();
                    conn.Dispose();
                    cmd.Dispose();

                    return client;
                }
                catch (OleDbException ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
            }
        }

        public async Task<int> UpdateImage(int id, string imageJson)
        {


            var storedProcedureName = "spUpdateImage";


            // Establish connection to DB, define command to execute stored procedure
            using (OleDbConnection conn = new OleDbConnection(_MDBConnString))
            using (OleDbCommand cmd = new OleDbCommand(storedProcedureName, conn))
            {
                try
                {
                    await conn.OpenAsync();
                    // Set type of command to stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    var clientParameters = new Dictionary<string, object>
                        {
                            { "@idpar", id },
                            { "@Odontogramapar", imageJson } //Convert JsonString to bytes
                        };

                    // Now add them to the command
                    foreach (var p in clientParameters)
                    {
                        cmd.Parameters.Add(new OleDbParameter(p.Key, p.Value ?? DBNull.Value));
                    }
                    // Execute insert
                    int updatedrowCount = cmd.ExecuteNonQuery();



                    conn.Close();
                    conn.Dispose();
                    cmd.Dispose();

                    return updatedrowCount;
                }
                catch (OleDbException ex)
                {
                    Console.WriteLine(ex);
                    return 0;
                }
            }

        }


    }
}

