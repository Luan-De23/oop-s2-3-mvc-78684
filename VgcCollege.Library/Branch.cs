namespace VgcCollege.Library;

public class Branch
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    
    // FK's
    
    
    // Relations
    public ICollection<Course> Courses { get; set; } = new List<Course>();
}