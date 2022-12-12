using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class Company
    {
        public int Id { get; set; }
        //public string CompanyName { get; set; } // actual column name is " Name": to check for coloumn name changing

        public string Name { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}
