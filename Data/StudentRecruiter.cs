using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;

namespace StackifyExample4.Data
{
   public class StudentRecruiter
   {
      private readonly SchoolContext context;

      public StudentRecruiter(SchoolContext context)
      {
         this.context = context;
      }


      public async Task ClonesDepartmentsAsync(int? target = 1000)
      {
         for (int i = 0; i < target; i++)
         {
            var name = $"Clone-DEP-{Guid.NewGuid()}";
            var student = new Department
            {
               Name = name,
               StartDate = DateTime.Now,
               Budget = i * 100,
            };
            context.Departments.Add(student);
            await context.SaveChangesAsync();
         }
      }

      public async Task RecruitClonesAsync(int? target = 1000, bool isBackground = false)
      {
         for (int i = 0; i < target; i++)
         {
            var name = isBackground ? $"Clone-BACK-{Guid.NewGuid()}" : $"Clone-{Guid.NewGuid()}";
            var student = new Student
            {
               FirstMidName = name,
               LastName = name,
               EnrollmentDate = DateTime.Now
            };
            context.Students.Add(student);
            await context.SaveChangesAsync();
         }
      }

      public async Task RemoveStudentClonesAsync()
      {
         var clones = await GetStudentClonesAsync();
         foreach (var clone in clones)
         {
            context.Students.Remove(clone);
            await context.SaveChangesAsync();
         }

      }

      public async Task RemoveDepartmentClonesAsync()
      {
         var clones = await GetClonedDepartmentsAsync();
         foreach (var clone in clones)
         {
            context.Departments.Remove(clone);
            await context.SaveChangesAsync();
         }

      }

      public async Task<IEnumerable<Department>> GetClonedDepartmentsAsync()
      {
         var departments = await context.Departments.Where(d => d.Name.StartsWith("Clone-")).ToListAsync();
         return departments;

      }


      public async Task<IEnumerable<Student>> GetStudentClonesAsync()
      {
         var students = await context.Students.Where(s => s.LastName.StartsWith("Clone-")).ToListAsync();
         return students;
      }
   }
}
