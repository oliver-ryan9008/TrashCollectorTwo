using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TrashCollector2.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        public string EmailAddress { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public int ZipCode { get; set; }
        public string UserId { get; set; }
    }
}