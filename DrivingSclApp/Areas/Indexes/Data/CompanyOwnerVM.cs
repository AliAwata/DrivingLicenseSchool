using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DrivingSclApp.Areas.Indexes.Data
{
    public class CompanyOwnerVM
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [ScaffoldColumn(false)]
        [DisplayName("الرمز الالكتروني")]
        public long NB { get; set; }
        public long COMP_NB { get; set; }
        public long PRS_NB { get; set; }
        [DisplayName("الصفة الوظيفية")]
        public string JOBTITLE { get; set; }
        [DisplayName("ملاحظات")]
        public string NOTE { get; set; }
        [DisplayName("اسم الشركة")]
        public string COMP_NAME { get; set; }
        [DisplayName("اسم المالك")]
        public string PRS_NAME { get; set; }
        [DisplayName("الرقم الوطني")]
        public string NATNO { get; set; }
        [DisplayName("اسم الاب")]
        public string FATHER { get; set; }
        [DisplayName("اسم الام")]
        public string MOTHER { get; set; }
        [DisplayName("القيد المدني")]
        public string CIVILLOC { get; set; }
        [DisplayName("الميلاد")]
        public DateTime? BDATE { get; set; }
        //[DisplayName("الجنس")]
        //public string SEX_string { get; set; }
    }
}