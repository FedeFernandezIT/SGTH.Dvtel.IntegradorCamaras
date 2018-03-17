using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DvTelIntegradorCamaras.Models
{
    public class ResponseExportVideo
    {
        public Guid idExportSession { get; set; }
        public List<string> path { get; set; }
    }
}