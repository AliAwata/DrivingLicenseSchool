using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DrivingSclApp.Areas.Schools.Data
{
    public class SchoolVehiclesVM
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [DisplayName("الرمز الالكتروني")]
        public long NB { get; set; }
        public long SCL_NB { get; set; }
        public long GOV_NB { get; set; }
        [DisplayName("رقم اللوحة")]
        public string BOARDNO { get; set; }
        [DisplayName("الهيكل")]
        public string VIN { get; set; }
        [DisplayName("رقم المحرك")]
        public string ENGINENO { get; set; }
        public Nullable<long> CLS_NB { get; set; }
        [DisplayName("اللون")]
        public string COLOR { get; set; }
        [DisplayName("العرض")]
        public string WIDTH { get; set; }
        [DisplayName("الطول")]
        public string LENG { get; set; }
        [DisplayName("سعة المحرك")]
        public Nullable<int> ENGINESIZE { get; set; }
        [DisplayName("سنة الصنع")]
        public Nullable<int> PRODYEAR { get; set; }
        [DisplayName("سنة التصنيع")]
        public Nullable<int> REGYEAR { get; set; }
        [DisplayName("الصانع")]
        public string BRAND { get; set; }
        [DisplayName("الطراز")]
        public string MODELNO { get; set; }
        [DisplayName("عدد المقاعد")]
        public Nullable<short> SEATS { get; set; }
        public Nullable<long> PRS_NB { get; set; }
        [DisplayName("تاريخ انتهاء الترخيص")]
        public Nullable<System.DateTime> LICENSEEXPIRYDATE { get; set; }
        [DisplayName("المدرسة")]
        public string SCL_NAME { get; set; }
        [DisplayName("المالك")]
        public string PRS_NAME { get; set; }
        [DisplayName("المحافظة")]
        public string GOV_NAME { get; set; }
        [DisplayName("الصف")]
        public string CLS_NAME { get; set; }

    }
}