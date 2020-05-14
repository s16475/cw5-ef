using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace cw5_ef.DTOs.Requests
{
    public class EnrollStudentRequest
    {
        [Required(ErrorMessage = "Musisz podać index")]
        [RegularExpression("^s[0-9]+$")]
        public string IndexNumber { get; set; }
        
        [Required(ErrorMessage = "Musisz podać imię")]
        public string LastName { get; set; }
        
        [Required(ErrorMessage = "Musisz podać nazwisko")]
        public string FirstName { get; set; }
        
        [Required(ErrorMessage = "Musisz podać date urodzenia")]
        public DateTime BirthDate { get; set; }
        
        [Required(ErrorMessage = "Musisz podać kierunek studiów")]
        public string Studies { get; set; }
    }
}
