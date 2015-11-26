using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HK.Models
{
    public class Exporter
    {
        public int ExporterID {get; set;}
        [Required]
        [DisplayName("Exporter Name")]
        public string ExporterName { get; set; }

        [Required]
        [DisplayName("Exporter Address")]
        [DataType(DataType.MultilineText)]
        public string ExporterAddress { get; set; }
    }
}