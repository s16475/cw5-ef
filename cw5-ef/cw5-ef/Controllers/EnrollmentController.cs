using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace cw5_ef.Controllers
{
    [Route("api/enrollment")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly IStudentsDbService _service;

        public EnrollmentController(IStudentsDbService _service)
        {
            this._service = _service;
        }

        //zadanie 4.1 (5.1) - dodajemy nowych studentow
        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            var result = _service.EnrollStudent(request);
            if (result != null) return Ok(result);
            return NotFound();
        }

        //zadanie 4.2 (5.2) - promocja na nowy semestr
        [HttpPost("promotions")]
        public IActionResult PromoteStudent(PromoteStudentRequest promote)
        {
            var result = _service.PromoteStudent(promote);
            if (result != null) return Ok(result);
            return NotFound();
        }




    }
}

