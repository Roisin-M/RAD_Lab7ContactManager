﻿using System.ComponentModel.DataAnnotations;

namespace ContactManager.Models
{
    public class contact
    {
        //user ID from AspNetUser table
        public string? OwnerID { get; set; }
        public int ContactId { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zip { get; set; }
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        //Contact Status enum 
        public ContactStatus Status { get; set; }
    }
    public enum ContactStatus
    {
        Submitted,
        Approved,
        Rejected
    }
}
