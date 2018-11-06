using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProcesadorElecciones
{
    class ManejadorXML
    {
        private XElement documento;
        private IEnumerable<XElement> datosBase;
        private string ficheroOriginalSinExtensión;

        public IEnumerable<XElement> DatosBase { get => datosBase;}

        public ManejadorXML(string nombre_archivo)
        {
            LoadDocument(nombre_archivo);
            ficheroOriginalSinExtensión = nombre_archivo.Split('.')[0];
        }
        

        public void GenerarSVGPorcentaje(IDictionary<string, double> datos, IDictionary<string, int> datosExtra)
        {

            XNamespace nm = "http://www.w3.org/2000/svg";

            XElement res =
                new XElement(nm + "svg",
                    new XAttribute("version", "2.0"),
                    new XElement(nm + "title", "RESULTADOS ELECTORALES"),
                    new XElement(nm + "text", "RESULTADOS ELECTORALES",
                        new XAttribute("transform", "translate(250, 30), scale(2)"),
                        new XAttribute("style", "fill: #555; font-family: Helvetica, sans-serif;")),
                    new XElement(nm + "g",
                            datos.Where(p  => Logica.IsNombrePartido(p.Key))
                            .Where(v => v.Value > 0.5)
                            .Select(
                                    (d, barN) => new XElement(nm + "g",
                                            new XElement(nm + "rect",
                                                new XAttribute("width", Convert.ToInt32(d.Value)),
                                                new XAttribute("height", 19),
                                                new XAttribute("y", 20*barN),
                                                new XAttribute("transform", "scale(7, 1)"),
                                                new XAttribute("style", "fill: #aaa;")),
                                            new XElement(nm + "text", d.Key + ": " + Math.Round(d.Value, 2) + "%",
                                                new XAttribute("x", Convert.ToInt32(d.Value)*7 + 5),
                                                new XAttribute("dy", ".35em"),
                                                new XAttribute("y", 20*barN + 9),
                                                new XAttribute("style", "fill: #555; font-family: Helvetica, sans-serif;"))
                                            )),
                            new XAttribute("transform" , "translate(20, 50), scale(1.5)")
                                            ),
                 new XElement(nm + "g",
                    new XElement(nm + "text", "Total de votos: " + datosExtra["Votos"]),
                    new XElement(nm + "text", "Porcentaje de participación: " + Math.Round(datosExtra["Votantes"]*100d/(datosExtra["Votantes"] + datosExtra["Abstenciones"]),2) +"%",
                        new XAttribute("transform", "translate(0, 20)")),
                    new XElement(nm + "text", "Abstenciones: " + Math.Round(datosExtra["Abstenciones"] * 100d / (datosExtra["Votantes"] + datosExtra["Abstenciones"]),2) + "%",
                        new XAttribute("transform", "translate(0, 40)")),
                    new XElement(nm + "text", "Votos en blanco: " + Math.Round(datosExtra["Blancos"] * 100d / (datosExtra["Votos"]), 2) +"%",
                        new XAttribute("transform", "translate(0, 60)")),
                 
                    new XAttribute("transform", "translate(700, 120)"),
                    new XAttribute("style", "fill: #555; font-family: Helvetica, sans-serif;")
                 ));

            CreateDocument(res, ficheroOriginalSinExtensión + "_resultadosPorcentaje.svg");
        }

        internal void GenerarSVGTotal(IOrderedEnumerable<KeyValuePair<string, int>> datos, IDictionary<string, int> datosExtra)
        {
            XNamespace nm = "http://www.w3.org/2000/svg";

            XElement res =
                new XElement(nm + "svg",
                    new XAttribute("version", "2.0"),
                    new XElement(nm + "title", "RESULTADOS ELECTORALES"),
                    new XElement(nm + "text", "RESULTADOS ELECTORALES",
                        new XAttribute("transform", "translate(250, 30), scale(2)"),
                        new XAttribute("style", "fill: #555; font-family: Helvetica, sans-serif;")),
                    new XElement(nm + "g",
                            datos.Where(p => Logica.IsNombrePartido(p.Key))
                            .Where(v => v.Value > 1000)
                            .Select(
                                    (d, barN) => new XElement(nm + "g",
                                            new XElement(nm + "rect",
                                                new XAttribute("width", Convert.ToInt32(d.Value * 100d / datos.Where(p => Logica.IsNombrePartido(p.Key)).Sum(v => v.Value))),
                                                new XAttribute("height", 19),
                                                new XAttribute("y", 20 * barN),
                                                new XAttribute("transform", "scale(7, 1)"),
                                                new XAttribute("style", "fill: #aaa;")),
                                            new XElement(nm + "text", d.Key + ": " + d.Value,
                                                new XAttribute("x", Convert.ToInt32(d.Value*100d/datos.Where(p=>Logica.IsNombrePartido(p.Key)).Sum(v => v.Value)) * 7 + 5),
                                                new XAttribute("dy", ".35em"),
                                                new XAttribute("y", 20 * barN + 9),
                                                new XAttribute("style", "fill: #555; font-family: Helvetica, sans-serif;"))
                                            )),
                            new XAttribute("transform", "translate(20, 50), scale(1.5)")
                                            ),
                 new XElement(nm + "g",
                    new XElement(nm + "text", "Total de votos: " + datosExtra["Votos"]),
                    new XElement(nm + "text", "Votantes: " + datosExtra["Votantes"],
                        new XAttribute("transform", "translate(0, 20)")),
                    new XElement(nm + "text", "Abstenciones: " +datosExtra["Abstenciones"],
                        new XAttribute("transform", "translate(0, 40)")),
                    new XElement(nm + "text", "Votos en blanco: " + datosExtra["Blancos"],
                        new XAttribute("transform", "translate(0, 60)")),

                    new XAttribute("transform", "translate(700, 120)"),
                    new XAttribute("style", "fill: #555; font-family: Helvetica, sans-serif;")
                 ));

            CreateDocument(res, ficheroOriginalSinExtensión + "_resultadosTotal.svg");
        }
        
        
        
        private void LoadDocument(string nombre_archivo)
        {
            try
            {
                this.documento = XElement.Load(nombre_archivo);
                this.datosBase = documento.Descendants("dato");
            }
            catch (FileNotFoundException)
            {
                throw;
            }
        }

        private void CreateDocument(XElement toSave, string nombre_archivo)
        {
            try
            {
                toSave.Save(nombre_archivo);
            }
            catch (FileNotFoundException)
            {
                throw;
            }
        }
        




    }
}
