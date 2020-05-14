using cw5_ef.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cw5_ef.Services
{
    interface IStudentsDbService
    {
        IActionResult EnrollStudent(EnrollStudentRequest request);
        IActionResult PromoteStudent(PromoteStudentRequest promote);
    }
}
