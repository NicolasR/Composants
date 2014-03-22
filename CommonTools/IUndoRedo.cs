using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stl.Tme.Components.Tools
{
    interface IUndoRedo
    {
        void Do();
        void Undo();
    }
}
