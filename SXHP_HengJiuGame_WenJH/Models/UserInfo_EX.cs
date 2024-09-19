using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SXHP_HengJiuGame_WenJH.Models
{
    public class UserInfo_EX:UserInfo
    {
        public string OldPassWord {  get; set; }

        public string job {  get; set; }

        public string structure {  get; set; }
    }
}