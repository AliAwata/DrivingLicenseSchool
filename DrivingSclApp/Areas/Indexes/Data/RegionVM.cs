using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DrivingSclApp.Areas.Indexes.Data
{
    public class RegionVM
    {
        [DisplayName("الرمز الالكتروني")]
        public long NB { get; set; }
        [DisplayName("اسم المنطقة")]
        public string NAME { get; set; }
        public long CTY_NB { get; set; }
        [DisplayName("المدينة")]
        public string CityName { get; set; }
    }
}