using ERPClinic.Components.Classes;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERPClinic.Components.Controllers
{
    [Route("api/expenses")]
    [ApiController]
    public class ExpensesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AppointmentsController> _logger;
        private readonly DBSelection _dbSelection;
        private readonly ExpensesMDBClass _expensesMDBClass;

        public ExpensesController(AppDbContext context, ILogger<AppointmentsController> logger,
            DBSelection dbSelection, ExpensesMDBClass expensesMDBClass)
        {
            _context = context;
            _logger = logger;
            _dbSelection = dbSelection;
            _expensesMDBClass = expensesMDBClass;
        }

        [HttpGet]
        public async Task<IActionResult> GetExpensesall()
        {
            if (_dbSelection.EF_Enabled)
            {
                try
                {
                    var expenses = _context.Expenses.Include(e => e.Expense)
                                   .ToList();

                    return Ok(expenses);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    _logger.LogError(ex, "Error retrieving expenses");
                    return NotFound();
                }
            }
            // If EF is not enabled, use the MDB class to get expenses
            var expensesMDB = await _expensesMDBClass.GetAllExpenses();
            if (expensesMDB != null)
                return Ok(expensesMDB);
            return NoContent();



        }

        [HttpGet("/api/getexpensesID")]
        public async Task<IActionResult> GetExpenseID()
        {
            if(_dbSelection.EF_Enabled)
            {
                try
                {
                    var expenseID = _context.ExpensesID
                                   .ToList();

                    return Ok(expenseID);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    _logger.LogError(ex, "Error retrieving expenses IDs");
                    return NotFound();
                }
            }
            var expenseIDMDB = await _expensesMDBClass.GetExpensesID();
            if (expenseIDMDB != null)
                return Ok(expenseIDMDB);
            return NoContent();
        }

        //Delete Expense
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {

            if(_dbSelection.EF_Enabled)
            {
                var expense = await _context.Expenses.FindAsync(id);
                if (expense == null) return NotFound();

                _context.Expenses.Remove(expense);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            var expensedeletedMDB = await _expensesMDBClass.DeleteExpense(id);
            if (expensedeletedMDB > 0)
                return Ok(expensedeletedMDB);
            return NotFound();

        }


        //Add Expense Record
        [HttpPost]
        public async Task<IActionResult> AddEvent([FromBody] Expenses expense)
        {
            if(_dbSelection.EF_Enabled)
            {
                try
                {

                    _context.Expenses.Add(expense);
                    await _context.SaveChangesAsync();
                    return Ok(expense);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return null;
                }
            }
            var expenseAdded = await _expensesMDBClass.AddExpense(expense);
            if (expenseAdded != null)
                return Ok(expenseAdded);
            return NoContent();

        }

        //Update Expense Record
        [HttpPut]
        public async Task<IActionResult> UpdateEvent([FromBody] Expenses updated)
        {
            if (_dbSelection.EF_Enabled)
            {
                var record = await _context.Expenses.FindAsync(updated.id);
                if (record == null) return NotFound();

                record.ExpenseID = updated.ExpenseID;
                record.Date = updated.Date;
                record.Notes = updated.Notes;
                record.Amount = updated.Amount;
                record.Paid = updated.Paid;




                await _context.SaveChangesAsync();
                return Ok(record);
            }

            var expenseUpdated = await _expensesMDBClass.UpdateExpense(updated);
            if (expenseUpdated >0)
                return Ok(expenseUpdated);
            return NotFound();


        }

    }
}
