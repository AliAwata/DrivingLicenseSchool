using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DrivingSclApp.Areas.Indexes.Data
{
    public class PersonVM
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [ScaffoldColumn(false)]
        [DisplayName("الرمز الالكتروني")]
        public long NB { get; set; }
        [DisplayName("الرقم الوطني")]
        public string NATNO { get; set; }
        [DisplayName("الاسم")]
        public string FNAME { get; set; }
        [DisplayName("الكنية")]
        public string LNAME { get; set; }
        [DisplayName("اسم الاب")]
        public string FATHER { get; set; }
        [DisplayName("اسم الام")]
        public string MOTHER { get; set; }
        [DisplayName("القيد المدني")]
        public string CIVILLOC { get; set; }
        [DisplayName("العنوان الحالي")]
        public string ACTADDRESS { get; set; }
        [DisplayName("العنوان")]
        public string ADDRESS { get; set; }
        [DisplayName("الهاتف")]
        public string PHONE { get; set; }
        [DisplayName("المحمول")]
        public string MOBILE { get; set; }
        [DisplayName("يوم الميلاد")]
        public long? BDATED { get; set; }
        [DisplayName("شهر الميلاد")]
        public long? BDATEM { get; set; }
        [DisplayName("سنة الميلاد")]
        public long? BDATEY { get; set; }
        [DisplayName("الميلاد")]
        
        public DateTime? BDATE { get; set; }
        [DisplayName("رقم الهوية")]
        public string IDCARDNO { get; set; }
        [DisplayName("تاريخ الهوية")]
        public DateTime? IDCARDDAT { get; set; }
        [DisplayName("مكان الميلاد")]
        public string BPLACE { get; set; }
        [DisplayName("الامانة")]
        public string ALAMANA { get; set; }
        [DisplayName("الجنس")]
        public bool? SEX { get; set; }
        [DisplayName("الجنس")]
        public string SEX_string { get; set; }
        public long TYP { get; set; }
        public long NAT { get; set; }
        [DisplayName("الجنسية")]
        public string NationName { get; set; }
        [DisplayName("نوع الشخص")]
        public string PerType { get; set; }
    }
}