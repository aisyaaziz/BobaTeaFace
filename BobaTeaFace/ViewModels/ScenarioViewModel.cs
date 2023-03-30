﻿namespace BobaTeaFace.ViewModels
{
    public class ScenarioViewModel
    {
        public int vocablevel { get; set; }
        public int Gender { get; set; }
        public string Scenario { get; set; }
        public int quantity { get; set; }
        public string size { get; set; }
    }
    public class ScenarioResponseViewModel
    {
        public string scenario { get; set; }
        public List<DalleEAPIModelLink>? data { get; set; }
    }
}
