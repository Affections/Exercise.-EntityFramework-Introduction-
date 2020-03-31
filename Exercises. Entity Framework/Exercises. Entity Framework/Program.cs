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
             string result = RemoveTown(context);
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

        public static string GetEmploye147(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var employee147 = new Employee();
            var employee = context.Employees.Select(o => new
            {
                employee147 = o.EmployeeId.Equals(147),
                FirstName = employee147.FirstName,
                LastName = employee147.LastName,
                JobTitle = employee147.JobTitle,
                Projects = o.EmployeesProjects
                    .Select(ep => ep.Project.Name)
                    .OrderBy(p => p)
                    .ToList()

            }).First();
           
                sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");
                sb.AppendLine(String.Join(Environment.NewLine, employee.Projects));

            
            return sb.ToString().TrimEnd();

        }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var departments = context.Departments.Where(o => o.Employees.Count > 5).OrderBy(o => o.Employees.Count)
                .ThenBy(o => o.Name).Select(o => new
                {
                    DepartmentName = o.Name,
                    ManagerFirstName = o.Manager.FirstName,
                    ManagerLastName = o.Manager.LastName,
                    AllEmployes = o.Employees.Select(x => new
                    {
                        EmployeeFirstName = x.FirstName,
                        EmployeeLastName = x.LastName,
                        EmployeeJobTitle = x.JobTitle
                    }).OrderBy(x => x.EmployeeFirstName).ThenBy(x => x.EmployeeLastName)
                });

            foreach (var d in departments)
            {
                sb.AppendLine($"{d.DepartmentName} - {d.ManagerFirstName} {d.ManagerLastName}");
                sb.AppendLine(String.Join(Environment.NewLine, d.AllEmployes));
            }
    

            return sb.ToString().TrimEnd();

        }

        public static string IncreaaseSalaries(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var employees = context.Employees.Where(o =>
                o.Department.Name == "Engineering" || o.Department.Name == "Tool Design" ||
                o.Department.Name == "Marketing" || o.Department.Name == "Information Services").Select(o => new
            {
                FirstName = o.FirstName,
                LastName = o.LastName,
                Salary = o.Salary * 1.12m
            }).OrderBy(o => o.FirstName).ThenBy(o => o.LastName);
            foreach (var d in employees)
            {
                sb.AppendLine($"{d.FirstName} {d.LastName} (${d.Salary:f2})");
           
            }


            return sb.ToString().TrimEnd();

        }

        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var employees = context.Employees.Where(o => o.FirstName.StartsWith("Sa")).Select(o => new
            {
                FirstName = o.FirstName,
                LastName = o.LastName,
                JobTitle = o.JobTitle,
                Salary = o.Salary
            }).OrderBy(o => o.FirstName).ThenBy(o => o.LastName);
            foreach (var d in employees)
            {
                sb.AppendLine($"{d.FirstName} {d.LastName} - {d.JobTitle} - (${d.Salary:f2})");

            }


            return sb.ToString().TrimEnd();
        }

        public static string DeleteProjectById(SoftUniContext context)

        {
            StringBuilder sb = new StringBuilder();
            var project = context.Projects.Find(2);
            context.Projects.Remove(project);
            context.EmployeesProjects.ToList().ForEach(ep => context.EmployeesProjects.Remove(ep));
            context.SaveChanges();
            var projects = context.Projects.Take(10).Select(o => new
            {
                ProjectName = o.Name
            });
            foreach (var d in projects)
            {
                sb.AppendLine($"{d.ProjectName}");

            }


            return sb.ToString().TrimEnd();
        }

        public static string RemoveTown(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            string townName = Console.ReadLine();

            context.Employees
                .Where(e => e.Address.Town.Name == townName)
                .ToList()
                .ForEach(e => e.AddressId = null);

            int addressesCount = context.Addresses
                .Where(a => a.Town.Name == townName)
                .Count();

            context.Addresses
                .Where(a => a.Town.Name == townName)
                .ToList()
                .ForEach(a => context.Addresses.Remove(a));

            context.Towns
                .Remove(context.Towns
                    .SingleOrDefault(t => t.Name == townName));

            context.SaveChanges();
         

            Console.WriteLine($"{addressesCount} {(addressesCount == 1 ? "address" : "addresses")} in {townName} {(addressesCount == 1 ? "was" : "were")} deleted");
            return sb.ToString().TrimEnd();
        }

    }
  

}
