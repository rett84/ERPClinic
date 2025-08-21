using System.Data.OleDb;
using System.Data;

namespace ERPClinic.Components.Classes
{
    public class CalendarMDBClass
    {
        private readonly IConfiguration _configuration;
        private readonly string _MDBConnString;


        public CalendarMDBClass(IConfiguration configuration)
        {
            _MDBConnString = configuration.GetConnectionString("MDBConnStringJet");
        }

        public async Task<List<CalendarEvent?>> GetAllCalendarEvents()
        {
            var storedProcedureName = "spGetAllCalendarEvents";

            var calendarEvents = new List<CalendarEvent>();

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
                            calendarEvents.Add(new CalendarEvent
                            {
                                id = row.Field<int>("id"),
                                Title = row.Field<string>("Title"),
                                DateTime = row.Field<DateTime>("DateTimeEv"),
                                Details = row.Field<string>("Details"),
                                ClientID = row.Field<int>("ClientID"),
                                Amount = row.Field<decimal?>("Amount"),
                                Paid = row.Field<string>("Paid"),
                                Received = row.Field<decimal?>("Received"),
                                Notes = row.Field<string>("Notes"),
                            });
                        }
                    }
                    conn.Close();
                    conn.Dispose();
                    cmd.Dispose();

                    return calendarEvents;
                }
                catch (OleDbException ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
            }
        }


        public async Task<CalendarEvent?> AddCalendarEvent(CalendarEvent ev)
        {

                var storedProcedureName = "spAddCalendarEvent";


                // Establish connection to DB, define command to execute stored procedure
                using (OleDbConnection conn = new OleDbConnection(_MDBConnString))
                using (OleDbCommand cmd = new OleDbCommand(storedProcedureName, conn))
                {
                    try
                    {
                        await conn.OpenAsync();
                        // Set type of command to stored procedure
                        cmd.CommandType = CommandType.StoredProcedure;

                        var evparameters = new Dictionary<string, object>
                        {
                            { "@ClientID", ev.ClientID },
                            { "@Title", ev.Title ?? string.Empty },
                            { "@DateTime", ev.DateTime },
                            { "@Details", ev.Details ?? string.Empty },
                            { "@Amount", ev.Amount.HasValue ? (object)ev.Amount.Value : DBNull.Value },
                            { "@Paid", ev.Paid ?? string.Empty },
                            { "@Received", ev.Received.HasValue ? (object)ev.Received.Value : DBNull.Value },
                            { "@Notes", ev.Notes ?? string.Empty }
                        };

                        // Now add them to the command
                        foreach (var p in evparameters)
                        {
                            cmd.Parameters.Add(new OleDbParameter(p.Key, p.Value ?? DBNull.Value));
                        }
                        // Execute insert
                        cmd.ExecuteNonQuery();


                        conn.Close();
                        conn.Dispose();
                        cmd.Dispose();

                        return ev;
                    }
                    catch (OleDbException ex)
                    {
                        Console.WriteLine(ex);
                        return null;
                    }
                }
        }

        public async Task<int> DeleteEvent(int id)
        {


            var storedProcedureName = "spDeleteCalendarEvent";


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

        public async Task<CalendarEvent?> UpdateCalendarEvent(CalendarEvent calendarEvent)
        {


            var storedProcedureName = "spUpdateCalendarEvent";


            // Establish connection to DB, define command to execute stored procedure
            using (OleDbConnection conn = new OleDbConnection(_MDBConnString))
            using (OleDbCommand cmd = new OleDbCommand(storedProcedureName, conn))
            {
                try
                {
                    await conn.OpenAsync();
                    // Set type of command to stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    var calendarparemeters = new Dictionary<string, object>
                        {
                            { "@idpar", calendarEvent.id },
                            { "@ClientIDpar", calendarEvent.ClientID },
                            { "@Titlepar", calendarEvent.Title ?? string.Empty },
                            { "@DateTimepar", calendarEvent.DateTime },
                            { "@Detailspar", calendarEvent.Details ?? string.Empty },
                            { "@Amountpar", calendarEvent.Amount.HasValue ? (object)calendarEvent.Amount.Value : 0 },
                            { "@Paidpar", calendarEvent.Paid ?? "No" },
                            { "@Receivedpar", calendarEvent.Received.HasValue ? (object)calendarEvent.Received.Value : 0 },
                            { "@Notespar", calendarEvent.Notes ?? string.Empty }
                        };

                    // Now add them to the command
                    foreach (var p in calendarparemeters)
                    {
                        cmd.Parameters.Add(new OleDbParameter(p.Key, p.Value ?? DBNull.Value));
                    }
                    // Execute insert
                    cmd.ExecuteNonQuery();



                    conn.Close();
                    conn.Dispose();
                    cmd.Dispose();

                    return calendarEvent;
                }
                catch (OleDbException ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
            }

        }


        public async Task<int> UpdateClientStatus(int ClientID, string ClientStatus)
        {


            var storedProcedureName = "spUpdateClientStatus";


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
                            { "@idpar", ClientID },
                            { "@ClientStatuspar", ClientStatus ?? "CC"},
                        };

                    // Now add them to the command
                    foreach (var p in clientParameters)
                    {
                        cmd.Parameters.Add(new OleDbParameter(p.Key, p.Value ?? DBNull.Value));
                    }
                    // Execute insert
                    int rowsUpdated = cmd.ExecuteNonQuery();



                    conn.Close();
                    conn.Dispose();
                    cmd.Dispose();

                    return rowsUpdated;
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
