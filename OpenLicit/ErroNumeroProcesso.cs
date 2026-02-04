using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLicit
{
    public class ErroNumeroProcesso : Exception
    {
        public ErroNumeroProcesso(string msg) : base(msg) { }
    }
}
