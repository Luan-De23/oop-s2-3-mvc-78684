namespace VgcCollege.Library;

public enum AttendaceStatus
{
    Present,
    Absent,
    Late
}

public class AttendaceRecord
{
    public int Id { get; set; }
    public DateOnly WeekDate { get; set; }
    public AttendaceStatus Present { get; set; }
    
    // FK's
    public int CourseEnrolmentId  { get; set; }
    public CourseEnrolment CourseEnrolment { get; set; }
    
    public int StudentProfileId { get; set; }
    public StudentProfile? StudentProfile { get; set; }
    
    // Relations
    public ICollection<CourseEnrolment> CourseEnrolments { get; set; } = new List<CourseEnrolment>();
}