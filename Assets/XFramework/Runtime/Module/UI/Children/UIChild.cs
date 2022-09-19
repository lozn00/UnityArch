using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace XFramework
{
    public class UIChild : UI
    {
        public int Id { get; private set; }

        public void SetId(int id)
        {
            this.Id = id;
        }

        protected override void OnClose()
        {
            base.OnClose();
            if (this.Parent != null && !this.Parent.IsDisposed)
            {
                if (this.Parent is UIChildrenList list)
                    list.RemoveFormChildrenList(this.Id);
            }

            this.Id = -1;
        }
    }
}
