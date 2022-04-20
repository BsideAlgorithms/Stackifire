using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Dapper;

namespace StackifyExample4.Data
{
   public class DepartmentRepository
   {
      private readonly DapperContext context;

      public DepartmentRepository(DapperContext context)
      {
         this.context = context;
      }

      public async Task<IEnumerable<Department>> GetClonedDepartmentsAsync()
      {
         var query = "SELECT * FROM Department WHERE Name LIKE @Name";
         using (var connection = context.CreateConnection())
         {
            var deparments = await connection.QueryAsync<Department>(query, new { Name = "Clone-%" });
            return deparments.ToList();
         }
      }

      public async Task ClonesDepartmentsAsync(int? target = 1000)
      {
         for (int i = 0; i < target; i++)
         {
            var name = $"Clone-{Guid.NewGuid()}";
            var department = new Department
            {
               Name = name,
               Budget = i * 100,
               StartDate = DateTime.Now,
            };
            var query = "INSERT INTO Department (Name, Budget, StartDate) VALUES (@Name, @Budget, @StartDate)";
            var parameters = new DynamicParameters();
            parameters.Add("Name", department.Name, DbType.String);
            parameters.Add("Budget", department.Budget, DbType.Int32);
            parameters.Add("StartDate", department.StartDate, DbType.DateTime);
            using (var connection = context.CreateConnection())
            {
               await connection.ExecuteAsync(query, parameters);
            }
         }
      }

      public async Task RemoveDepartmentClonesAsync()
      {
         var query = "DELETE FROM Department WHERE Name LIKE @Name";
         var parameters = new DynamicParameters();
         parameters.Add("Name", "Clone-%", DbType.String);
         using (var connection = context.CreateConnection())
         {
            await connection.ExecuteAsync(query, parameters);
         }
      }
   }
}
