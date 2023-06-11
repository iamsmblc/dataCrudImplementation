using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataDictionaryWebApi.Models
{

    public partial class DataMixingMaster
    {
        public int DataMixingMasterId { get; set; }
        public string DataMixingVersion { get; set; }
        public string  DataMixingVersionDescription { get; set; }

        public short? IsApproved { get; set; }

        public DateTime FromDate { get; set; }
        public int ExcelLink { get; set; }
        public int ApprovalMailLink { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}