using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace cw5_ef.DTOs.Requests
{
    public class PromoteStudentRequest
    {
        [Required]
        public int StudiesId { get; set; }
        [Required]
        public int Semester { get; set; }
    }
}
