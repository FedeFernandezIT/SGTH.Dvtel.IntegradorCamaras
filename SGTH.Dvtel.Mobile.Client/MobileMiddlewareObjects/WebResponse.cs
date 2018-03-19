using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Serialization;

namespace SGTH.Dvtel.Mobile.Client.MobileMiddlewareObjects
{
    [Serializable]
    [XmlRoot("Response")]
    public class WebResponse
    {
        public const string ContentTypeTextHtmlUtf8 = "text/html; charset=utf-8";
        
        [XmlElement("Body")] public Body Body;
        [XmlElement("Header")] public Header Header;
        private string _htmlHeader;
        private string _htmlMessage;

        public WebResponse()
        {
            Header = new Header();
            Body = new Body();
        }

        protected virtual string GetResponseMessage()
        {
            //var textWriter = new StringWriter();
            var serializer = new XmlSerializer(typeof(WebResponse));
            string utf8Response;
            using (StringWriter textWriter = new Utf8StringWriter())
            {
                serializer.Serialize(textWriter, this);
                utf8Response = textWriter.ToString();
            }
            return utf8Response;
        }

        public string BuildHttpResponse()
        {
            _htmlMessage = GetResponseMessage();
            _htmlHeader = BuildHttpHeader();

            return string.Concat(_htmlHeader, _htmlMessage);
        }

        protected string BuildHttpHeader()
        {
            const HttpStatusCode status = HttpStatusCode.OK;
            //int contentLength = m_htmlMessage.Length;
            int contentLength = Encoding.UTF8.GetByteCount(_htmlMessage);

            const string xServerHeader = "";
            const string serverHeader = "";

            string contentLengthHeader = "";

            if (contentLength != 0)
                contentLengthHeader = "Content-Length:" + contentLength + "\r\n";

            string response = "HTTP/1.0 " + HttpStatusCodeToString(status) + "\r\n" +
                              "Cache-Control: no-cache\r\n" +
                              "Pragma: no-cache\r\n" +
                              "Date: " + DateTime.UtcNow.ToString("r") + "\r\n" +
                              serverHeader +
                              xServerHeader +
                              "Connection: Close\r\n" +
                              "Content-Type: " + ContentTypeTextHtmlUtf8 + "\r\n" +
                              contentLengthHeader +
                              "\n";

            return response;
        }

        private string HttpStatusCodeToString(HttpStatusCode status)
        {
            string code = (int) status + " ";
            switch (status)
            {
                case HttpStatusCode.OK:
                    return code + "OK";
                case HttpStatusCode.NotFound:
                    return code + "Not Found";
                default:
                    return code + "Something";
            }
        }
    }

    [Serializable]
    public class Header
    {
        [XmlElement("Command")] public CommandType Command;
        [XmlElement("Error")] public ErrorType Error;
        [XmlElement("SessionId")] public Guid SessionId;

        public Header()
        {
            SessionId = Guid.Empty;
            Error = ErrorType.None;
            Command = CommandType.None;
        }
    }

    [Serializable]
    public class Body
    {
        public ArchiveStream ArchiveStream;
        public Camera Camera;

        [XmlArray("Cameras"), XmlArrayItem("Camera")] public List<Camera> Cameras;
        [XmlElement("Configuration")] public Configuration ClientConfiguration;

        [XmlArray("Clips"), XmlArrayItem("Clip")] public List<Clip> Clips;

        public LiveUrl LiveUrl;

        [XmlElement("PTZ")] public PTZ Ptz;
        [XmlArray("Sites"), XmlArrayItem("Site")] public List<Site> Sites;
    }

    /// <summary>
    /// StringWriter that overrides the encoding to UTF-8
    /// </summary>
    public class Utf8StringWriter : StringWriter
    {
        [XmlIgnore]
        public override Encoding Encoding
        {
            //get { return Encoding.Unicode; }
            get { return Encoding.UTF8; }
            
        }
    }
}