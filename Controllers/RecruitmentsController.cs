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
      private readonly DepartmentRepository departmentRepository;
      private readonly StudentRepository studentRepository;
      private readonly IBackgroundJobClient backgroundJobClient;

      public RecruitmentsController(DepartmentRepository departmentRepository,
         StudentRepository studentRepository,
         IBackgroundJobClient backgroundJobClient)
      {
         this.departmentRepository = departmentRepository;
         this.studentRepository = studentRepository;
         this.backgroundJobClient = backgroundJobClient;
      }

      [HttpGet(nameof(GetStudentClones))]
      public Task<IEnumerable<Student>> GetStudentClones()
      {
         return studentRepository.GetStudentClonesAsync();
      }

      [HttpGet(nameof(GetClonedDepartments))]
      public Task<IEnumerable<Department>> GetClonedDepartments()
      {
         return departmentRepository.GetClonedDepartmentsAsync();
      }

      [HttpPost(nameof(RecruitStudentClones))]
      public Task RecruitStudentClones([FromBody] int target)
      {
         return studentRepository.RecruitClonesAsync(target);
      }

      [HttpPost(nameof(ClonesDepartmentsWithBackgroundJob))]
      public void ClonesDepartmentsWithBackgroundJob([FromBody] int target)
      {
         backgroundJobClient.Enqueue(() => departmentRepository.ClonesDepartmentsAsync(target));
      }

      [HttpPost(nameof(RemoveStudentClones))]
      public Task RemoveStudentClones()
      {
         return studentRepository.RemoveStudentClonesAsync();
      }

      [HttpPost(nameof(RemoveDepartmentClonesWithBackgroundJob))]
      public void RemoveDepartmentClonesWithBackgroundJob()
      {
         backgroundJobClient.Enqueue(() => departmentRepository.RemoveDepartmentClonesAsync());
      }

   }
}
