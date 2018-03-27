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
        public const string OK = "Ok";
        public const string UNAUTHORIZED = "Unauthorized";
        public const string NOT_FOUND = "Not Found";
        public const string METHOD_NOT_ALLOWED = "Method Not Allowed";
        public const string INTERNAL_SERVER_ERROR = "Internal Server Error";
        public const string BAD_REQUEST = "Bad Request";
        public const string BAD_GATEWAY = "Bad Gateway";
    }
}