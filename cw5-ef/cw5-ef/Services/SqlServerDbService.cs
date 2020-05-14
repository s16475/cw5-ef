using cw5_ef.DTOs.Requests;
using cw5_ef.DTOs.Responses;
using cw5_ef.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace cw5_ef.Services
{
    public class SqlServerDbService : IStudentsDbService
    {

        private s16475Context _dbContext;

        public SqlServerDbService([FromServices] DbContext dbContext)
        {
            this._dbContext = (s16475Context)dbContext;
        }

        public Enrollment EnrollStudent(Student studentToEnroll, string studiesName)
        {
            try
            {
                var studiesExists = _dbContext.Studies.Any(s => s.Name.Equals(studiesName));
                if (!studiesExists)
                {
                    return null;
                }

                var studentAlreadyExists =
                    _dbContext.Student.Any(s => s.IndexNumber.Equals(studentToEnroll.IndexNumber));

                if (studentAlreadyExists)
                {
                    return null;
                }

                Student toAddStudent = new Student
                {
                    IndexNumber = studentToEnroll.IndexNumber,
                    LastName = studentToEnroll.LastName,
                    BirthDate = studentToEnroll.BirthDate,
                    FirstName = studentToEnroll.FirstName,
                    IdEnrollment = 1
                };

                _dbContext.Student.Add(toAddStudent);
                _dbContext.SaveChanges();


                int idStudy = _dbContext.Studies.Single(s => s.Name.Equals(studiesName)).IdStudy;
                int idEnrollment = _dbContext.Enrollment.Where(e => e.Semester == 1 && e.IdStudy == idStudy)
                    .OrderByDescending(e => e.StartDate).First().IdEnrollment;

                if (idEnrollment == 0)
                {
                    idEnrollment = _dbContext.Enrollment.Max(e => e.IdEnrollment) + 1;
                    Enrollment newEnrollment = new Enrollment
                    {
                        IdEnrollment = idEnrollment,
                        Semester = 1,
                        IdStudy = idStudy,
                        StartDate = DateTime.Now
                    };
                    _dbContext.Enrollment.Add(newEnrollment);
                    _dbContext.SaveChanges();
                }

                toAddStudent.IdEnrollment = idEnrollment;
                _dbContext.SaveChanges();

                var resp = _dbContext.Enrollment.Single(e => e.IdEnrollment == idEnrollment);
                return resp;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public Enrollment PromoteStudents(int studiesId, int semester)
        {
            try
            {
                var studiesGot = _dbContext.Studies.Single(s => s.IdStudy == studiesId);

                var oldEnrollment =
                    _dbContext.Enrollment.Single(e => e.IdStudy == studiesGot.IdStudy && e.Semester == semester);

                var newEnrollment =
                    _dbContext.Enrollment.SingleOrDefault(
                        e => e.IdStudy == studiesGot.IdStudy && e.Semester == semester + 1);

                if (newEnrollment == null)
                {
                    var newId = _dbContext.Enrollment.Max(e => e.IdEnrollment) + 1;
                    newEnrollment = new Enrollment
                    {
                        IdEnrollment = newId,
                        Semester = semester + 1,
                        IdStudy = studiesGot.IdStudy,
                        StartDate = DateTime.Now
                    };
                    _dbContext.Enrollment.Add(newEnrollment);
                    _dbContext.SaveChanges();
                }

                var studentsToUpdate = _dbContext.Student.Where(s => s.IdEnrollment == oldEnrollment.IdEnrollment)
                    .ToList();

                foreach (Student student in studentsToUpdate)
                {
                    student.IdEnrollment = newEnrollment.IdEnrollment;
                }

                _dbContext.SaveChanges();

                return newEnrollment;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}
