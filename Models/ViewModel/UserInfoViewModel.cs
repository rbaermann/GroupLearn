using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GroupLearn.Models
{
    public class UserInfoViewModel
    {
        public User CurrentUser {get; set;}

        public User UserInfo {get; set;}

        public School UsersSchool {get; set;}

        public List<Group> UsersGroups {get; set;}
    }
}