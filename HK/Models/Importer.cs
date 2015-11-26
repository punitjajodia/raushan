using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HK.Models
{
    public class Importer
    {
        public int ImporterID {get; set;}

        [Required]
        [DisplayName("Importer Name")]
        public string ImporterName { get; set; }

        [Required]
        [DisplayName("Importer Address")]
        [DataType(DataType.MultilineText)]
        public string ImporterAddress { get; set; }

        [DisplayName("Tax Certicate No")]
        public string TaxCertificateNumber { get; set; }
    }
}