using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLicit
{
    public class ErroCampoVazio : Exception
    {
        public ErroCampoVazio(string msg) : base(msg) { }
    }
}
