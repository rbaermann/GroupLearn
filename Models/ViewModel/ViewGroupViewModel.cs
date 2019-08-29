using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GroupLearn.Models
{
    public class ViewGroupViewModel
    {
        public Group thisGroup{get;set;}
        public UserGroup UserGroup{get;set;}
        public List<UserGroup> UserGroups{get;set;}
        public User thisUser{get;set;}
        List<Group> currentGroups{get;set;}
    }
}