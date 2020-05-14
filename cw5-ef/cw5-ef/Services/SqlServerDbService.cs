﻿using cw5_ef.DTOs.Requests;
using cw5_ef.DTOs.Responses;
using cw5_ef.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace cw5_ef.Services
{
    public class SqlServerDbService : IStudentsDbService
    {

        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            EnrollStudentResponse esr = new EnrollStudentResponse() { };

            using (var con = new SqlConnection(ConString))
            using (var com = new SqlCommand())
            {
                con.Open();
                var tran = con.BeginTransaction();
                com.Connection = con;
                com.Transaction = tran;
                try
                {

                    //1. Czy studia istnieją?

                    com.CommandText = "SELECT IdStudy AS idStudies FROM Studies WHERE Name=@name";
                    com.Parameters.AddWithValue("name", request.Studies);
                    var dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        dr.Close();
                        tran.Rollback();
                        return new NotFoundResult();
                    }

                    int idStudies = (int)dr["idStudies"];
                    dr.Close();

                    //2. Sprawdzenie czy nie występuje konflikt indeksów  

                    com.CommandText = "SELECT IndexNumber FROM Student WHERE IndexNumber = '" + request.IndexNumber + "'";
                    dr = com.ExecuteReader();
                    if (dr.Read())
                    {
                        dr.Close();
                        tran.Rollback();
                        return new BadRequestResult();
                    }
                    dr.Close();

                    // 3.Nadanie i wstawienie IdEnrollment

                    int idEnrollment;
                    com.CommandText = "SELECT IdEnrollment FROM Enrollment WHERE IdStudy = " + idStudies + " AND Semester = 1";

                    dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        idEnrollment = 1;
                        dr.Close();
                        com.CommandText = "INSERT INTO Enrollment(IdEnrollment,Semester,IdStudy,StartDate)" +
                        "  VALUES(" + idEnrollment + ", 1, " + idStudies + ",GetDate())";
                        com.ExecuteNonQuery();
                    }
                    idEnrollment = (int)dr["idEnrollment"];
                    dr.Close();

                    //4. Wstawienie studenta

                    string strDateFormat = "dd.MM.yyyy";
                    DateTime BirthDate = DateTime.ParseExact(request.BirthDate.ToString(), strDateFormat, CultureInfo.InvariantCulture);

                    com.CommandText = $"INSERT INTO Student VALUES (@IndexNumber, @FirstName, @LastName, @BirthDate, @IdEnrollment)";
                    com.Parameters.AddWithValue("IndexNumber", request.IndexNumber);
                    com.Parameters.AddWithValue("FirstName", request.FirstName);
                    com.Parameters.AddWithValue("LastName", request.LastName);
                    com.Parameters.AddWithValue("BirthDate", BirthDate);
                    com.Parameters.AddWithValue("IdEnrollment", idEnrollment);
                    com.ExecuteNonQuery();

                    esr.IdEnrollment = idEnrollment;
                    esr.IdStudy = idStudies;
                    esr.Semester = 1;
                    esr.StartDate = DateTime.Now;
                    tran.Commit();
                    tran.Dispose();

                    ObjectResult objectResult = new ObjectResult(esr);
                    objectResult.StatusCode = 201;
                    return objectResult;
                }
                catch (SqlException exc)
                {
                    tran.Rollback();
                    return new BadRequestResult();
                }
            }
        }

        //public IActionResult EnrollStudent(EnrollStudentRequest request)
        //{
        //    throw new NotImplementedException();
        //}

        public IActionResult PromoteStudent(PromoteStudentRequest promote)
        {
            Enrollment enrollment = new Enrollment();
            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();


                com.Parameters.AddWithValue("Studies", promote.Studies);
                com.Parameters.AddWithValue("Semester", promote.Semester);
                var wynik = UseProcedure("PromoteStudents", com);

                if (wynik[0][0].Equals("404"))
                {
                    return new NotFoundResult();
                }
                enrollment.IdEnrollment = wynik[0][0];
                enrollment.IdStudy = wynik[0][2];
                enrollment.Semester = wynik[0][1];
                enrollment.StartDate = wynik[0][3];

            }

            ObjectResult objectResult = new ObjectResult(enrollment);
            objectResult.StatusCode = 201;
            return objectResult;

        }

        //public IActionResult PromoteStudent(PromoteStudentRequest promote)
        //{
        //    throw new NotImplementedException();
        //}

        public List<string[]> UseProcedure(string name, SqlCommand com)
        {
            List<string[]> wynik = new List<string[]>();

            com.CommandType = CommandType.StoredProcedure;
            com.CommandText = name;
            if (com.Connection.State != ConnectionState.Open) com.Connection.Open();

            var dr = com.ExecuteReader();

            while (dr.Read())
            {
                string[] tmp = new string[dr.FieldCount];
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    tmp[i] = dr.GetValue(i).ToString();
                }
                wynik.Add(tmp);
            }
            dr.Close();
            com.CommandType = CommandType.Text;
            com.Parameters.Clear();

            return wynik;
        }
    }
}