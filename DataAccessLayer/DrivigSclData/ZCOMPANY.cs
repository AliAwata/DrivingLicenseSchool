//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DrivingSclData
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("DRIVINGSCHOOLS.ZCOMPANY")]
    public partial class ZCOMPANY
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NB { get; set; }
        public string COMPNAME { get; set; }
        public string COMREG_NO { get; set; }
        public Nullable<System.DateTime> COMREG_DATE { get; set; }
        public string COMREG_TYP { get; set; }
        public Nullable<long> COMREG_GOV { get; set; }
        public Nullable<long> CTY_NB { get; set; }
        public Nullable<long> REG_NB { get; set; }
        public string ADDRESS { get; set; }
        public string PHONE1 { get; set; }
        public string PHONE2 { get; set; }
        public string MOBILE { get; set; }
        public string FAX { get; set; }
        public string NOTE { get; set; }
    
        public virtual ZCITY ZCITY { get; set; }
        public virtual ZREGION ZREGION { get; set; }
        public virtual ZGOVERN ZGOVERN { get; set; }
    }
}