using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DrivingSclApp.Areas.Schools.Data
{
    public class SchoolDocVm
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [ScaffoldColumn(false)]
        [DisplayName("الرمز الالكتروني")]
        public long NB { get; set; }
        public long SCL_NB { get; set; }
        [DisplayName("تاريخ الوثيقة")]
        public Nullable<System.DateTime> DAT { get; set; }
        [DisplayName("رقم الوثيقة")]
        public string NUM { get; set; }
        public long TYPE_NB { get; set; }
        [DisplayName("صادرة عن")]
        public string ISSUER { get; set; }
        [DisplayName("الوصف")]
        public string DESCRIB { get; set; }
        public long USAGE_NB { get; set; }
        [DisplayName("ملف الوثيقة")]
        public string DOCFILE { get; set; }
        [DisplayName("ملاحظات")]
        public string NOTE { get; set; }
        [DisplayName("المدرسة")]
        public string SCL_NAME { get; set; }
        [DisplayName("نوع الوثيقة")]
        public string TYPE_NAME { get; set; }
        [DisplayName("نمط الاستخدام")]
        public string USAGE_NAME { get; set; }
    }
}