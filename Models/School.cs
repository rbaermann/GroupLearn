using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace GroupLearn.Models
{
    public class School
    {
        [Key]
        public int SchoolId{get;set;}
        [Required]
        [MinLength(2)]
        public string Name{get;set;}
        [Required]
        [MinLength(2)]
        public string Location{get;set;}
        public List<User> Students{get;set;}
        public List<Group> Groups{get;set;}
        public DateTime CreatedAt{get;set;}=DateTime.Now;
        public DateTime UpdatedAt{get;set;}=DateTime.Now;
    }
}