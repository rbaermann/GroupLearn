using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GroupLearn.Models
{
    public class CreateGroupViewModel
    {
        public Group newGroup{get;set;}
        public UserGroup UserGroup{get;set;}
        public User currentUser{get;set;}
        public School thisSchool{get;set;}
    }
}