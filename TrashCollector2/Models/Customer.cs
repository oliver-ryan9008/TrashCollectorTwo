using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TrashCollector2.Models
{
    public class Customer
    {
        [Key]
        public string UserId { get; set; }
        public string EmailAddress { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string StreetAddress { get; set; }
        public int ZipCode { get; set; }
        public string WeeklyPickupDay { get; set; }
        [DataType(DataType.Date)]
        public DateTime? OneTimePickupDate { get; set; }
        public int? MoneyOwed { get; set; }
        public DateTime? StartOfDelayedPickup { get; set; }
        public DateTime? EndOfDelayedPickup { get; set; }
        public bool IsOnHold { get; set; }
        public bool? IsConfirmed { get; set; }
    }
}