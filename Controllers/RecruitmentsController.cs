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

      [HttpGet]
      public Task<IEnumerable<Student>> Get()
      {
         return studentRecruiter.GetClonesAsync();
      }

      [HttpPost(nameof(RecruitClones))]
      public Task RecruitClones([FromBody] int target)
      {
         return studentRecruiter.RecruitClonesAsync(target);
      }

      [HttpPost(nameof(RecruitClonesWithBackgroundJob))]
      public void RecruitClonesWithBackgroundJob([FromBody] int target)
      {
         backgroundJobClient.Enqueue(() => studentRecruiter.RecruitClonesInBackgroundAsync(target));
      }

      [HttpPost(nameof(RemoveClones))]
      public Task RemoveClones()
      {
         return studentRecruiter.RemoveClonesAsync();
      }

      [HttpPost(nameof(RemoveClonesWithBackgroundJob))]
      public void RemoveClonesWithBackgroundJob()
      {
         backgroundJobClient.Enqueue(() => studentRecruiter.RemoveClonesInBackgroundAsync());
      }

   }
}
