using Data.DTO;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Data.Contracts
{
   public interface IServiceLayer // where T is Model Class(Company)
    {
        public Task<IEnumerable<Company>> GetCompanies();

        public Task<Company> GetCompany(int id);

        public Task<Company> CreateCompany(CompanyForCreationDto company);
        public Task UpdateCompany(int id, CompanyForUpdateDto company);
        public Task DeleteCompany(int id);

        // Using Stored Procedure

        public Task<IEnumerable<Company>> GetCompanypro();
        public Task<Company> GetCompanyprobyid(int id);
        public Task CreateCompanypro(CompanyForCreationDto company);
        public Task UpdateCompanypro(int id, CompanyForUpdateDto company);
        public Task DeleteCompanypro(int id);

    }
}
