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
   public class StudentRepository
   {
      private readonly DapperContext context;

      public StudentRepository(DapperContext context)
      {
         this.context = context;
      }

      internal async Task<IEnumerable<Student>> GetStudentClonesAsync()
      {
         var query = "SELECT * FROM Person WHERE FirstName LIKE @Name";
         using (var connection = context.CreateConnection())
         {
            var students = await connection.QueryAsync<Student>(query, new { Name = "Clone-%" });
            return students.ToList();
         }
      }

      public async Task RecruitClonesAsync(int? target = 1000)
      {
         for (int i = 0; i < target; i++)
         {
            var name = $"Clone-{Guid.NewGuid()}";
            var student = new Student
            {
               FirstMidName = name,
               LastName = name,
               EnrollmentDate = DateTime.Now
            };
            var query = "INSERT INTO Person (FirstName, LastName, EnrollmentDate) VALUES (@FirstName, @LastName, @EnrollmentDate)";
            var parameters = new DynamicParameters();
            parameters.Add("FirstName", student.FirstMidName, DbType.String);
            parameters.Add("LastName", student.LastName, DbType.String);
            parameters.Add("EnrollmentDate", student.EnrollmentDate, DbType.DateTime);
            using (var connection = context.CreateConnection())
            {
               await connection.ExecuteAsync(query, parameters);
            }
         }
      }

      public async Task RemoveStudentClonesAsync()
      {
         var query = "DELETE FROM Person WHERE LastName LIKE @Name";
         var parameters = new DynamicParameters();
         parameters.Add("Name", "Clone-%", DbType.String);
         using (var connection = context.CreateConnection())
         {
            await connection.ExecuteAsync(query, parameters);
         }
      }
   }
}
