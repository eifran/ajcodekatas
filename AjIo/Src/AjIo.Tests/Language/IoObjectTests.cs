﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using AjIo.Language;
using AjIo.Methods;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AjIo.Tests.Language
{
    [TestClass]
    public class IoObjectTests
    {
        private IoObject obj;

        [TestInitialize]
        public void Setup()
        {
            this.obj = new IoObject();
            this.obj.SetSlot("name", "Fido");
        }

        [TestMethod]
        public void GetDefinedSlot()
        {
            Assert.AreEqual("Fido", this.obj.GetSlot("name"));
        }

        [TestMethod]
        public void SetAndGetSlot()
        {
            this.obj.SetSlot("foo", "bar");
            Assert.AreEqual("bar", obj.GetSlot("foo"));
        }

        [TestMethod]
        public void GetUndefinedSlotAsNull()
        {
            Assert.IsNull(this.obj.GetSlot("foo"));
        }

        [TestMethod]
        public void CloneObject()
        {
            Message message = new Message("clone");
            object result = message.Send(null, this.obj);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IObject));
            Assert.IsInstanceOfType(result, typeof(ClonedObject));

            ClonedObject obj2 = (ClonedObject)result;

            Assert.AreEqual(obj, obj2.Parent);
        }

        [TestMethod]
        public void SetSlot()
        {
            Message message = new Message("setSlot", new object[] { "foo", "bar" });
            
            message.Send(this.obj, this.obj);

            object result = this.obj.GetSlot("foo");

            Assert.IsNotNull(result);
            Assert.AreEqual("bar", result);
        }

        [TestMethod]
        public void SetSlotAndGetMessage()
        {
            Message message = new Message("setSlot", new object[] { "foo", "bar" });

            message.Send(this.obj, this.obj);

            object result = (new Message("foo")).Send(null, this.obj);

            Assert.IsNotNull(result);
            Assert.AreEqual("bar", result);
        }

        [TestMethod]
        public void GetToString()
        {
            string text = this.obj.ToString();
            Assert.IsTrue(text.StartsWith("Object_"));
        }

        [TestMethod]
        public void GetClonedToString()
        {
            IObject cloned = new ClonedObject(this.obj);
            string text = cloned.ToString();
            Assert.IsTrue(text.StartsWith("Object_"));
        }

        [TestMethod]
        public void NewSlot()
        {
            Message message = new Message("newSlot", new object[] { "foo", "bar" });

            message.Send(this.obj, this.obj);

            object result = this.obj.GetSlot("foo");

            Assert.IsNotNull(result);
            Assert.AreEqual("bar", result);

            object result2 = this.obj.GetSlot("setFoo");

            Assert.IsNotNull(result2);
            Assert.IsInstanceOfType(result2, typeof(SetterMethod));

            SetterMethod method = (SetterMethod)result2;

            Assert.AreEqual("foo", method.SlotName);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RaiseIfUpdateUndefinedSlot()
        {
            this.obj.UpdateSlot("undefined", "bar");
        }
    }
}

