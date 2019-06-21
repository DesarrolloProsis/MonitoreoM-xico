using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoreoMexIra
{
    public class PlazasInfo
    {
        public string Name_caseta { get; set; }
        public string Argumentos { get; set; }
    }
    public class Argumentos
    {
        public string ListaSQL { get; set; }
        public string WebService { get; set; }
        public string ArchivosServidor { get; set; }
    }
}
