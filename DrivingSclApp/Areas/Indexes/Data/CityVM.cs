using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DrivingSclApp.Areas.Indexes.Data
{
    public class CityVM
    {
        [DisplayName("الرمز الإلكتروني")]
        public long NB { get; set; }
        [DisplayName("المدينة")]
        public string NAME { get; set; }
        [ScaffoldColumn(false)]
        public long GOV_NB { get; set; }
        [DisplayName("المحافظة")]
        public string GOVNAME { get; set; }
    }
}