﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class HenJiuGameEntities : DbContext
    {
        public HenJiuGameEntities()
            : base("name=HenJiuGameEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<UserInfo> UserInfo { get; set; }
        public virtual DbSet<Job> Job { get; set; }
        public virtual DbSet<Game> Game { get; set; }
        public virtual DbSet<GameRole> GameRole { get; set; }
        public virtual DbSet<Menu> Menu { get; set; }
        public virtual DbSet<Structure> Structure { get; set; }
        public virtual DbSet<RoleMenus> RoleMenus { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
    }
}
