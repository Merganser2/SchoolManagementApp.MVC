using SchoolManagementApp.MVC.Data;
using SchoolManagementApp.MVC.Models;

namespace SchoolManagementApp.MVC.Models;

public class ClassEnrollmentViewModel
{
    // TEMP: Allowing this ViewModel to have a dependency on Data Model
    public Class? Class { get; set; }

    public List<StudentEnrollmentViewModel> Students {get; set;} = new List<StudentEnrollmentViewModel>();
}
