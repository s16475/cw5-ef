using cw5_ef.DTOs.Requests;
using cw5_ef.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cw5_ef.Services
{
    public interface IStudentsDbService
    {
        public Enrollment EnrollStudent(Student studentToEnroll, string studiesName);

        public Enrollment PromoteStudents(int studiesId, int semester);
    }
}
