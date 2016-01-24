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

        [DataType(DataType.MultilineText)]
        [DisplayName("Costs Included")]
        public string CostsIncluded { get; set; } 

        [DataType(DataType.MultilineText)]
        [DisplayName("Harmonic Codes")]
        public string HarmonicCodes { get; set; }

        [DisplayName("Total Gross Weight")]
        public string TotalGrossWeight { get; set; }

        [DisplayName("Total CTN")]
        public string TotalCartons { get; set; }

        [DisplayName("Country Of Origin")]
        public string CountryOfOrigin { get; set; }

        [DisplayName("Beneficiary Bank")]
        public string BeneficiaryBank { get; set; }

        [DisplayName("Beneficiary SWIFT")]
        public string BeneficiarySwift { get; set; }

        [DisplayName("Beneficiary USD Account No.")]
        public string BeneficiaryUsdAccount { get; set; }

        public string ImporterName { get; set; }

        public string ImporterAddress { get; set; }
        public string ImporterTaxCertificateNumber { get; set; }

        public string ExporterName { get; set; }
        public string ExporterAddress { get; set; }


        public virtual List<ContainerItem> ContainerItems { get; set; }

    }
}