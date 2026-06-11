using Microsoft.AspNetCore.Identity;

namespace HRAPI.Models;

// Application login user; optionally links Identity authentication to an employee profile.
public class AppUser : IdentityUser
{
    public int? EmployeeId { get; set; }          // FK → Employee
    public Employee? Employee { get; set; }       // navigation
}
