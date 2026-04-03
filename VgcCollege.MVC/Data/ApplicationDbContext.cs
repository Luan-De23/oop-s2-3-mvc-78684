using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Library;

namespace VgcCollege.MVC.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<Branch> Branches =>  Set<Branch>();
    public DbSet<Course> Courses =>  Set<Course>();
    public DbSet<CourseEnrolment> CourseEnrolments =>  Set<CourseEnrolment>();
    
    public DbSet<FacultyProfile> FacultyProfiles =>  Set<FacultyProfile>();
    
    public DbSet<StudentProfile> StudentProfiles =>  Set<StudentProfile>();
    public DbSet<AttendaceRecord> AttendaceRecords =>  Set<AttendaceRecord>();
    
    public DbSet<Assignment> Assignments =>  Set<Assignment>();
    public DbSet<AssignmentResult> AssignmentResults =>  Set<AssignmentResult>();
    
    public DbSet<Exam> Exams =>  Set<Exam>();
    public DbSet<ExamResult> ExamResults =>  Set<ExamResult>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Branch  1---* Course
        modelBuilder.Entity<Course>()
            .HasOne(c => c.Branch)
            .WithMany(b => b.Courses)
            .HasForeignKey(c => c.BranchId);
        
        // Course 1---* CourseEnroll
        modelBuilder.Entity<CourseEnrolment>()
            .HasOne(c => c.Course)
            .WithMany(c => c.CourseEnrolments)
            .HasForeignKey(c => c.CourseId);
        // Course 1---* Exam
        modelBuilder.Entity<Exam>()
            .HasOne(e => e.Course)
            .WithMany(c => c.Exams)
            .HasForeignKey(c => c.CourseId);
        // Course 1---* Assign
        modelBuilder.Entity<Assignment>()
            .HasOne(a => a.Course)
            .WithMany(c => c.Assignments)
            .HasForeignKey(c => c.CourseId);
        // Course *---* FacultyProf
        modelBuilder.Entity<FacultyProfile>()
            .HasMany<Course>(f => f.Courses)
            .WithMany(c => c.FacultyProfiles)
            .UsingEntity(j => j.ToTable("faculty_Courses"));
        
        // CourseEnroll 1---* Attendance
        modelBuilder.Entity<AttendaceRecord>()
            .HasOne(a => a.CourseEnrolment)
            .WithMany(s => s.AttendaceRecords)
            .HasForeignKey(a => a.CourseEnrolmentId);
        
        // StudentProfile 1---* CourseEnroll
        modelBuilder.Entity<CourseEnrolment>()
            .HasOne<StudentProfile>(c => c.StudentProfile)
            .WithMany(s => s.CoursesEnrolments)
            .HasForeignKey(c => c.StudentProfileId);
        
        // StudentProfile 1---* AssignResults
        modelBuilder.Entity<AssignmentResult>()
            .HasOne(a => a.StudentProfile)
            .WithMany(s => s.AssignmentResults)
            .HasForeignKey(a => a.StudentProfileId);
        // StudentProfile 1---* ExamResults
        modelBuilder.Entity<ExamResult>()
            .HasOne(e => e.StudentProfile)
            .WithMany(s => s.ExamResults)
            .HasForeignKey(e => e.StudentProfileId);
        
        // FacultyProfile 1---* Exam
        modelBuilder.Entity<Exam>()
            .HasOne(e => e.FacultyProfile)
            .WithMany(f => f.Exams)
            .HasForeignKey(e => e.FacultyProfileId);
        // FacultyProfile 1---* ExamResults
        modelBuilder.Entity<ExamResult>()
            .HasOne(e => e.FacultyProfile)
            .WithMany(f => f.ExamResults)
            .HasForeignKey(e => e.FacultyProfileId);
        // FacultyProfile 1---* Assign
        modelBuilder.Entity<Assignment>()
            .HasOne(a => a.FacultyProfile)
            .WithMany(s => s.Assignments)
            .HasForeignKey(a => a.FacultyProfileId);
        // FacultyProfile 1---* AssignResults
        modelBuilder.Entity<AssignmentResult>()
            .HasOne(a => a.FacultyProfile)
            .WithMany(s => s.AssignmentResults)
            .HasForeignKey(a => a.FacultyProfileId);
        
        // Exam 1---* ExamResult
        modelBuilder.Entity<ExamResult>()
            .HasOne(e => e.Exam)
            .WithMany(e => e.ExamResults)
            .HasForeignKey(e => e.ExamId);
        // Assign 1---* AssignResult
        modelBuilder.Entity<AssignmentResult>()
            .HasOne(a => a.Assigment)
            .WithMany(a => a.AssignmentResults)
            .HasForeignKey(a => a.AssigmentId);
    }
}