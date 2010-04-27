﻿namespace AjIo.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public abstract class BaseObject : IObject
    {
        protected Dictionary<string, object> slotValues = new Dictionary<string, object>();

        public virtual void SetSlot(string name, object value)
        {
            this.slotValues[name] = value;
        }

        public virtual void UpdateSlot(string name, object value)
        {
            if (this.slotValues.ContainsKey(name)) 
            {
                this.slotValues[name] = value;
                return;
            }

            throw new InvalidOperationException(string.Format("Not defined slot '{0}'", name));
        }

        public virtual object GetSlot(string name)
        {
            if (this.slotValues.ContainsKey(name))
                return this.slotValues[name];

            return null;
        }

        public override string ToString()
        {
            return string.Format("{0}_{1:x}", this.TypeName, this.GetHashCode());
        }

        public abstract string TypeName { get; }

        public virtual IObject Self
        {
            get
            {
                return this;
            }
        }

        public object Evaluate(object expression)
        {
            IMessage message = expression as IMessage;

            if (message != null)
                return message.Send(this, this);

            ICollection<IMessage> messages = expression as ICollection<IMessage>;

            if (messages != null)
            {
                object receiver = this;

                foreach (IMessage msg in messages)
                    receiver = msg.Send(this, receiver);

                return receiver;
            }

            return expression;
        }
    }
}
