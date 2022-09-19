﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace XFramework
{
    [MiniTweenType(nameof(Quaternion))]
    public class QuaternionTween : MiniTween<Quaternion>
    {
        protected override void AddElapsedTimeAfter()
        {
            var progress = base.Progress;
            var value = Quaternion.Lerp(base.startValue, base.endValue, progress);

            base.setValue_Action?.Invoke(value);
        }
    }
}
