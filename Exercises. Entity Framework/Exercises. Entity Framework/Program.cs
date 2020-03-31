using Exercises._Entity_Framework.Data;
using Exercises._Entity_Framework.Models;
using System;
using System.Linq;
using System.Text;

namespace Exercises._Entity_Framework
{
    public class Program
    {
        public static void Main(string[] args)
        {
           
             SoftUniContext context = new SoftUniContext();
             string result = GetAddressesByTown(context);
             Console.WriteLine(result);

        }
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees.OrderBy(e => e.EmployeeId).Select(e => new
            {
                Id = e.EmployeeId,
                Name = String.Join(" ", e.FirstName, e.LastName, e.MiddleName),
                e.JobTitle,
                e.Salary
            });
            foreach (var e in employees)
            {
                sb.AppendLine($"{e.Name} {e.JobTitle} {e.Salary:f2}");
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesSalarayOver50000(SoftUniContext context)
        {
            StringBuilder sb= new StringBuilder();
            
            var employees = context.Employees.Where(z => z.Salary > 50000).OrderBy(e => e.FirstName).Select(e => new
            {
                e.FirstName,
                e.Salary
            });
            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} - {e.Salary:f2}");
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var employees = context.Employees.Where(e => e.Department.Name == "Research and Development").
                OrderBy(e => e.Salary).
                ThenByDescending(e => e.FirstName).
                Select(e => new
            {
                e.FirstName,
                e.LastName,
                DepartmentName = e.Department.Name,
                e.Salary
              
            });
            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} from {e.DepartmentName} - {e.Salary:f2}");
            }
            return sb.ToString().TrimEnd();
        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var newAddress = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };
            context.Addresses.Add(newAddress);
;            Employee name = context.Employees.First(e => e.LastName == "Nakov");
            name.Address = newAddress;
            context.SaveChanges();
            var employees = context.Employees.OrderByDescending(e => e.AddressId).Select(e => new
            {
                AddressText = e.Address.AddressText,
              
            }).Take(10).ToList();
           
                sb.AppendLine(String.Join(Environment.NewLine, employees));

                return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var employees = context.Employees
                .Where(e => e.EmployeesProjects.Any(ep =>
                    ep.Project.StartDate.Year >= 2001 && ep.Project.StartDate.Year <= 2003)).Take(10).Select(e => new
                {
                        e.FirstName, 
                        e.LastName,
                        ManagerFirstName=e.Manager.FirstName,
                        ManagerLastName = e.Manager.LastName,
                        Projects = e.EmployeesProjects.Select(ep => ep.Project)


                    }).ToList();
            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.ManagerFirstName} {e.ManagerLastName}");
                //sb.AppendLine(e.Projects.Select(p =>
                //    $"--{p.Name} - {p.StartDate.ToString()} - {p.EndDate == null ? "not finished" : p.EndDate.ToString()}"));
            }
            return sb.ToString().TrimEnd();

        }

        public static string GetAddressesByTown(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var addresses = context.Addresses.GroupBy(a => new {
                        a.AddressId,
                        a.AddressText,
                        a.Town.Name
                    },
                    (key, group) => new {
                        AddressText = key.AddressText,
                        Town = key.Name,
                        Count = group.Sum(a => a.Employees.Count)
                    }).OrderByDescending(x => x.Town).OrderBy(x => x.Town)
                .ThenBy(x => x.AddressText).Take(10).ToList();
            foreach (var a in addresses)
            {
                sb.AppendLine($"{a.AddressText}, {a.Town} - {a.Count} employees");
                //sb.AppendLine(e.Projects.Select(p =>
                //    $"--{p.Name} - {p.StartDate.ToString()} - {p.EndDate == null ? "not finished" : p.EndDate.ToString()}"));
            }
            return sb.ToString().TrimEnd();
        }

    }
  

}
