using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace SchoolManagementApp.MVC.Data;

public class CourseMetaData
{
    [StringLength(50)]
    [Display(Name = "Name")]
    public string CourseName { get; set; } = null!;
}

public class LecturerMetaData
{
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = null!;

    [Display(Name = "Last Name")]
    public string LastName { get; set; } = null!;
}

public class StudentMetaData
{
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = null!;

    [Display(Name = "Last Name")]
    public string LastName { get; set; } = null!;

    [Display(Name = "Date Of Birth")]
    public DateTime? DateOfBirth { get; set; }
}

// Connect Metadata to each Data Model class
[ModelMetadataType(typeof(CourseMetaData))]
public partial class Course { }

[ModelMetadataType(typeof(LecturerMetaData))]
public partial class Lecturer { }

[ModelMetadataType(typeof(StudentMetaData))]
public partial class Student { }
