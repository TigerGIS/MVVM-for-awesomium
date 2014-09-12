﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMAwesonium.Infra
{
    public static class TaskHelper
    {
        public static Task<T> FromResult<T>(T result)
        {
            TaskCompletionSource<T> res = new TaskCompletionSource<T>();
            res.SetResult(result);
            return res.Task;
        }
    }
}