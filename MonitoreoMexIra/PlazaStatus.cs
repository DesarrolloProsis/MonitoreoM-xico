using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoreoMexIra
{
    public class ConcentradoPlazas
    {
        public List<PlazaStatus> Concentrado = new List<PlazaStatus>();
    }
    public class PlazaStatus
    {
        public string Nombre_Plaza { get; set; }
        public string StatusPlaza { get; set; }
    }
}
