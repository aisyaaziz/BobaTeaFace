namespace BobaTeaFace.Models
{
    public class Course1
    {
        public int Id { get; set; }

        public string? ImageUrl { get; set; }

        public string ScenarioName { get; set; }
        public string Keywords { get; set; } 

        public CourseLevel Level { get; set; }
    }

    public enum CourseLevel
    {
        Low,
        Medium,
        High
    }

}
