using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DataAnalysis2.Models
{
    public class DataClass
    {
        [Display(Name = "电容距离/mm")]
        public string Distance { get; set; }
        [Display(Name = "正行程电压/mV")]
        public string ZV { get; set; }
        [Display(Name = "反行程电压/mV")]
        public string FV { get; set; }
    }
}