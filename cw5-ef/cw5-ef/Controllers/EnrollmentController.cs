using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cw5_ef.DTOs.Requests;
using cw5_ef.DTOs.Responses;
using cw5_ef.Models;
using cw5_ef.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace cw5_ef.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        [HttpPost]
        [Route("enroll")]
        public IActionResult EnrollStudent([FromBody]EnrollStudentRequest request, [FromServices]IStudentsDbService dbService)
        {
            Student studentToEnroll = new Student
            {
                IndexNumber = request.IndexNumber,
                LastName = request.LastName,
                FirstName = request.FirstName,
                BirthDate = request.BirthDate
            };

            Enrollment tmp = dbService.EnrollStudent(studentToEnroll, request.Studies);
            if (tmp == null) return BadRequest();

            EnrollStudentResponse response = new EnrollStudentResponse
            {
                Semester = tmp.Semester,
                IdStudy = tmp.IdStudy,
                StartDate = tmp.StartDate,
                IdEnrollment = tmp.IdEnrollment
            };
            return Ok(response);
        }

        [HttpPost]
        [Route("promotions")]
        public IActionResult PromoteSemester([FromBody] PromoteStudentRequest request, [FromServices] IStudentsDbService dbService)
        {
            var newEnrollment = dbService.PromoteStudents(request.StudiesId, request.Semester);

            if (newEnrollment == null)
            {
                return BadRequest();
            }
            else
            {
                var response = new EnrollStudentResponse
                {
                    Semester = newEnrollment.Semester,
                    IdStudy = newEnrollment.IdStudy,
                    StartDate = newEnrollment.StartDate,
                    IdEnrollment = newEnrollment.IdEnrollment
                };
                return Ok(response);
            }
        }


    }
}

