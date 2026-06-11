using Microsoft.AspNetCore.Identity;

namespace HRAPI.Models;

public class AppUser : IdentityUser
{
    public int? EmployeeId { get; set; }          // FK → Employee
    public Employee? Employee { get; set; }       // navigation
}
