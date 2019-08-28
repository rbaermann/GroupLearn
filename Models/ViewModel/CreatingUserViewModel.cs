using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GroupLearn.Models
{
    public class CreatingUserViewModel
    {
        public List<School> ListOfSchools {get; set;}

        public School NewSchool {get; set;}

        public User newUser {get; set;}
    }
}