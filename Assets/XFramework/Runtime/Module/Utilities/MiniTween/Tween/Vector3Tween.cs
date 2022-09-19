﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XFramework
{
    [MiniTweenType(nameof(Vector3))]
    public class Vector3Tween : MiniTween<Vector3>
    {
        protected override void AddElapsedTimeAfter()
        {
            var progress = base.Progress;
            var value = Vector3.Lerp(base.startValue, base.endValue, progress);

            base.setValue_Action?.Invoke(value);
        }
    }
}
