﻿namespace AjIo.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjIo.Methods;

    public class IoObject : BaseObject
    {
        public IoObject()
        {
            this.SetSlot("clone", new CloneMethod());
            this.SetSlot("setSlot", new SetSlotMethod());
            this.SetSlot("newSlot", new NewSlotMethod());
            this.SetSlot("updateSlot", new UpdateSlotMethod());
            this.SetSlot(":=", new SetSlotMethod());
            this.SetSlot("::=", new NewSlotMethod());
            this.SetSlot("=", new UpdateSlotMethod());
            this.SetSlot("method", new MethodMethod());
        }

        public override string TypeName { get { return "Object"; } }
    }
}