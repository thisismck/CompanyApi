using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CompanyApi.Data;
using CompanyApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace CompanyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class CompaniesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CompaniesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
        {
            return await _context.Companies.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetCompanyById(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return company;
        }

        [HttpGet("isin/{isin}")]
        public async Task<ActionResult<Company>> GetCompanyByIsin(string isin)
        {
            if (isin == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(m => m.Isin == isin);
            if (company == null)
            {
                return NotFound();
            }

            return company;
        }

        [HttpPost]
        public async Task<ActionResult<Company>> CreateCompany(Company company)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if ( await _context.Companies.AnyAsync(c => c.Isin == company.Isin))
            {
                return BadRequest("Company with this ISIN already exists");
            }
            _context.Add(company);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCompanyById), new { id = company.Id }, company);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, Company company)
        {
            if (id != company.Id)
            {
                return BadRequest();
            }

                try
                {
                    _context.Update(company);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return NoContent();
        }

        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.Id == id);
        }
    }
}
