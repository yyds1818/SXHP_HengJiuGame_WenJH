using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SXHP_HengJiuGame_WenJH.Models
{
    public class TreeTb_EX
    {
        public Guid? id { get; set; }

        public string name { get; set; }

        public string code { get; set; }

        public int type { get; set; }

        public string url {  get; set; }

        public DateTime? createDate { get; set; }

        public Guid? parentId { get; set; }

        public IEnumerable<TreeTb_EX> children { get; set; }

        public bool isParent { get; set; }
    }
}