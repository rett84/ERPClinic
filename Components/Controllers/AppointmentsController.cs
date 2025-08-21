using System.Threading.Tasks;

using ERPClinic.Components.Classes;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERPClinic.Components.Controllers
{
    [Route("api/appointments")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AppointmentsController> _logger;
        private readonly DBSelection _dbSelection;
        private readonly AppointmentMDBClass _appointmentMDBClass;
        private readonly CalendarMDBClass _calendarMDBClass;

        public AppointmentsController(AppDbContext context, ILogger<AppointmentsController> logger,
            DBSelection dbSelection, AppointmentMDBClass appointmentMDBClass, CalendarMDBClass calendarMDBClass)
        {
            _context = context;
            _logger = logger;
            _dbSelection = dbSelection;
            _appointmentMDBClass = appointmentMDBClass;
            _calendarMDBClass = calendarMDBClass;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointments(int id)
        {
            if (_dbSelection.EF_Enabled)
            {
                try
                {
                    //joins the corresponding client record to the calendar event, so the Name can be displayed
                    var appointments = _context.CalendarEvents.Include(e => e.Client).Where(r => r.ClientID == id)
                                   .ToList();

                    return Ok(appointments);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    _logger.LogError(ex, "Error retrieving appointments events");
                    return NotFound();
                }
            }
            var appointmentsMDB = await _appointmentMDBClass.GetAppointmentbyClientID(id);
            if (appointmentsMDB != null)
                return Ok(appointmentsMDB);
            return NoContent();

        }


        [HttpGet("/api/appointments/all")]
        public async Task<IActionResult> GetAppointmentsAll()
        {
            if(_dbSelection.EF_Enabled)
            {
                try
                {
                    //joins the corresponding client record to the calendar event, so the Name can be displayed
                    var appointments = _context.CalendarEvents.Include(e => e.Client)
                                   .ToList();

                    return Ok(appointments);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    _logger.LogError(ex, "Error retrieving appointments events");
                    return NotFound();
                }
            }
            var appointmentsMDB = await _appointmentMDBClass.GetAllAppointments();
            if (appointmentsMDB != null)
                return Ok(appointmentsMDB);
            return NoContent();

        }

        //Delete Record from Calendar
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {

            if(_dbSelection.EF_Enabled)
            {
                var ev = await _context.CalendarEvents.FindAsync(id);
                if (ev == null) return NotFound();

                _context.CalendarEvents.Remove(ev);
                await _context.SaveChangesAsync();
                return NoContent();
            }

            if (await _calendarMDBClass.DeleteEvent(id) > 0)
                return NoContent();

            return NotFound();

        }


        ////Add Appointment Record
        //[HttpPost]
        //public async Task<IActionResult> AddEvent([FromBody] CalendarEvent appointment)
        //{
        //    try
        //    {
        //        //evt.Amount = 0;
        //        //evt.Received = 0;

        //        _context.CalendarEvents.Add(appointment);
        //        await _context.SaveChangesAsync();
        //        return Ok(appointment);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());
        //        _logger.LogError(ex, "adding calendar event");
        //        return NotFound();
        //    }
        //}

        //Update Appointment Record
        [HttpPut]
        public async Task<IActionResult> UpdateEvent([FromBody] CalendarEvent updated)
        {
            if(_dbSelection.EF_Enabled)
            {
                try
                {
                    var record = await _context.CalendarEvents.FindAsync(updated.id);
                    if (record == null) return NotFound();

                    record.Amount = updated.Amount;
                    record.Paid = updated.Paid;
                    record.Received = updated.Received;
                    record.Notes = updated.Notes;
                    record.Details = updated.Details;

                    await _context.SaveChangesAsync();
                    return Ok(record);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    _logger.LogError(ex, "Updating appointment");
                    return NotFound();
                }
               
            }

            var updatedappointment = await _appointmentMDBClass.UpdateAppointment(updated);
            if (updatedappointment != null)
                return Ok(updatedappointment);
            return NotFound();


        }

        // Update Client Status Record
        [HttpPut("/api/appointments/clientStatus")]
        public async Task<IActionResult> UpdateClientStatus([FromBody] Clients updated)
        {
            if (_dbSelection.EF_Enabled)
            {
                var record = await _context.Clients.FindAsync(updated.id);
                if (record == null) return NotFound();


                record.ClientStatus = updated.ClientStatus;



                await _context.SaveChangesAsync();
                return Ok(record);
            }


            var clientStatus = await _calendarMDBClass.UpdateClientStatus(updated.id, updated.ClientStatus);
            if (clientStatus>0)
                return Ok(updated);
            return NotFound();

        }
    }
}
