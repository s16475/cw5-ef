using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cw5_ef.DTOs.Responses
{
    public class EnrollStudentResponse
    {
        
         public int IdEnrollment { get; set; }
         public int Semester { get; set; }
         public int IdStudy { get; set; }
         public DateTime StartDate { get; set; }

        
    }
}
