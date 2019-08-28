using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupLearn.Models
{
    public class UserGroup
    {
        [Key]
        public int UserGroupId{get;set;}
        public int UserId{get;set;}
        public int GroupId{get;set;}
        public User User{get;set;}
        public Group Group{get;set;}
    }
}