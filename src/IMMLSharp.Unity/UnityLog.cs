﻿using Imml.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace IMMLSharp
{
    public class UnityLog : ILog
    {
        public void Write(string message)
        {
            Debug.Log(message);
        }
    }
}
