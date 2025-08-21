using System.Data;
using System.Data.OleDb;

namespace ERPClinic.Components.Classes
{
    public class ExpensesMDBClass
    {
        private readonly IConfiguration _configuration;
        private readonly string _MDBConnString;


        public ExpensesMDBClass(IConfiguration configuration)
        {
            _MDBConnString = configuration.GetConnectionString("MDBConnStringJet");
        }

        // Get All Expenses
        public async Task<List<Expenses?>> GetAllExpenses()
        {
            var storedProcedureName = "spGetAllExpenses";
            var expenses = new List<Expenses>();
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
                            expenses.Add(new Expenses
                            {
                                id = row.Field<int>("Expenses.id"),
                                Date = row.Field<string>("DateTime"),
                                ExpenseID = row.Field<int>("ExpenseID"),
                                Amount = row.Field<decimal?>("Amount"),
                                Notes = row.Field<string>("Notes"),
                                Paid = row.Field<string>("Paid"),
                                Expense = (new ExpenseID
                                {
                                    id = row.Field<int>("e.id"),
                                    Expense = row.Field<string>("Expense"),
                                    Despesa = row.Field<string>("Despesa")
                                })
                            });
                        }
                    }
                    conn.Close();
                    conn.Dispose();
                    cmd.Dispose();
                    return expenses;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return null;
                }
            }
        }

        // Get ExpensesIDs
        public async Task<List<ExpenseID?>> GetExpensesID()
        {
            var storedProcedureName = "spGetExpensesID";
            var expensesID = new List<ExpenseID>();
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
                            expensesID.Add(new ExpenseID
                            {
                                id = row.Field<int>("id"),
                                Expense = row.Field<string>("Expense"),
                                Despesa = row.Field<string>("Despesa")
                            });


                        }
                    }
                    conn.Close();
                    conn.Dispose();
                    cmd.Dispose();
                    return expensesID;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return null;
                }
            }
        }

        public async Task<int> UpdateExpense(Expenses expense)
        {

            var storedProcedureName = "spUpdateExpense";


            // Establish connection to DB, define command to execute stored procedure
            using (OleDbConnection conn = new OleDbConnection(_MDBConnString))
            using (OleDbCommand cmd = new OleDbCommand(storedProcedureName, conn))
            {
                try
                {
                    await conn.OpenAsync();
                    // Set type of command to stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    var expenseparameters = new Dictionary<string, object>
                        {
                            { "@idpar", expense.id },
                            { "@ExpenseIDpar", expense.ExpenseID },
                            { "@Datepar", expense.Date },
                            { "@Amountpar", expense.Amount.HasValue ? (object)expense.Amount.Value : 0 },
                            { "@Paidpar", expense.Paid ?? "No" },
                            { "@Notespar", expense.Notes ?? string.Empty }
                        };

                    // Now add them to the command
                    foreach (var p in expenseparameters)
                    {
                        cmd.Parameters.Add(new OleDbParameter(p.Key, p.Value ?? DBNull.Value));
                    }
                    // Execute update
                    int updatedexpenses = cmd.ExecuteNonQuery();



                    conn.Close();
                    conn.Dispose();
                    cmd.Dispose();

                    return updatedexpenses;
                }
                catch (OleDbException ex)
                {
                    Console.WriteLine(ex);
                    return 0;
                }
            }

        }

        public async Task<Expenses?> AddExpense(Expenses expense)
        {

            var storedProcedureName = "spAddExpense";


            // Establish connection to DB, define command to execute stored procedure
            using (OleDbConnection conn = new OleDbConnection(_MDBConnString))
            using (OleDbCommand cmd = new OleDbCommand(storedProcedureName, conn))
            {
                try
                {
                    await conn.OpenAsync();
                    // Set type of command to stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    var expenseparameters = new Dictionary<string, object>
                        {
                            { "@ExpenseIDpar", expense.ExpenseID },
                            { "@Datepar", expense.Date },
                            { "@Amountpar", expense.Amount.HasValue ? (object)expense.Amount.Value : 0 },
                            { "@Paidpar", expense.Paid ?? "No" },
                            { "@Notespar", expense.Notes ?? string.Empty }
                        };

                    // Now add them to the command
                    foreach (var p in expenseparameters)
                    {
                        cmd.Parameters.Add(new OleDbParameter(p.Key, p.Value ?? DBNull.Value));
                    }
                    // Execute insert
                    cmd.ExecuteNonQuery();



                    conn.Close();
                    conn.Dispose();
                    cmd.Dispose();

                    return expense;
                }
                catch (OleDbException ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
            }

        }

        public async Task<int> DeleteExpense(int id)
        {

            var storedProcedureName = "spDeleteExpense";


            // Establish connection to DB, define command to execute stored procedure
            using (OleDbConnection conn = new OleDbConnection(_MDBConnString))
            using (OleDbCommand cmd = new OleDbCommand(storedProcedureName, conn))
            {
                try
                {
                    await conn.OpenAsync();
                    // Set type of command to stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    var expenseparameters = new Dictionary<string, object>
                        {
                            { "@idpar", id },
                        };

                    // Now add them to the command
                    foreach (var p in expenseparameters)
                    {
                        cmd.Parameters.Add(new OleDbParameter(p.Key, p.Value ?? DBNull.Value));
                    }
                    // Execute delete
                    int deleteddexpense = cmd.ExecuteNonQuery();



                    conn.Close();
                    conn.Dispose();
                    cmd.Dispose();

                    return deleteddexpense;
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
