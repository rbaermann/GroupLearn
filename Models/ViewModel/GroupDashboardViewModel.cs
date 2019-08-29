using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace GroupLearn.Models
{
    public class GroupDashboardViewModel
    {
        public User thisUser{get;set;}
        public List<Group> currentGroups{get;set;}
        public UserGroup UserGroup{get;set;}
        public List<User> Leaders{get;set;}
    }
}