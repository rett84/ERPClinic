using System.IO;
using System.Text;

using ERPClinic.Components.Classes;
using ERPClinic.Components.Pages;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using static ERPClinic.Components.Controllers.CalendarController;

namespace ERPClinic.Components.Controllers
{
    [Route("api/clients")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CalendarController> _logger;
        private  readonly bool _EF_enabled = false; // Flag to enable/disable EF functionality
        private readonly ClientsMDBClass _clientsMDBClass;
        private DBSelection _dbSelection;

        public ClientsController(AppDbContext context, ILogger<CalendarController> logger, ClientsMDBClass clientsMDBClass, DBSelection dBSelection)
        {
            _context = context;
            _logger = logger;
            _dbSelection = dBSelection;
            _clientsMDBClass = clientsMDBClass;
        }


        //  Get All Clients Records
        [HttpGet]
        public async Task<IActionResult> GetClients()
        {
            try
            {
                if (_dbSelection.EF_Enabled)
                {
                    var records = _context.Clients
                        .OrderBy(c => c.Name).
                        ToList();

                    return Ok(records);
                }

                var clients = await _clientsMDBClass.GetAllClients();
                return Ok(clients);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(500, ex.ToString());
            }
        }

        //Delete Record from Clients
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {

            if (_dbSelection.EF_Enabled)
            {
                var client = await _context.Clients.FindAsync(id);
                if (client == null) return NotFound();

                try
                {
                    _context.Clients.Remove(client);
                    await _context.SaveChangesAsync();
                    return NoContent();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return StatusCode(500, ex.ToString());
                }
            }
            else
            {
                if (await _clientsMDBClass.DeleteClient(id) > 0)
                    return NoContent();
                return NotFound();

            }
        }

        //Add Client Record
        [HttpPost]
        public async Task<IActionResult> AddClient([FromBody] Clients client)
        {
            try
            {
                if (_dbSelection.EF_Enabled)
                {
                    //    Attempt to find an existing Client Name
                    var existingEntity = _context.Clients.FirstOrDefault(e => e.Name == client.Name);
                    if (existingEntity == null)
                    {
                        _context.Clients.Add(client);
                        await _context.SaveChangesAsync();
                        return Ok(client);
                    }
                    else
                    {
                        return BadRequest("Client already exists.");
                    }
                }
                else
                {
                    var clientAdd = _clientsMDBClass.AddClient(client);
                    if (clientAdd != null)
                        return Ok(client);

                    return BadRequest("Client already exists.");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        //Update Client Record
        [HttpPut]
        public async Task<IActionResult> UpdateClient([FromBody] Clients updated)
        {
            if (_dbSelection.EF_Enabled)
            {
                var record = await _context.Clients.FindAsync(updated.id);
                if (record == null) return NotFound();

                record.Name = updated.Name;
                record.DOB = updated.DOB;
                record.Sex = updated.Sex;
                record.Phone = updated.Phone;
                record.Address = updated.Address;
                record.City = updated.City;
                record.State = updated.State;
                record.PostalCode = updated.PostalCode;
                if (updated.ClientStatus != null)
                    record.ClientStatus = updated.ClientStatus;



                await _context.SaveChangesAsync();
                return Ok(record);
            }

            var clientAdd = _clientsMDBClass.UpdateClient(updated);
            if (clientAdd != null)
                return Ok(updated);

            return NotFound();

        }


        //Save Odontograma to DB
        [HttpPost("/api/clients/uploadimage")]
        public async Task<IActionResult> UploadImage([FromForm] string imageJson, [FromForm] int id)
        {
           // if (file == null || file.Length == 0)
            //    return BadRequest("No file uploaded");

            using var ms = new MemoryStream();
          //  await file.CopyToAsync(ms);

            if (_dbSelection.EF_Enabled)
            {
                //TODO: old code, in SQL needs to convet to save as JSON
                var client = await _context.Clients.FindAsync(id);
                if (client == null)
                    return NotFound("Client not found.");


                client.Odontograma = imageJson;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Image saved successfully" });
            }
            else
            {
                var updated = await _clientsMDBClass.UpdateImage(id, imageJson);
                if (updated>0)
                    return Ok(new { message = "Image saved successfully" });

                return NoContent();
            }
        }

        //Send Odontograma Image to frontend
        [HttpGet("/api/clients/getimage/{id}")]
        [HttpHead("/api/clients/getimage/{id}")]
        public async Task<IActionResult> GetImage(int id)
        {
            var odontogramaJson = TemplateOdontograma(@"wwwroot/Images/odontogramaTemplate.jpg");

            if (_dbSelection.EF_Enabled)
            {
                //TODO: old code, in SQL needs to convet to load as JSON
                var client = await _context.Clients.FindAsync(id);
                if (client == null)
                    return NotFound("Client not found.");


                if (string.IsNullOrEmpty(client.Odontograma))
                    return Ok(odontogramaJson);

                //From DB
                return Ok(client.Odontograma);

            }
            else
            {

                var client = await _clientsMDBClass.GetClientbyID(id);
                //If there is no Odontograma for the client in the DB, display template so a new Odontograma can be saved
                //in the client record.
                if (string.IsNullOrEmpty(client.Odontograma))
                    return Ok(odontogramaJson);

                //From DB
                return Ok(client.Odontograma);

            }


        }

        private object TemplateOdontograma(string filePath)
        {
            byte[] imageBytes = System.IO.File.ReadAllBytes(filePath);
            string base64String = Convert.ToBase64String(imageBytes);

            // Return a full Fabric.js JSON structure
            var fabricJson = new
            {
               // version = "5.2.4", // use your Fabric.js version
                objects = new object[] { },
                backgroundImage = new
                {
                    type = "image",
                   // version = "5.2.4",
                    originX = "left",
                    originY = "top",
                    left = 0,
                    top = 0,
                    scaleX = 1,
                    scaleY = 1,
                    src = $"data:image/jpeg;base64,{base64String}",
                    crossOrigin = "anonymous"
                }
            };

            return fabricJson;
        }
    }
}
