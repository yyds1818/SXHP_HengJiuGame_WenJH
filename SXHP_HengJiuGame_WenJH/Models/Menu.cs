//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace SXHP_HengJiuGame_WenJH.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Menu
    {
        public System.Guid ID { get; set; }
        public Nullable<System.Guid> ParentID { get; set; }
        public string MenuName { get; set; }
        public string MenuCode { get; set; }
        public string MenuUrl { get; set; }
        public int Type { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
    }
}
