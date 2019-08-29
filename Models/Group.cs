using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace GroupLearn.Models
{
    public class Group
    {
        [Key]
        public int GroupId{get;set;}
        [Required]
        [MinLength(2,ErrorMessage="Name must be at least 2 characters")]
        public string Name{get;set;}

        [Required]
        public string Subject{get;set;}

        [Required]
        [ValidDate]
        public DateTime Date{get;set;}

        public int Size{get;set;}

        [Required]
        public string Location{get;set;}
        [Required]
        public DateTime Time{get;set;}

        [Dur]
        public int Duration{get;set;}
        public string HourMinute{get;set;}

        public List<UserGroup> UserGroups{get;set;}
        public School School{get;set;}
        public int SchoolId{get;set;}

        public User Leader{get;set;}

        public DateTime CreatedAt{get;set;}=DateTime.Now;
        public DateTime UpdatedAt{get;set;}=DateTime.Now;

        public class ValidDateAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if ((DateTime)value < DateTime.Today)
                {
                    return new ValidationResult("Please Enter a Future Date.");
                }
                return ValidationResult.Success;
            }
        }

        public class DurAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if((int)value < 0)
                {
                    return new ValidationResult("Please Enter a Valid Duration.");
                }
                return ValidationResult.Success;
            }
        }
    }
}