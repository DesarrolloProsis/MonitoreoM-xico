using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestMonitoreoMexIra
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Btn_Inicio_Click(object sender, EventArgs e)
        {
            MonitoreoMexIra.Service1 service = new MonitoreoMexIra.Service1();
            service.Inicio();
        }
    }
}
