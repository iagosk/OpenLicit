using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLicit
{
    public class ErroFormatoInvalido : Exception
    {
        public ErroFormatoInvalido(string msg) : base(msg) { }
    }
}
