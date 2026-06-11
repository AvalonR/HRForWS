using Microsoft.AspNetCore.Identity;

namespace HRAPI.Models;

// Application login user. It can optionally link to an HR employee profile.
public class AppUser : IdentityUser
{
    public int? EmployeeId { get; set; }          // FK → Employee
    public Employee? Employee { get; set; }       // navigation
}
