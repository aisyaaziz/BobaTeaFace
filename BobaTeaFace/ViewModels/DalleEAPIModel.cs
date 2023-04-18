namespace BobaTeaFace.ViewModels
{
    // serves as our input model
    public class DalleEAPIModelInput
    {
        public string model { get; set; }
        public string? prompt { get; set; }
        public short? n { get; set; }
        public string? size { get; set; }
        public string response_format { get; set; } = "url";
    }

    // model for the image url
    public class DalleEAPIModelLink
    {
        public string? url { get; set; }
    }

    // model for the DALL E api response
    public class DalleEAPIModelResponseModel
    {
        public long created { get; set; }
        public List<DalleEAPIModelLink>? data { get; set; }
    }
}
