using BussinessService.Service;
using Data.Contracts;
using Data.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dapper_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly CompanyService _companyService;
        private readonly IServiceLayer _company;
        public ServiceController(IServiceLayer company, CompanyService companyService)
        {
            _companyService = companyService;
            _company = company;
        }


        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            try
            {
                var companies = await _companyService.GetCompaniesList();
                return Ok(companies);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompany(int id)
        {
            try
            {
                var company = await _companyService.GetCompanybyid(id);
                if (company == null)
                    return NotFound();
                return Ok(company);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompany(CompanyForCreationDto company)
        {
            try
            {
                var createdCompany = await _companyService.CreateCompany(company);
                return CreatedAtRoute("CompanyById", new { id = createdCompany.Id }, createdCompany);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany(int id, CompanyForUpdateDto company)
        {
            try
            {
                var dbCompany = await _companyService.GetCompanybyid(id);
                if (dbCompany == null)
                    return NotFound();
                await _companyService.UpdateCompany(id, company);
                return NoContent();
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            try
            {
                var dbCompany = await _companyService.GetCompanybyid(id);
                if (dbCompany == null)
                    return NotFound();
                await _companyService.DeleteCompany(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////////

        //using SP


        [HttpGet("allcompanypro")]
        public async Task<IActionResult> GetCompanyPro()
        {
            try
            {
                var company = await _companyService.GetCompanypro();
                if (company == null)
                    return NotFound();
                return Ok(company);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("pro/{id}")]
        public async Task<IActionResult> GetCompanyproid(int id)
        {
            try
            {
                var company = await _companyService.GetCompanybyidpro(id);
                if (company == null)
                    return NotFound();
                return Ok(company);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        //Insert
        [HttpPost("pro")]
        public async Task<IActionResult> CreateCompanypros(CompanyForCreationDto company)
        {
            try
            {
                await _companyService.CreateCompanyprocedure(company);
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        //update
        [HttpPut("pro/{id}")]
        public async Task<IActionResult> UpdateCompanyprobyid(int id, CompanyForUpdateDto company)
        {
            try
            {
                var dbCompany = await _companyService.GetCompanybyidpro(id);
                if (dbCompany == null)
                    return NotFound();
                await _companyService.UpdateCompanypro(id, company);
                return NoContent();
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        //delete
        [HttpDelete("pro/{id}")]
        public async Task<IActionResult> DeleteCompanyproid(int id)
        {
            try
            {
                var dbCompany = await _companyService.GetCompanybyidpro(id);
                if (dbCompany == null)
                    return NotFound();
                await _companyService.DeleteCompanypro(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

    }
}
