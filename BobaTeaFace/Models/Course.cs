using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BobaTeaFace.Models
{
    public class Course
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public Gender Gender { get; set; }
        public string ChildImageUrl { get; set; }
        public string ExternalImageUrl { get; set; }

        public string WhoQ { get; set; }
        public string WhoA { get; set; }

        public string WhatQ { get; set; }
        public string WhatA { get; set; }

        public string WhenQ { get; set; }
        public string WhenA { get; set; }

        public string WhyQ { get; set; }
        public string WhyA { get; set; }

        public string WhereQ { get; set; }
        public string WhereA { get; set; }

        public string HowQ { get; set; }
        public string HowA { get; set; }
    
        public bool isPublished { get; set; }
    }

    public enum Gender
    {
        Male = 10,
        Female = 20
    }
}
