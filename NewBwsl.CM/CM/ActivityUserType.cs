//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace NewMK.Model.CM
{
    using System;
    using System.Collections.Generic;
    
    public partial class ActivityUserType
    {
        public System.Guid ID { get; set; }
        public System.Guid ActivityID { get; set; }
        public Nullable<int> DeLevelID { get; set; }
    
        public virtual Activity Activity { get; set; }
    }
}