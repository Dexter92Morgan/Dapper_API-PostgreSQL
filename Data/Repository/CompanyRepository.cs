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
    public class CompanyRepository : ICompanyRepository
    {
        private readonly DapperContext _context;
        public CompanyRepository(DapperContext context)
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


        //public async Task CreateCompany(CompanyForCreationDto company)
        //{
        //    var query = "INSERT INTO Companies (Name, Address, Country) VALUES (@Name, @Address, @Country)";

        //    var parameters = new DynamicParameters();
        //    parameters.Add("Name", company.Name, DbType.String);
        //    parameters.Add("Address", company.Address, DbType.String);
        //    parameters.Add("Country", company.Country, DbType.String);

        //    using (var connection = _context.CreateConnection())
        //    {
        //        await connection.ExecuteAsync(query, parameters);
        //    }
        //}

        //Insert and returning last inserted value
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


        //Executing Multiple SQL Statements with a Single Query

        public async Task<Company> GetCompanyEmployeesMultipleResults(int id)
        {
            var query = "SELECT * FROM Company WHERE Id = @Id;" +
                        "SELECT * FROM Employee WHERE CompanyId = @Id";
            using (var connection = _context.CreateConnection())
            using (var multi = await connection.QueryMultipleAsync(query, new { id }))
            {
                var company = await multi.ReadSingleOrDefaultAsync<Company>();
                if (company != null)
                    company.Employees = (await multi.ReadAsync<Employee>()).ToList();
                return company;
            }
        }

        //Multiple Mapping
        public async Task<List<Company>> GetCompaniesEmployeesMultipleMapping()
        {
            var query = "SELECT * FROM Company c JOIN Employee e ON c.Id = e.CompanyId";
            using (var connection = _context.CreateConnection())
            {
                var companyDict = new Dictionary<int, Company>();
                var companies = await connection.QueryAsync<Company, Employee, Company>(
                    query, (company, employee) =>
                    {
                        if (!companyDict.TryGetValue(company.Id, out var currentCompany))
                        {
                            currentCompany = company;
                            companyDict.Add(currentCompany.Id, currentCompany);
                        }
                        currentCompany.Employees.Add(employee);
                        return currentCompany;
                    }
                );
                return companies.Distinct().ToList();
            }

        }

        //Transactions
        public async Task CreateMultipleCompanies(List<CompanyForCreationDto> companies)
        {
            var query = "INSERT INTO Company (Name, Address, Country) VALUES (@Name, @Address, @Country)";
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    foreach (var company in companies)
                    {
                        var parameters = new DynamicParameters();

                        parameters.Add("Name", company.Name, DbType.String);
                        parameters.Add("Address", company.Address, DbType.String);
                        parameters.Add("Country", company.Country, DbType.String);
                        await connection.ExecuteAsync(query, parameters, transaction: transaction);
                    }
                    transaction.Commit();
                }
            }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

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

            
        //Update using sp
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

//, commandType: CommandType.StoredProcedure

// query for creating table in postgrey with primary key working 

//   CREATE TABLE company(
//   id SERIAL PRIMARY KEY,
//    name character varying(150),
//	address character varying(150),
//	country character varying(150)
//);

// for function sp

//        CREATE OR REPLACE FUNCTION ShowCompanyForProvidedEmployeeId()
//RETURNS TABLE(id integer, name varchar, address varchar, country varchar) as
//$BODY$   
//BEGIN

//    return query
//    select a.id, a.name, a.address, a.country from company a join employee c on a.id = c.companyid where c.id = id;
//END;
//$BODY$
// LANGUAGE plpgsql;


//CREATE FUNCTION my_proc()
//RETURNS TABLE(id int, name character varying, address character varying, country character varying )
//AS $BODY$
//BEGIN
//  SELECT a.id, a.name,a.address,a.country
//  FROM company a
//  INNER JOIN employee b ON b.id = a.id
//  WHERE a.id = '2';
//END;
//$BODY$
//LANGUAGE plpgsql;


////ForeignKey creation
///
//DROP TABLE IF EXISTS company;
//DROP TABLE IF EXISTS employee;

//CREATE TABLE company(
//   id INT GENERATED ALWAYS AS IDENTITY,
//   name VARCHAR(255) NOT NULL,
//    address VARCHAR(255) NOT NULL,
//    country VARCHAR(255) NOT NULL,
//   PRIMARY KEY(id)
//);

//CREATE TABLE employee(
//   id INT GENERATED ALWAYS AS IDENTITY,
//   name VARCHAR(255) NOT NULL,
//    age INT,
//   position VARCHAR(150),
//   companyid INT,
//   PRIMARY KEY(id),
//   CONSTRAINT fk_company
//      FOREIGN KEY(companyid) 
//	  REFERENCES company(id)
//);


//begin
//  return query
//    select c.id, c.name, c.address, c.country
//    from company c join employee e on c.id = e.companyid
//    where e.id = '1';
//end;


//get all function

//CREATE OR REPLACE FUNCTION company_name()
//RETURNS TABLE(id integer, name character varying(255),
//			   address character varying(255), country character varying(255))

//AS $e_name$
//BEGIN
//   RETURN QUERY SELECT *  FROM company;

//END;
//$e_name$ 
//LANGUAGE plpgsql;

//get by id function

//CREATE OR REPLACE FUNCTION company_id1(p_id integer)
//RETURNS TABLE(id integer, name character varying(255),
//			   address character varying(255), country character varying(255))
			   
//AS $e_name$
//BEGIN
//   RETURN QUERY SELECT a.id, a.name, a.address, a.country  FROM company a where a.id = p_id ;

//END;
//$e_name$ 
//LANGUAGE plpgsql;


//Insert  procedure

//create or replace procedure sace_company(p_name character varying, p_address character varying, p_country character varying)
//as
//$$
//	begin
//        insert into company (name, address, country) values(p_name, p_address, p_country);
//end;
//$$
//language plpgsql;

//Update  procedure

//create or replace procedure update_company(p_id integer, p_name character varying, p_address character varying, p_country character varying)
//as
//$$
//	begin
//        update company set name = p_name, address = p_address, country = p_country where id = p_id;
//end;
//$$
//language plpgsql;

//Delete  procedure

//create or replace procedure delete_company(p_id integer)
//as
//$$
//	begin
//        delete from company where id = p_id;
//end;
//$$
//language plpgsql;