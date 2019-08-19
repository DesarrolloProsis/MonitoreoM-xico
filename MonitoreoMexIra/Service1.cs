using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace MonitoreoMexIra
{
    public partial class Service1 : ServiceBase
    {
        private System.Timers.Timer timer = null;
        int intentos;
        public ConcentradoPlazas NuevoConcentrado = new ConcentradoPlazas();

        private static readonly TelegramBotClient Bot = new TelegramBotClient("834404388:AAG8JcPTHi9API16h1TF5C_EgsB78QToaP8");

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timer = new System.Timers.Timer();
            timer.Interval = 300000;
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;
            timer.Start();
        }

        public void Inicio()
        {
            timer = new System.Timers.Timer();
            timer.Interval = 10000;
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;
            timer.Start();
        }

        public void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Monitorear();
        }

        protected override void OnStop()
        {
        }

        public async void Monitorear()

        {
            NuevoConcentrado.Concentrado.Clear();
            try
            {
                timer.Enabled = false;
                intentos++;
                List<PlazasInfo> plazas = new List<PlazasInfo>();
                List<Argumentos> args = new List<Argumentos>();
                string Mensaje = string.Empty;

                using (HttpClient client = new HttpClient())
                {
                    var json = await client.GetStringAsync("http://pc004.sytes.net:185/api/values");
                    var data = JArray.Parse(json);

                    foreach (var Plaza in data)
                    {
                        plazas.Add(new PlazasInfo
                        {
                            Name_caseta = Plaza["Name_caseta"].ToString(),
                            Argumentos = Plaza["Argumentos"].ToString()
                        });
                    }
                }

                foreach (var Plaza in plazas)
                {
                    if (Plaza.Argumentos.ToLower().Contains("sin"))
                    {
                        if (Plaza.Argumentos == "Sin Conexion Sql")
                        {
                            NuevoConcentrado.Concentrado.Add(new PlazaStatus
                            {
                                Nombre_Plaza = Plaza.Name_caseta,
                                StatusPlaza = "Sin Conexión SQL"
                            });
                            Mensaje += "*Plaza: " + NuevoConcentrado.Concentrado.LastOrDefault().Nombre_Plaza + "* \n" + NuevoConcentrado.Concentrado.LastOrDefault().StatusPlaza + "\n";
                        }
                        else if (intentos == 36 && Plaza.Argumentos != "Sin Conexion Sql")
                        {
                            NuevoConcentrado.Concentrado.Add(new PlazaStatus
                            {
                                Nombre_Plaza = Plaza.Name_caseta,
                                StatusPlaza = "Sin Conexión a Plaza"
                            });
                        }
                    }
                    else
                    {
                        var jsonargs = JArray.Parse("[" + Plaza.Argumentos + "]");
                        PlazaStatus NPlazaStatus = new PlazaStatus();
                        foreach (var argumento in jsonargs)
                        {
                            args.Add(new Argumentos
                            {
                                ListaSQL = argumento["ListaSQL"][3].ToString(),
                                WebService = argumento["WebService"][1].ToString(),
                                ArchivosServidor = argumento["ArchivosServidor"][3].ToString()
                            });

                            NPlazaStatus = GetStatus(Plaza.Name_caseta, args.LastOrDefault());

                            if (intentos == 36)
                            {
                                NuevoConcentrado.Concentrado.Add(new PlazaStatus
                                {
                                    Nombre_Plaza = NPlazaStatus.Nombre_Plaza,
                                    StatusPlaza = NPlazaStatus.StatusPlaza
                                });
                            }
                            else if (NPlazaStatus.StatusPlaza != "OK")
                            {
                                Mensaje += "*Plaza: " + NPlazaStatus.Nombre_Plaza + "* \n" + NPlazaStatus.StatusPlaza + "\n";
                            }
                        }
                    }
                }
                if (intentos == 36)
                {
                    intentos = 0;
                    Mensaje = "*Resumen de Plazas* \n";
                    foreach (var item in NuevoConcentrado.Concentrado)
                    {
                        Mensaje += "*Plaza: " + item.Nombre_Plaza + "* \n" + item.StatusPlaza + "\n";
                    }
                    await Bot.SendTextMessageAsync(343941115, "*México-Irapuato* \n" + Mensaje, Telegram.Bot.Types.Enums.ParseMode.Markdown);
                }
                else if (Mensaje != string.Empty)
                {
                    await Bot.SendTextMessageAsync(343941115, "*México-Irapuato* \n" + Mensaje, Telegram.Bot.Types.Enums.ParseMode.Markdown);
                }
                timer.Enabled = true;
            }
            catch (Exception ex)
            {
                await Bot.SendTextMessageAsync(343941115, "*México-Irapuato* \n" + ex.Message, Telegram.Bot.Types.Enums.ParseMode.Markdown);
                timer.Enabled = true;
            }
          
        }
        public PlazaStatus GetStatus(string NombrePlaza, Argumentos args)
        {
            string Observaciones = string.Empty;
            if (args.ListaSQL == "True")
                Observaciones += "Listas SQL atrasadas\n";
            if (args.WebService == "True")
                Observaciones += "Web Service atrasado\n";
            if (args.ArchivosServidor == "True")
                Observaciones += "Archivos Servidor atrasado\n";
            return new PlazaStatus
            {
                Nombre_Plaza = NombrePlaza,
                StatusPlaza = Observaciones == string.Empty ? "OK" : Observaciones
            };         
        }
    }
}