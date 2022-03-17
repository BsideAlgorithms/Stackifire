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


      public Task RecruitClonesInBackgroundAsync(int? target = 1000)
      {
         return RecruitClonesAsync(target, isBackground: true);
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

      public Task RemoveClonesInBackgroundAsync() => RemoveClonesAsync();

      public async Task RemoveClonesAsync()
      {
         var clones = await GetClonesAsync();
         foreach (var clone in clones)
         {
            context.Students.Remove(clone);
            await context.SaveChangesAsync();
         }

      }

      public async Task<IEnumerable<Student>> GetClonesAsync()
      {
         var students = await context.Students.Where(s => s.LastName.StartsWith("Clone-")).ToListAsync();
         return students;
      }
   }
}
