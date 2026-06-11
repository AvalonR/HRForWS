using HRAPI.Enums;
using HRAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HRAPI.Data;

// Central EF Core context: defines tables, relationships, constraints, and Identity integration.
public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<SalaryHistory> SalaryHistories => Set<SalaryHistory>();
    public DbSet<PayrollRecord> PayrollRecords => Set<PayrollRecord>();
    public DbSet<Deduction> Deductions => Set<Deduction>();
    public DbSet<PerformanceReview> PerformanceReviews => Set<PerformanceReview>();
    public DbSet<JobPosting> JobPostings => Set<JobPosting>();
    public DbSet<Applicant> Applicants => Set<Applicant>();
    public DbSet<Application> Applications => Set<Application>();
    public DbSet<Interview> Interviews => Set<Interview>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // IdentityDbContext configures ASP.NET Identity tables before HR-specific mappings are added.
        base.OnModelCreating(modelBuilder);

        // ── AppUser 

        // Links a login account to an optional employee profile without requiring every user to be an employee.
        modelBuilder.Entity<AppUser>()
            .HasOne(u => u.Employee)
            .WithMany()
            .HasForeignKey(u => u.EmployeeId)
            .OnDelete(DeleteBehavior.SetNull);

        // Unique Indexes 
        // Unique indexes prevent duplicate business identifiers such as department codes and employee emails.
        modelBuilder.Entity<Department>()
            .HasIndex(d => d.Code)
            .IsUnique();

        modelBuilder.Entity<Employee>()
            .HasIndex(e => e.EmployeeNumber)
            .IsUnique();

        modelBuilder.Entity<Employee>()
            .HasIndex(e => e.Email)
            .IsUnique();

        modelBuilder.Entity<Position>()
            .HasIndex(p => new { p.Title, p.DepartmentId })
            .IsUnique();

        modelBuilder.Entity<LeaveType>()
            .HasIndex(lt => lt.Name)
            .IsUnique();

        modelBuilder.Entity<Applicant>()
            .HasIndex(a => a.Email)
            .IsUnique();

        modelBuilder.Entity<Attendance>()
            .HasIndex(a => new { a.EmployeeId, a.Date })
            .IsUnique();

        // ── Enum String Conversions ──

        // Stores enum values as readable strings in SQLite instead of numeric values.
        modelBuilder.Entity<Attendance>()
            .Property(a => a.Status)
            .HasConversion<string>();

        modelBuilder.Entity<LeaveRequest>()
            .Property(lr => lr.Status)
            .HasConversion<string>();

        modelBuilder.Entity<JobPosting>()
            .Property(jp => jp.EmploymentType)
            .HasConversion<string>();

        modelBuilder.Entity<JobPosting>()
            .Property(jp => jp.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Application>()
            .Property(a => a.Status)
            .HasConversion<string>();

        modelBuilder.Entity<PerformanceReview>()
            .Property(pr => pr.Status)
            .HasConversion<string>();

        modelBuilder.Entity<PayrollRecord>()
            .Property(pr => pr.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Interview>()
            .Property(i => i.Type)
            .HasConversion<string>();

        modelBuilder.Entity<Deduction>()
            .Property(d => d.Type)
            .HasConversion<string>();

        // ── Decimal Precision ──

        // Money fields use consistent precision because salaries and payroll values need predictable decimals.
        modelBuilder.Entity<SalaryHistory>()
            .Property(s => s.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<PayrollRecord>()
            .Property(p => p.BaseSalary)
            .HasPrecision(18, 2);

        modelBuilder.Entity<PayrollRecord>()
            .Property(p => p.Overtime)
            .HasPrecision(18, 2);

        modelBuilder.Entity<PayrollRecord>()
            .Property(p => p.Bonuses)
            .HasPrecision(18, 2);

        modelBuilder.Entity<PayrollRecord>()
            .Property(p => p.DeductionsTotal)
            .HasPrecision(18, 2);

        modelBuilder.Entity<PayrollRecord>()
            .Property(p => p.NetPay)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Position>()
            .Property(p => p.MinSalary)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Position>()
            .Property(p => p.MaxSalary)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Deduction>()
            .Property(d => d.Amount)
            .HasPrecision(18, 2);

        // Cascade / Restrict Behavior 

        // Self-referencing: prevent cascade cycles
        // Restricting deletes protects department hierarchies from accidental cascading deletion.
        modelBuilder.Entity<Department>()
            .HasOne(d => d.ParentDepartment)
            .WithMany(d => d.SubDepartments)
            .HasForeignKey(d => d.ParentDepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Managers are employees too, so this prevents deleting one employee from cascading through the team.
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Manager)
            .WithMany(e => e.Subordinates)
            .HasForeignKey(e => e.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        // PerformanceReview has two FKs to Employee → avoid multiple cascade paths
        // Reviewer is also an employee, so delete behavior is restricted to avoid ambiguous cascades.
        modelBuilder.Entity<PerformanceReview>()
            .HasOne(pr => pr.Reviewer)
            .WithMany()
            .HasForeignKey(pr => pr.ReviewerId)
            .OnDelete(DeleteBehavior.Restrict);

        // LeaveRequest has two FKs to Employee → avoid multiple cascade paths
        // ReviewedByEmployee is optional because new requests start pending before HR reviews them.
        modelBuilder.Entity<LeaveRequest>()
            .HasOne(lr => lr.ReviewedByEmployee)
            .WithMany()
            .HasForeignKey(lr => lr.ReviewedByEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Interview has optional FK to Employee
        modelBuilder.Entity<Interview>()
            .HasOne(i => i.Interviewer)
            .WithMany()
            .HasForeignKey(i => i.InterviewerId)
            .OnDelete(DeleteBehavior.Restrict);

        // ── Seed Data ──

        // Default leave types make the leave request workflow usable right after migrations run.
        modelBuilder.Entity<LeaveType>().HasData(
            new LeaveType { Id = 1, Name = "Annual", DaysAllowed = 20, IsPaid = true },
            new LeaveType { Id = 2, Name = "Sick", DaysAllowed = 10, IsPaid = true },
            new LeaveType { Id = 3, Name = "Personal", DaysAllowed = 5, IsPaid = false },
            new LeaveType { Id = 4, Name = "Maternity", DaysAllowed = 90, IsPaid = true },
            new LeaveType { Id = 5, Name = "Paternity", DaysAllowed = 10, IsPaid = true }
        );
    }
}
