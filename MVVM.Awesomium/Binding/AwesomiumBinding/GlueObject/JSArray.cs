﻿using Awesomium.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMAwesomium.Infra;
using System.Collections;
using MVVMAwesomium.Exceptions;

namespace MVVMAwesomium.AwesomiumBinding
{
    internal class JSArray : GlueBase, IJSObservableBridge
    {
        public JSArray(IEnumerable<IJSCSGlue> values, IEnumerable collection, Type ElementType)
        {
            JSValue = new JSValue(values.Select(v => v.JSValue).ToArray());
            Items = new List<IJSCSGlue>(values);
            CValue = collection;
            IndividualType = ElementType;
        }

        private Type IndividualType { get; set; }

        public CollectionChanges GetChanger(JSValue[] value, JSValue[] status, JSValue[] index, IJSCBridgeCache bridge)
        {
            return new CollectionChanges(bridge, value, status, index, IndividualType);
        }

        private void ReplayChanges(IndividualCollectionChange change, IList ilist)
        {
            switch (change.CollectionChangeType)
            {
                case CollectionChangeType.Add:
                    if (change.Index == ilist.Count)
                    {
                        ilist.Add(change.Object.CValue);
                        Items.Add(change.Object);
                    }
                    else
                    {
                        ilist.Insert(change.Index, change.Object.CValue);
                        Items.Insert(change.Index, change.Object);
                    }
                    break;

                case CollectionChangeType.Remove:
                    ilist.RemoveAt(change.Index);
                    Items.RemoveAt(change.Index);
                    break;
            }
        }

        public void UpdateEventArgsFromJavascript(CollectionChanges iCollectionChanges)
        {
            IList ilist = CValue as IList;
            if (ilist == null) return;

            iCollectionChanges.IndividualChanges.ForEach(c => ReplayChanges(c, ilist));
        }



        public void Add(IJSCSGlue iIJSCBridge, int Index)
        {
            ((JSObject)MappedJSValue).InvokeAsync("silentsplice", new JSValue(Index), new JSValue(0), iIJSCBridge.GetJSSessionValue());
            if (Index > Items.Count - 1)
                Items.Add(iIJSCBridge);
            else
                Items.Insert(Index, iIJSCBridge);
        }

        public void Insert(IJSCSGlue iIJSCBridge, int Index)
        {
            ((JSObject)MappedJSValue).InvokeAsync("silentsplice", new JSValue(Index), new JSValue(1), iIJSCBridge.GetJSSessionValue());
            Items[Index] = iIJSCBridge;
        }

        public void Remove(int Index)
        {
            ((JSObject)MappedJSValue).InvokeAsync("silentsplice", new JSValue(Index), new JSValue(1));
            Items.RemoveAt(Index);
        }

        public void Reset()
        {
            ((JSObject)MappedJSValue).InvokeAsync("silentremoveAll");
            Items.Clear();
        }

        protected override void ComputeString(StringBuilder sb, HashSet<IJSCSGlue> alreadyComputed)
        {
            sb.Append("[");
            bool f = true;
            foreach (var it in Items)
            {
                if (!f)
                    sb.Append(",");
                f = false;
                it.BuilString(sb, alreadyComputed);
            }

            sb.Append("]");
        }

        public JSValue JSValue { get; private set; }

        public object CValue { get; private set; }

        public IList<IJSCSGlue> Items { get; private set; }

        public IEnumerable<IJSCSGlue> GetChildren()
        {
            return Items;
        }

        public JSCSGlueType Type { get { return JSCSGlueType.Array; } }

        public JSValue MappedJSValue { get; private set; }

        public void SetMappedJSValue(JSValue ijsobject, IJSCBridgeCache mapper)
        {
            MappedJSValue = ijsobject;
        }

    }
}
