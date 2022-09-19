using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XFramework
{
    [MiniTweenType(nameof(Int64))]
    public class LongTween : MiniTween<long>
    {
        protected override void AddElapsedTimeAfter()
        {
            var startValue = base.startValue;
            var endValue = base.endValue;
            var interval = endValue - startValue;
            var progress = base.Progress;
            var sign = interval < 0 ? -1 : 1;
            var value = (long)(startValue + Mathf.Abs(interval) * sign * progress);

            if (sign >= 0)
                value = Math.Min(value, endValue);
            else
                value = Math.Max(value, endValue);

            base.setValue_Action?.Invoke(value);
        }
    }
}
