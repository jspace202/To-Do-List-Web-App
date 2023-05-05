namespace CCFinal.CanvasIntegration.Services;

public record Data(AllCourses data);

public class AllCourses {
    public Course[] allCourses { get; set; }
}

public class Course {
    public string Name { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Assignmentsconnection AssignmentsConnection { get; set; }
}

public class Assignmentsconnection {
    public Node[] Nodes { get; set; }
}

public class Node {
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DueAt { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
    public DateTime? UnlockAt { get; set; }
    public string Description { get; set; }
    public string[] SubmissionTypes { get; set; }
    public Uri HtmlUrl { get; set; }
}
