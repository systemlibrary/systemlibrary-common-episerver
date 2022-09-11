using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SystemLibrary.Common.Episerver.Tests.Models;

public class JsonEditCarInvalid
{
    [Required(ErrorMessage = "Must have larger text than 0")]
    [Display(Name = "Fornavn her", Description = "Fill in firstname")]
    public string Name { get; set; }

    [Required(AllowEmptyStrings = true)]
    [Display(Name = "Middlename", Description = "Middlename her...")]
    public string MiddleName { get; set; }

    public string lastName { get; set; }

    public List<int> PhoneNumbers { get; set; }

    public int[] PhoneNumberArray { get; set; }
}

public class JsonEditCar
{
    [Required(ErrorMessage = "Must have larger text than 0")]
    [Display(Name = "Firstname here", Description = "Fill in firstname")]
    public string Name { get; set; }

    [Required(AllowEmptyStrings = true)]
    [Display(Name = "Middle name", Description = "Middlename here...")]
    public string MiddleName { get; set; }

    public string lastName { get; set; }

    [Required]
    [Display(Name = "Age", Description = "From 0-100")]
    public int Age { get; set; }

    public DateTime Created { get; set; }

    [Display(Description = "Hello world")]
    public DateTimeOffset CreatedOffset { get; set; }

    public bool isSold { get; set; }

    [Required]
    public double Price { get; set; }

    public CarType CarType { get; set; }

    public JsonEditOwner Owner { get; set; }

    public int A1 { get; set; }
    public int A2;
}

public class JsonEditOwner
{
    public string Name { get; set; }
    public int CountryCode { get; set; }
    public int Id { get; set; }
    public DateTime Registered { get; set; }
    public bool InvoiceUnpaid { get; set; }
    public Available Status { get; set; }
}

public enum Available
{
    Enabled,
    Disabled
}

public enum CarType
{
    Car10,
    Car20,
    Car30,
    Car40
}

