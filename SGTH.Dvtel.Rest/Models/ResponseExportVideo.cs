using System;
using System.Collections.Generic;

namespace SGTH.Dvtel.Rest.Models
{
    public class ResponseExportVideo
    {
        public Guid idExportSession { get; set; }
        public List<string> path { get; set; }
    }
}