using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DrivingSclApp.Areas.Schools.Data
{
    public class SchoolVM
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [ScaffoldColumn(false)]
        [DisplayName("الرمز الالكتروني")]
        public long NB { get; set; }
        public string SCL_CODE { get; set; }
        [DisplayName("اسم المدرسة")]
        public string SCLNAME { get; set; }
        public long ST_NB { get; set; }
        public long GOV_NB { get; set; }
        [DisplayName("رقم السجل التجاري")]
        public string COMREG_NO { get; set; }
        [DisplayName("تاريخ السجل التجاري")]
        public Nullable<System.DateTime> COMREG_DATE { get; set; }
        [DisplayName("نوع السجل التجاري")]
        public string COMREG_TYP { get; set; }
        [DisplayName("رمز مصدر السجل التجاري")]
        public Nullable<long> COMREG_GOV { get; set; }
        public Nullable<long> CTY_NB { get; set; }
        public Nullable<long> REG_NB { get; set; }
        [DisplayName("العنوان")]
        public string ADDRESS { get; set; }
        public long STS_NB { get; set; }
        [DisplayName("المحافظة")]
        public string GOV_NAME { get; set; }
        [DisplayName("المدينة")]
        public string CTY_NAME { get; set; }
        [DisplayName("المنطقة")]
        public string REG_NAME { get; set; }
        [DisplayName("حالة المدرسة")]
        public string STS_NAME { get; set; }
        [DisplayName("نوع المدرسة")]
        public string STY_NAME { get; set; }
    }
}