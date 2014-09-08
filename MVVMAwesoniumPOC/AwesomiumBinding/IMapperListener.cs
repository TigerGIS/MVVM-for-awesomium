﻿using Awesomium.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVVMAwesonium.AwesomiumBinding
{
    internal interface IMapperListener
    {
        void RegisterFirst(JSObject iRoot);

        void RegisterMapping(JSObject iFather,string att, JSObject iChild);

        void RegisterCollectionMapping(JSObject iFather, string att, int index, JSObject iChild);
    }
}