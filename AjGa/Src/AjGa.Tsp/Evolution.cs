﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AjGa;

namespace AjGa.Tsp
{
    public class Evolution : BaseEvolution<int, int>
    {
        public Evolution(Evaluator evaluator)
            : base(evaluator, new List<IGenomaFactory<int,int>>())
        {
        }

        public Evolution(Evaluator evaluator, List<IGenomaFactory<int, int>> operators)
            : base(evaluator, operators)
        {
        }

        protected override IPopulation<int, int> CreateNewPopulation()
        {
            return new Population();
        }
    }
}

