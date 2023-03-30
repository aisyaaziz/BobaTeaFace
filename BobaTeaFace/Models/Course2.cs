namespace BobaTeaFace.Models
{
    public class Course2
    {
        public int Id { get; set; }


        public string? ImageUrl { get; set; }

        public string ScenarioName { get; set; }
        public string Vocabularies { get; set; }

        public string ChatGPTScenario { get; set; }

        public string What { get; set; }
        public string Why { get; set; }
        public string Who { get; set; }
        public string Where { get; set; }
        public string When { get; set; }
        public string How { get; set; }

        public CourseLevel Level { get; set; }
    }
}
