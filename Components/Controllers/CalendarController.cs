using Azure.Core;

using ERPClinic.Components.Classes;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ERPClinic.Components.Controllers
{
    [Route("api/calendar")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CalendarController> _logger;
        private readonly DBSelection _dbSelection;
        private readonly CalendarMDBClass _calendarMDBClass;
        private readonly ClientsMDBClass _clientsMDBClass;

        public CalendarController(AppDbContext context, ILogger<CalendarController> logger,
            DBSelection dBSelection, CalendarMDBClass calendarMDBClass, ClientsMDBClass clientsMDBClass)
        {
            _context = context;
            _logger = logger;
            _dbSelection = dBSelection;
            _calendarMDBClass = calendarMDBClass;
            _clientsMDBClass = clientsMDBClass;

        }

        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            if (_dbSelection.EF_Enabled)
            {

                try
                {
                    var events = _context.CalendarEvents
                                   .ToList();

                    return Ok(events);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    _logger.LogError(ex, "Error retrieving calendar events");
                    return null;
                }
            }

            var calendarEvents = await _calendarMDBClass.GetAllCalendarEvents();
            if (calendarEvents !=null)
                return Ok(calendarEvents);

            return NoContent();


        }

        //Add Calendar Record
        [HttpPost]
        public async Task<IActionResult> AddEvent([FromBody] CalendarEvent evt)
        {
            if (_dbSelection.EF_Enabled)
            {
                try
                {

                    var record = await _context.Clients.FindAsync(evt.ClientID);
                    if (record == null) return NotFound();

                    record.ClientStatus = evt.Client?.ClientStatus;

                    //prevent EF from tracking a new Client
                    evt.Client = null;


                    _context.CalendarEvents.Add(evt);
                    await _context.SaveChangesAsync();
                    return Ok(evt);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    _logger.LogError(ex, "adding calendar event");
                    return NotFound();
                }
            }

          //  var addEv = await _calendarMDBClass.AddCalendarEvent(evt);
          //  var updateClientstatus = await _calendarMDBClass.UpdateClientStatus(evt.ClientID, evt.Client.ClientStatus ?? "IP");
            if (await _calendarMDBClass.AddCalendarEvent(evt) !=null);
                if (await _calendarMDBClass.UpdateClientStatus(evt.ClientID, evt.Client.ClientStatus?? "IP") >0)
                    return Ok(evt);
            return NotFound();
        }

        //public class DeleteRequest
        //{
        //    public int id { get; set; }
        //}

        //Delete Record from Calendar
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
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

        //Update Calendar Record
        [HttpPut]
        public async Task<IActionResult> UpdateEvent([FromBody] CalendarEvent updated)
        {
            if (_dbSelection.EF_Enabled)
            {
                var record = await _context.CalendarEvents.FindAsync(updated.id);
                if (record == null) return NotFound();

                record.Title = updated.Title;
                record.DateTime = updated.DateTime;
                record.Notes = updated.Notes;
                record.ClientID = updated.ClientID;

                await _context.SaveChangesAsync();
                return Ok(record);
            }

           var updatedEvent = await _calendarMDBClass.UpdateCalendarEvent(updated);
           if(updatedEvent != null)
                return Ok(updated);
            return NotFound();
        }

        //Get List of Clients to Populate List
        [HttpGet("/api/clients/names")]
        public async Task<IActionResult> GetClientNames()
        {
            if (_dbSelection.EF_Enabled)
            {
                try
                {

                    var names = await _context.Clients
                        //.Select(c => new { c.id, c.Name })
                        .OrderBy(c => c.Name)
                        .ToListAsync();

                    return Ok(names);
                }
                catch (Exception ex)
                {
                    // Log the error and return 500
                    _logger.LogError(ex, "Failed to load client names");
                    return StatusCode(500, "Internal server error");
                }
            }

            var clients = (await _clientsMDBClass.GetAllClients()).OrderBy(c => c.Name);

            if (clients != null)
                return Ok(clients);
            return NoContent();



        }
    }
}
