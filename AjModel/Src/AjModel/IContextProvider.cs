﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AjModel
{
    public interface IContextProvider
    {
        Context GetInstance();
    }
}
