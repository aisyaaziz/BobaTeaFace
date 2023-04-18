namespace BobaTeaFace.ViewModels
{
    public class ScenarioImageViewModel
    {
        public int vocablevel { get; set; }
        public int Gender { get; set; }
        public string Scenario { get; set; }
        public int quantity { get; set; }
        public string size { get; set; }
        
        public IFormFile image { get; set; }    
    }
    public class ScenarioViewModel
    {
        public int vocablevel { get; set; }
        public int Gender { get; set; }
        public string Scenario { get; set; }
        public int quantity { get; set; }
        public string size { get; set; }
    }
    public class ScenarioChatViewModel
    {
        public string guid { get; set; }
        public string message { get; set; }
    }
    public class ScenarioResponseViewModel
    {
        public string scenario { get; set; }
        public string childImageUrl { get; set; }
        public List<DalleEAPIModelLink>? data { get; set; }
    }
    public class ScenarioDallEResponseViewModel
    {
        public string scenario { get; set; }
        public string childImageUrl { get; set; }
        public string externalImageUrl { get; set; }
    }
    public class ScenarioWHRequestViewModel
    {
        public string Title { get; set; }
        public string Scenario { get; set; }
        public int Gender { get; set; }
    }
    public class ScenarioWHResponseViewModel
    {
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
    }
}
