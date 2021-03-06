//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DrivingSclData
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("DRIVINGSCHOOLS.SCHOOL")]
    public partial class SCHOOL
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SCHOOL()
        {
            this.SCLPHONE = new HashSet<SCLPHONE>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NB { get; set; }
        public string SCL_CODE { get; set; }
        public string SCLNAME { get; set; }
        public long ST_NB { get; set; }
        public long GOV_NB { get; set; }
        public string COMREG_NO { get; set; }
        public Nullable<System.DateTime> COMREG_DATE { get; set; }
        public string COMREG_TYP { get; set; }
        public Nullable<long> COMREG_GOV { get; set; }
        public Nullable<long> CTY_NB { get; set; }
        public Nullable<long> REG_NB { get; set; }
        public string ADDRESS { get; set; }
        public long STS_NB { get; set; }

        public virtual ICollection<SCLPHONE> SCLPHONE { get; set; }
        public virtual ICollection<SCHOOLOWNER> SCHOOLOWNERs { get; set; }
        public virtual ICollection<SCHOOLVEHICLE> SCHOOLVEHICLEs { get; set; }
        public virtual ICollection<SCHOOLTRAINER> SCHOOLTRAINERs { get; set; }
        public virtual ICollection<SCHOOLDOC> SCHOOLDOCs { get; set; }
        public virtual ZCITY ZCITY { get; set; }
        public virtual ZGOVERN ZGOVERN { get; set; }
        public virtual ZREGION ZREGION { get; set; }
        public virtual ZSCLTYPE ZSCLTYPE { get; set; }
        public virtual ZSCLSTATUS ZSCLSTATUS { get; set; }
    }
}
