using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Try_pls.Data;
using Try_pls.Services;
using Try_pls.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Try_pls.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class HomeController : Controller
    {
        private readonly ContactsdBcontext dbContext;
        private readonly DatabaseServicecs databaseService;

        public HomeController(ContactsdBcontext dbContext, DatabaseServicecs databaseService)
        {
            this.dbContext = dbContext;
            this.databaseService = databaseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetContacts()
        {
            var contacts = await dbContext.Contacts.ToListAsync();
            return Ok(contacts);
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetContact([FromRoute] string id)
        {
            var contact = await dbContext.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }
            return Ok(contact);
        }

        [HttpPost]
        public async Task<IActionResult> AddContact(AddContactRequest addContactRequest)
        {
            var contact = new Contacts()
            {
                Id = Guid.NewGuid().ToString(),
                Addresses = addContactRequest.Addresses,
                Dates = addContactRequest.Dates,
                Deceased = addContactRequest.Deceased,
                Gender = addContactRequest.Gender,
                Names = addContactRequest.Names,
            };

            // Use the DatabaseWriter for the retry and backoff mechanism
            bool success = await databaseService.WriteToDatabaseWithRetry(
                async () =>
                {
                    await dbContext.Contacts.AddAsync(contact);
                    await dbContext.SaveChangesAsync();
                }
            );

            if (success)
            {
                return Ok(contact);
            }
            else
            {
                return StatusCode(500, "Failed to write to the database after multiple attempts.");
            }
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateContact([FromRoute] string id, UpdateContactRequest updateContactRequest)
        {
            var contact = await dbContext.Contacts.FindAsync(id);
            if (contact != null)
            {
                contact.Addresses = updateContactRequest.Addresses;
                contact.Dates = updateContactRequest.Dates;
                contact.Deceased = updateContactRequest.Deceased;
                contact.Gender = updateContactRequest.Gender;
                contact.Names = updateContactRequest.Names;

                await dbContext.SaveChangesAsync();
                return Ok(contact);
            }
            return NotFound();
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteContact([FromRoute] string id)
        {
            var contact = await dbContext.Contacts.FindAsync(id);
            if (contact != null)
            {
                dbContext.Remove(contact);
                await dbContext.SaveChangesAsync();
                return Ok(contact);
            }
            return NotFound();
        }


        [HttpGet]
        [Route("SearchAndFilter")]
        public async Task<IActionResult> SearchAndFilterContacts(
    [FromQuery] string? search,
    [FromQuery] DateTime? startDate,
    [FromQuery] DateTime? endDate,
    [FromQuery] List<string>? countries,
    [FromQuery] string gender = "Male",
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string sortField = "Id",
    [FromQuery] string sortOrder = "asc")
        {
            IQueryable<Contacts> query = dbContext.Contacts;

            // Search
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();

                query = query.Where(contact =>
                    contact.Addresses.Any(address =>
                        (address.Country != null && address.Country.ToLower() == search) ||
                        (address.AddressLine != null && address.AddressLine.ToLower() == search) ||
                        (address.City != null && address.City.ToLower() == search)
                    ) ||
                    contact.Names.Any(name =>
                        (name.FirstName != null && name.FirstName.ToLower() == search) ||
                        (name.MiddleName != null && name.MiddleName.ToLower() == search) ||
                        (name.Surname != null && name.Surname.ToLower() == search)
                    )
                );
            }

            // Filter - Apply filter only if gender is provided
            if (!string.IsNullOrEmpty(gender))
            {
                query = query.Where(contact => contact.Gender != null && contact.Gender.ToLower() == gender.ToLower());
            }
            

            if (startDate.HasValue)
            {
                query = query.Where(contact =>
                    contact.Dates.Any(date =>
                        date.DateValue.HasValue && date.DateValue.Value.Date >= startDate.Value.Date
                    )
                );
            }

            if (endDate.HasValue)
            {
                query = query.Where(contact =>
                    contact.Dates.Any(date =>
                        date.DateValue.HasValue && date.DateValue.Value.Date <= endDate.Value.Date
                    )
                );
            }

            if (countries != null && countries.Any())
            {
                query = query.Where(contact =>
                    contact.Addresses.Any(address =>
                        address.Country != null && countries.Contains(address.Country)
                    )
                );
            }

            // Sorting
            // Validate and sanitize input
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (string.IsNullOrWhiteSpace(sortField))
            {
                sortField = "Id";
            }

            // Ensure valid sortOrder (asc or desc)
            sortOrder = sortOrder.ToLower() == "desc" ? "desc" : "asc";

            // Sorting
            switch (sortField.ToLower())
            {
                case "id":
                    query = sortOrder == "asc" ? query.OrderBy(contact => contact.Id) : query.OrderByDescending(contact => contact.Id);
                    break;
                case "gender":
                    query = sortOrder == "asc" ? query.OrderBy(contact => contact.Gender) : query.OrderByDescending(contact => contact.Gender);
                    break;
                case "firstname":
                    query = sortOrder == "asc" ? query.OrderBy(contact => contact.Names.FirstOrDefault() != null ? contact.Names.First().FirstName : string.Empty) : query.OrderByDescending(contact => contact.Names.FirstOrDefault() != null ? contact.Names.First().FirstName : string.Empty);
                    break;
                case "lastname":
                    query = sortOrder == "asc" ? query.OrderBy(contact => contact.Names.FirstOrDefault() != null ? contact.Names.First().Surname : string.Empty) : query.OrderByDescending(contact => contact.Names.FirstOrDefault() != null ? contact.Names.First().Surname : string.Empty);
                    break;
                default:
                    query = sortOrder == "asc" ? query.OrderBy(contact => contact.Id) : query.OrderByDescending(contact => contact.Id);
                    break;
            }

            // Pagination
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            var result = new
            {
                TotalItems = totalItems,
                TotalPages = totalPages,
                PageSize = pageSize,
                CurrentPage = page,
                Contacts = await query.ToListAsync()
            };

            return Ok(result);
        }


    }
}