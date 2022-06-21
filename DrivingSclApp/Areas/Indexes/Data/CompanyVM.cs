using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DrivingSclApp.Areas.Indexes.Data
{
    public class CompanyVM
    {
        [DisplayName("الرمز الإلكتروني")]
        public long NB { get; set; }
        [DisplayName("اسم الشركة")]
        public string COMPNAME { get; set; }
        [DisplayName("رقم تجاري للشركة")]
        public string COMREG_NO { get; set; }
        [DisplayName("تاريخ السجل التجاري")]
        public DateTime? COMREG_DATE { get; set; }
        [DisplayName("نوع السجل التجاري")]
        public string COMREG_TYP { get; set; }
        public long? COMREG_GOV { get; set; }
        public long? CTY_NB { get; set; }
        public long? REG_NB { get; set; }
        [DisplayName("العنوان")]
        public string ADDRESS { get; set; }
        [DisplayName("الهاتف الاول")]
        public string PHONE1 { get; set; }
        [DisplayName("الهاتف الثاني")]
        public string PHONE2 { get; set; }
        [DisplayName("المحمول")]
        public string MOBILE { get; set; }
        [DisplayName("الفاكس")]
        public string FAX { get; set; }
        [DisplayName("ملاحظات")]
        public string NOTE { get; set; }
        [DisplayName("المحافظة")]
        public string GovName { get; set; }
        [DisplayName("المدينة")]
        public string CityName { get; set; }
        [DisplayName("المنطقة")]
        public string RegionName { get; set; }
    }
}