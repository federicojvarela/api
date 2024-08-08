
namespace FJVApp.Models
{
    public class JsonResponse
    {
        public JsonResponse()
        {
            Status = String.Empty;
            Id = String.Empty;
        }
        public string Status { get; set; }
        public string Id { get; set; }
    }
}
