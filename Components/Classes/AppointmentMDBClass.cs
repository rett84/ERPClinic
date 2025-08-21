using System.Data.OleDb;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ERPClinic.Components.Classes
{
    public class AppointmentMDBClass
    {
        private readonly IConfiguration _configuration;
        private readonly string _MDBConnString;


        public AppointmentMDBClass(IConfiguration configuration)
        {
            _MDBConnString = configuration.GetConnectionString("MDBConnStringJet");

        }

        // Get All Appointments
        public async Task<List<CalendarEvent?>> GetAllAppointments()
        {
            var storedProcedureName = "spGetAllAppointments";

            var appointments = new List<CalendarEvent>();

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
                            appointments.Add(new CalendarEvent
                            {
                                id = row.Field<int>("Calendar.id"),
                                Title = row.Field<string>("Title"),
                                DateTime = row.Field<DateTime>("DateTimeEv"),
                                Details = row.Field<string>("Details"),
                                ClientID = row.Field<int>("ClientID"),
                                Amount = row.Field<decimal?>("Amount"),
                                Paid = row.Field<string>("Paid"),
                                Received = row.Field<decimal?>("Received"),
                                Notes = row.Field<string>("Notes"),
                                Client = new Clients
                                {
                                    id = row.Field<int>("Clients.id"),
                                    Name = row.Field<string>("FullName")
                                }
                            });

                        }
                    }
                    conn.Close();
                    conn.Dispose();
                    cmd.Dispose();

                    return appointments;
                }
                catch (OleDbException ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
            }
        }

        public async Task<List<CalendarEvent?>> GetAppointmentbyClientID(int ClientID)
        {
            var storedProcedureName = "spGetAppointmentbyClientID";

            var appointments = new List<CalendarEvent>();

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
                        { "@ClientIDpar", ClientID },
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

                        foreach (DataRow row in table.Rows)
                        {
                            appointments.Add(new CalendarEvent
                            {
                                id = row.Field<int>("Calendar.id"),
                                Title = row.Field<string>("Title"),
                                DateTime = row.Field<DateTime>("DateTimeEv"),
                                Details = row.Field<string>("Details"),
                                ClientID = row.Field<int>("ClientID"),
                                Amount = row.Field<decimal?>("Amount"),
                                Paid = row.Field<string>("Paid"),
                                Received = row.Field<decimal?>("Received"),
                                Notes = row.Field<string>("Notes"),
                                Client = new Clients
                                {
                                    id = row.Field<int>("Clients.id"),
                                    Name = row.Field<string>("FullName"),
                                    Phone = row.Field<string>("Phone")
                                }
                            });

                        }
                    }


                    conn.Close();
                    conn.Dispose();
                    cmd.Dispose();

                    return appointments;
                }
                catch (OleDbException ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
            }
        }

        public async Task<CalendarEvent?> UpdateAppointment(CalendarEvent appointment)
        {


            var storedProcedureName = "spUpdateAppointment";


            // Establish connection to DB, define command to execute stored procedure
            using (OleDbConnection conn = new OleDbConnection(_MDBConnString))
            using (OleDbCommand cmd = new OleDbCommand(storedProcedureName, conn))
            {
                try
                {
                    await conn.OpenAsync();
                    // Set type of command to stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    var appointmentparameters = new Dictionary<string, object>
                        {
                            { "@idpar", appointment.id },
                            { "@Detailspar", appointment.Details ?? string.Empty },
                            { "@Amountpar", appointment.Amount.HasValue ? (object)appointment.Amount.Value : 0 },
                            { "@Paidpar", appointment.Paid ?? "No" },
                            { "@Receivedpar", appointment.Received.HasValue ? (object)appointment.Received.Value : 0 },
                            { "@Notespar", appointment.Notes ?? string.Empty }
                        };

                    // Now add them to the command
                    foreach (var p in appointmentparameters)
                    {
                        cmd.Parameters.Add(new OleDbParameter(p.Key, p.Value ?? DBNull.Value));
                    }
                    // Execute insert
                    cmd.ExecuteNonQuery();



                    conn.Close();
                    conn.Dispose();
                    cmd.Dispose();

                    return appointment;
                }
                catch (OleDbException ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
            }

        }

    }
}
