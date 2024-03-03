﻿using System.ComponentModel.DataAnnotations;

namespace Try_pls.Models
{
    public interface IEntity
    {
        string Id { get; set; }
        List<Address>? Addresses { get; set; }
        List<Date> Dates { get; set; }
        bool Deceased { get; set; }
        string Gender { get; set; }
        List<Name> Names { get; set; }
    }

    public class Contacts : IEntity
    {
        public string Id { get; set; }
        public List<Address>? Addresses { get; set; }
        public List<Date> Dates { get; set; }
        public bool Deceased { get; set; }
        // [Required(AllowEmptyStrings = true)]
        public string Gender { get; set; }
        public List<Name> Names { get; set; }
    }

    public class Address
    {
        public string? AddressLine { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }

    public class Date
    {
        public string? DateType { get; set; }
        public DateTime? DateValue { get; set; }
    }

    public class Name
    {
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? Surname { get; set; }
    }
}

