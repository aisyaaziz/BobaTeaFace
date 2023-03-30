using Newtonsoft.Json;

namespace BobaTeaFace.ViewModels
{
    public class ChatGPTAPIModelInput
    {
        public int max_tokens { get; set; } = 2048;
        public double temperature { get; set; } = 0.5;
        public string? model { get; set; }
        public List<ChatGPTAPIModelInputMessage> messages { get; set; }
    }
    public class ChatGPTAPIModelInputMessage
    {
        public string? role { get; set; }
        public string? content { get; set; }
    }

    // model for the DALL E api response
    public class ChatGPTAPIModelResponseModel
    {
        public string id { get; set; }
        [JsonProperty("object")]
        public string chatgptobject {get;set; }
        public long created { get; set; }
        public string model { get; set; }
        public List<ChatGPTAPIModelResponseModelChoice> choices { get; set; }
        public ChatGPTAPIModelResponseModelUsage usage { get; set; }
    }
    public class ChatGPTAPIModelResponseModelChoice
    {
        public ChatGPTAPIModelInputMessage message { get; set; }
        public int index { get; set; }
        public string? logprobs { get; set; }
        public string finish_reason { get; set; }
    }
    public class ChatGPTAPIModelResponseModelUsage
    {
        public int prompt_tokens { get; set; }
        public int completion_tokens { get; set; }
        public int total_tokens { get; set; }
    }
}