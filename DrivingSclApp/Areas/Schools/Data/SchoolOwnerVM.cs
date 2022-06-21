using DrivingSclData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DrivingSclApp.Areas.Schools.Data
{
    public class SchoolOwnerVM
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [ScaffoldColumn(false)]
        [DisplayName("الرمز الالكتروني")]
        public long NB { get; set; }
        public long SCL_NB { get; set; }
        public long OT_NB { get; set; }
        public long ONR_NB { get; set; }
        [DisplayName("ملاحظات")]
        public string NOTE { get; set; }
        [DisplayName("اسم المالك")]
        public string OwnerName { get; set; }
        [DisplayName("كنية المالك")]
        public string OwnerLname { get; set; }
        [DisplayName("نوع المالك")]
        public string OwnerTypName { get; set; }
        [DisplayName("اسم المدرسة")]
        public string SchoolName { get; set; }
        public virtual ZOWNERTYP ZOWNERTYP { get; set; }
        public virtual SCHOOL SCHOOL { get; set; }
    }
}