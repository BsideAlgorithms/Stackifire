using System.Collections.Generic;
using System.Threading.Tasks;
using ContosoUniversity.Models;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using StackifyExample4.Data;

namespace StackifyExample4.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class RecruitmentsController : ControllerBase
   {
      private readonly StudentRecruiter studentRecruiter;
      private readonly IBackgroundJobClient backgroundJobClient;

      public RecruitmentsController(StudentRecruiter studentRecruiter, IBackgroundJobClient backgroundJobClient)
      {
         this.studentRecruiter = studentRecruiter;
         this.backgroundJobClient = backgroundJobClient;
      }

      [HttpGet(nameof(GetStudentClones))]
      public Task<IEnumerable<Student>> GetStudentClones()
      {
         return studentRecruiter.GetStudentClonesAsync();
      }

      [HttpGet(nameof(GetClonedDepartments))]
      public Task<IEnumerable<Department>> GetClonedDepartments()
      {
         return studentRecruiter.GetClonedDepartmentsAsync();
      }

      [HttpPost(nameof(RecruitStudentClones))]
      public Task RecruitStudentClones([FromBody] int target)
      {
         return studentRecruiter.RecruitClonesAsync(target);
      }

      [HttpPost(nameof(ClonesDepartmentsWithBackgroundJob))]
      public void ClonesDepartmentsWithBackgroundJob([FromBody] int target)
      {
         backgroundJobClient.Enqueue(() => studentRecruiter.ClonesDepartmentsAsync(target));
      }

      [HttpPost(nameof(RemoveStudentClones))]
      public Task RemoveStudentClones()
      {
         return studentRecruiter.RemoveStudentClonesAsync();
      }

      [HttpPost(nameof(RemoveDepartmentClonesWithBackgroundJob))]
      public void RemoveDepartmentClonesWithBackgroundJob()
      {
         backgroundJobClient.Enqueue(() => studentRecruiter.RemoveDepartmentClonesAsync());
      }

   }
}
