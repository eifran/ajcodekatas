﻿namespace AjHask.Tests.Functions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjHask.Language;

    class AddIntegerFunction : IMultiFunction
    {
        public int Arity
        {
            get { return 2; }
        }

        public object Value { get { return this; } }

        public IFunction Apply(IFunction parameter)
        {
            return new PartialFunction(this, parameter);
        }

        public IFunction Apply(IList<IFunction> parameters)
        {
            if (parameters == null || parameters.Count != this.Arity)
                throw new InvalidOperationException("Invalid number of parameters");

            return new ConstantFunction((int)(parameters[0].Value) + (int)(parameters[1].Value));
        }
    }
}
