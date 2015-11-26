using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HK.Models
{
    public class Container
    {
        public int ContainerID { get; set; }
        public string ContainerNumber { get; set; }
        public DateTime Date { get; set; }

        [DisplayName("Shipped Per")]
        public string ShippedPer { get; set; }

        [DisplayName("On/About")]
        public string OnAbout { get; set; }

        [DataType(DataType.MultilineText)]
        [DisplayName("From")]
        public string From { get; set; }

        [DisplayName("Airway Bill No. or B/L No.")]
        public string AirwayBillNumber { get; set; }

        [DisplayName("Letter of Credit No.")]
        public string LetterOfCreditNumber { get; set; }

        [DataType(DataType.MultilineText)]
        [DisplayName("Drawn Under")]
        public string DrawnUnder { get; set; }

        //[DataType(DataType.MultilineText)]
        //[Required]
        //public string Importer { get; set;}

        public int ImporterID { get; set; }
        public virtual Importer Importer { get; set; }

        public int ExporterID { get; set; }
        public virtual Exporter Exporter { get; set; }

        public virtual List<ContainerItem> ContainerItems { get; set; }

    }
}