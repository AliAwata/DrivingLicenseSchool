using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DrivingSclApp.Areas.Schools.Data
{
    public class SclPhoneVM
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [DisplayName("الرمز الالكتروني")]
        public long NB { get; set; }
        public long SCL_NB { get; set; }
        [DisplayName("رقم الهاتف")]
        public string PHONE_NO { get; set; }
        public byte PHONE_TYP { get; set; }
        [DisplayName("اسم المدرسة")]
        public string SCL_NAME { get; set; }
        [DisplayName("نوع الهاتف")]
        public string PHONE_TYPE { get; set; }
    }
}