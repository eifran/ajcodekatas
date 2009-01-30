﻿namespace AjGroups
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Element
    {
        private byte[] values;
        private int calculatedOrder;

        public Element(params byte[] values)
        {
            for (int k = 0; k < values.Length; k++)
            {
                if (values[k] > values.Length) 
                {
                    throw new InvalidOperationException(string.Format("Invalid value {0}", values[k]));
                }

                for (int j = k + 1; j < values.Length; j++)
                {
                    if (values[k] == values[j])
                    {
                        throw new InvalidOperationException(string.Format("Repeated value {0}", values[k]));
                    }
                }
            }

            this.values = (byte[])values.Clone();
        }

        public int Size
        {
            get
            {
                return this.values.Length;
            }
        }

        public int Order
        {
            get
            {
                if (this.calculatedOrder > 0)
                {
                    return this.calculatedOrder;
                }

                Element element = this;

                while (!element.IsIdentity())
                {
                    element = this.Multiply(element);
                    this.calculatedOrder++;
                }

                this.calculatedOrder++;

                return this.calculatedOrder;
            }
        }

        internal byte[] Values
        {
            get
            {
                return this.values;
            }
        }

        public static Element CreateIdentity(int size)
        {
            byte[] values = new byte[size];

            for (int k = 0; k < size; k++)
            {
                values[k] = (byte) k;
            }

            return new Element(values);
        }

        public static Element CreateSwap(int size) 
        {
            Element element = CreateIdentity(size);
            element.values[0] = 1;
            element.values[1] = 0;

            return element;
        }

        public static Element CreateRotation(int size)
        {
            byte[] values = new byte[size];

            for (int k = 0; k < size; k++)
            {
                values[k] = (byte)(k == size - 1 ? 0 : k + 1);
            }

            return new Element(values);
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }

            if (!(obj is Element))
            {
                return false;
            }

            Element element = (Element)obj;

            if (this.values.Length != element.values.Length)
            {
                return false;
            }

            if (this.Order != element.Order)
            {
                return false;
            }

            for (int k = 0; k < this.values.Length; k++)
            {
                if (this.values[k] != element.values[k])
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            int hash = 0;

            for (int k = 0; k < this.values.Length; k++)
            {
                hash *= 17;
                hash += this.values[k];
            }

            return hash;
        }

        public Element Multiply(Element element)
        {
            int k;
            int length1 = this.values.Length;
            int length2 = element.values.Length;
            int newlength = Math.Max(length1, length2);
            byte[] newvalues = new byte[newlength];

            for (k = 0; k < newlength; k++)
            {
                if (k >= length2)
                {
                    newvalues[k] = this.values[k];
                }
                else if (element.values[k] >= length1)
                {
                    newvalues[k] = element.values[k];
                }
                else
                {
                    newvalues[k] = this.values[element.values[k]];
                }
            }

            return new Element(newvalues);
        }

        private Boolean IsIdentity()
        {
            for (int k = 0; k < this.values.Length; k++)
            {
                if (values[k] != k)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
