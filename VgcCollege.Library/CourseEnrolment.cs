namespace VgcCollege.Library;

public enum status
{
    Enrolled,
    NotEnrolled
}

public class CourseEnrolment
{
    public int Id { get; set; }
    public DateOnly EnrolDate { get; set; }
    public status Status { get; set; }
    
    //PK's
    public int StudentProfileId { get; set; }
    public StudentProfile? StudentProfile { get; set; }
    
    public int CourseId { get; set; }
    public Course? Course { get; set; }
    
    // Relations
    public ICollection<StudentProfile> Profiles { get; set; } = new List<StudentProfile>();
    public ICollection<AttendaceRecord> AttendaceRecords { get; set; } = new List<AttendaceRecord>();
}