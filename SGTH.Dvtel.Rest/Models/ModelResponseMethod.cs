namespace SGTH.Dvtel.Rest.Models
{
    public class ModelResponseMethod
    {
        public string Status { get; set; }
        public string Msg { get; set; }
        public object Data { get; set; }
    }
    public static class CodeStatus
    {
        public const string OK_STRING = "Ok";
        public const string ERROR = "Error";
        public const string OK = "200";
        public const string UNAUTHORIZED = "401";
        public const string NOT_FOUND = "404";
        public const string METHOD_NOT_ALLOWED = "405";
        public const string INTERNAL_SERVER_ERROR = "500";
    }
}