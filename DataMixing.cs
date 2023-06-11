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

    public partial class DataMixing
    {



        public int DataDictionaryId { get; set; }
        public string DataDescription { get; set; }
        public int DataMixingRuleId { get; set; }
        public string DataMixingDescription { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime? ThruDate { get; set; }
        public int DataMixingMasterId { get; set; }
        public int DataMixingId { get; set; }
        public int DataMixingScopeId { get; set; }
        public bool? IsActive { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }




    }
}
