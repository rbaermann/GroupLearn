using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;


namespace GroupLearn.Models
{
    public class UserRates
    {
        public int UserRatesId {get; set;}

        public int UserId {get; set;}

        public int CurUser {get; set;}

        public int Rating {get; set;}

        public User User {get; set;}
    }
}