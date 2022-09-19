using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFramework
{
    public abstract class AUIEvent
    {
        public abstract string GetKey();

        public abstract UI OnCreate();

        public virtual void OnRemove(UI ui)
        {

        }
    }
}
