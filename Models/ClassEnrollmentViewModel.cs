using SchoolManagementApp.MVC.Data;
using SchoolManagementApp.MVC.Models;

namespace SchoolManagementApp.MVC.Models;

public class ClassEnrollmentViewModel
{
    // Replace this Data Model dependency with a custom ViewModel that will be populated by the Controller;
    //  a View Model shouldn't have a Data Model dependency like this:
    // public Class? Class { get; set; }
    public ClassViewModel? Class { get; set; }

    public List<StudentEnrollmentViewModel> Students {get; set;} = new List<StudentEnrollmentViewModel>();
}
