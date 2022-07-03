using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrivingSclApp.Areas.Schools.Data
{
    public class SchoolTrainerVM
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [DisplayName("الرمز الالكتروني")]
        public long NB { get; set; }
        public long PRS_NB { get; set; }
        public long SCL_NB { get; set; }
        public long TYP_NB { get; set; }
        [DisplayName("الشهادة العلمية")]
        public string DIPLOM { get; set; }
        [DisplayName("رقم إجازة التدريب")]
        public string LICENSENO { get; set; }
        [DisplayName("تاريخ إجازة التدريب")]
        public Nullable<System.DateTime> LICENSEDATE { get; set; }
        [DisplayName("مصدر إجازة التدريب")]
        public string LICENSEFROM { get; set; }
        [DisplayName("اسم المدرب")]
        public string PRS_NAME { get; set; }
        [DisplayName("اسم المدرسة")]
        public string SCL_NAME { get; set; }
        [DisplayName("الإختصاص")]
        public string TYP_NAME { get; set; }
    }
}