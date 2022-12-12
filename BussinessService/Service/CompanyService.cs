using Data.Contracts;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Data.DTO;

namespace BussinessService.Service
{
    public class CompanyService
    {
        private readonly IServiceLayer _company;

        public CompanyService(IServiceLayer company)
        {
            _company = company;
        }

     
        //GET All Company Details   
        public async Task<IEnumerable<Company>> GetCompaniesList()
        {
            try

            {
                return await _company.GetCompanies();

            }
            catch (Exception)
            {
                throw;
            }
        }

        // get by id
        public async Task<Company> GetCompanybyid(int id)
        {
            return await _company.GetCompany(id);
        }

        //Insert
        public async Task<Company> CreateCompany(CompanyForCreationDto company)
        {
            return await _company.CreateCompany(company);
        }

        //Update
        public async Task UpdateCompany(int id, CompanyForUpdateDto company)
        {
            await _company.UpdateCompany(id, company);

        }

        //Delete
        public async Task DeleteCompany(int id)
        {
            await _company.DeleteCompany(id);

        }


///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //Using Procedures

        //getall 
        public async Task<IEnumerable<Company>> GetCompanypro()
        {
            try

            {
                return await _company.GetCompanypro();

            }
            catch (Exception)
            {
                throw;
            }
        }

        //get by id
        public async Task<Company> GetCompanybyidpro(int id)
        {
            try

            {
                return await _company.GetCompanyprobyid(id);
            }
            catch (Exception)
            {
                throw;
            }

        }
        public async Task CreateCompanyprocedure(CompanyForCreationDto company)
        {
            await _company.CreateCompanypro(company);
        }

        public async Task UpdateCompanypro(int id, CompanyForUpdateDto company) 
        {
            await _company.UpdateCompanypro(id, company);

        }

        public async Task DeleteCompanypro(int id) 
        {
            await _company.DeleteCompanypro(id);

        }


    }
}
