using Dapper;
using Data.Context;
using Data.Contracts;
using Data.DTO;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
   public class ServiceLayerRepo : IServiceLayer
    {
        private readonly DapperContext _context;
        public ServiceLayerRepo(DapperContext context)
        {
            _context = context;
        }

        //get all
        public async Task<IEnumerable<Company>> GetCompanies()
        {
            var query = "SELECT * FROM Company";

            // var query = "SELECT Id, Name AS CompanyName, Address, Country FROM Companies";// As you can see, we are using the AS keyword to create an alias for the Name column.

            using (var connection = _context.CreateConnection())
            {
                var companies = await connection.QueryAsync<Company>(query);
                return companies.ToList();
            }
        }

        //get by id
        public async Task<Company> GetCompany(int id)
        {
            var query = "SELECT * FROM Company WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                var company = await connection.QuerySingleOrDefaultAsync<Company>(query, new { id });
                return company;
            }
        }

        //Insert and returning Last inserted value
        public async Task<Company> CreateCompany(CompanyForCreationDto company)
        {
            var query = "INSERT INTO Company (name, address, country) VALUES (@name, @address, @country) RETURNING id"; // RETURNING id for returning last inserted id


            var parameters = new DynamicParameters();
            parameters.Add("Name", company.Name, DbType.String);
            parameters.Add("Address", company.Address, DbType.String);
            parameters.Add("Country", company.Country, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                var id = await connection.QuerySingleAsync<int>(query, parameters);
                var createdCompany = new Company
                {
                    Id = id,
                    Name = company.Name,
                    Address = company.Address,
                    Country = company.Country
                };
                return createdCompany;
            }
        }


        //Update

        public async Task UpdateCompany(int id, CompanyForUpdateDto company)
        {
            var query = "UPDATE Company SET Name = @Name, Address = @Address, Country = @Country WHERE Id = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Int32);
            parameters.Add("Name", company.Name, DbType.String);
            parameters.Add("Address", company.Address, DbType.String);
            parameters.Add("Country", company.Country, DbType.String);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        //Delete
        public async Task DeleteCompany(int id)
        {
            var query = "DELETE FROM Company WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { id });
            }
        }



        /////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Using Stored Procedure


        //getall using SP
        public async Task<IEnumerable<Company>> GetCompanypro()
        {
            var functionName = "company_name"; // function name

            using (var connection = _context.CreateConnection())
            {
                var companies = await connection.QueryAsync<Company>(functionName, commandType: CommandType.StoredProcedure);

                return companies.ToList();
            }

            //or

            //IEnumerable<Company> companies = null;
            //var getcompany = "company_name";
            //using (var connection = _context.CreateConnection())
            //{
            //    companies = await connection.QueryAsync<Company>(getcompany, commandType: CommandType.StoredProcedure);
            //}
            //return companies;


        }

        //get by id using SP
        public async Task<Company> GetCompanyprobyid(int id)
        {
            var functionName = "company_id1"; // function name 

            var parameters = new DynamicParameters();
            parameters.Add("p_id", id, DbType.Int32); // "p_id" is name in id for CREATE OR REPLACE FUNCTION company_id1(p_id integer)

            using (var connection = _context.CreateConnection())
            {
                var company = await connection.QuerySingleOrDefaultAsync<Company>(functionName, parameters, commandType: CommandType.StoredProcedure);
                return company;
            }
        }

        //Insert using SP
        public async Task CreateCompanypro(CompanyForCreationDto company)
        {
            var procedureName = "call sace_company(:p_name, :p_address,:p_country)";

            var parameters = new DynamicParameters();

            parameters.Add("p_name", company.Name, DbType.String);
            parameters.Add("p_address", company.Address, DbType.String);
            parameters.Add("p_country", company.Country, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                await connection.QueryAsync<Company>(procedureName, parameters);

            }
        }

        //Update using SP
        public async Task UpdateCompanypro(int id, CompanyForUpdateDto company) 
        {
            var procedureName = "call update_company(:p_id, :p_name, :p_address,:p_country)";

            var parameters = new DynamicParameters();
            parameters.Add("p_id", id, DbType.Int32);
            parameters.Add("p_name", company.Name, DbType.String);
            parameters.Add("p_address", company.Address, DbType.String);
            parameters.Add("p_country", company.Country, DbType.String);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(procedureName, parameters);
            }
        }

        //Delete using SP
        public async Task DeleteCompanypro(int id)
        {
            var procedureName = "call delete_company(:p_id)";


            var parameters = new DynamicParameters();
            parameters.Add("p_id", id, DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(procedureName, parameters);
            }
        }



    }
}
