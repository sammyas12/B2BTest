using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace B2BTest
{
    internal class Program
    {
        static void Main()
        {
            Console.Title = "Demo B2B Samuel Arciniega";
            string Vurl = $"https://us-central1-b2b-hub-82515.cloudfunctions.net/app/api/Ej1?UserID=sammy.as@hotmail.com&companyID=123456789&portalID=oaXh7EU0ExaygAvvZM3y&facturaID=L90107";
            Console.WriteLine("--Obteniendo Partes de factura.");
            string VRespuesta = EjecutarApi(Vurl, "GET"); 
            if (VRespuesta.Contains("Error:") == true) 
            {
                Console.WriteLine(VRespuesta);
                Console.WriteLine("preciono enter para salir.");
                Console.ReadLine();
            }
            else
            {
                string DetallesRespuesta = VRespuesta.Remove(0, 1).Remove((VRespuesta.Length - 2), 1).Replace("\"", ""); 

                int ss = DetallesRespuesta.IndexOf(',');

                string Textototal = DetallesRespuesta.Substring(DetallesRespuesta.IndexOf("total:"));
                double Totalfactura = Convert.ToDouble(Textototal.Substring(0, Textototal.IndexOf(',')).Replace("total:",""));

                DetallesRespuesta = DetallesRespuesta.Substring(DetallesRespuesta.IndexOf("partidas")).Replace("[", ",").Replace(",{", "*");
                DetallesRespuesta = DetallesRespuesta.Replace("]", "").Replace("}", "");
                string[] ListaPates = DetallesRespuesta.Split('*');
                int Nopartida = 0;
                double TotalSumado = 0;
                foreach (string p in ListaPates)
                {
                    if (p.Contains("partidas") == false)
                    {
                        List<string> DetallesPartida = p.Split(',').ToList();
                        double Total = Convert.ToDouble(DetallesPartida.Single(x => x.Contains("Precio:")).Replace("Precio:", "").Replace("}", ""));
                        TotalSumado = TotalFactura(TotalSumado, Total);
                        Nopartida++;
                        Console.WriteLine($"--Partida no.{Nopartida} con {DetallesPartida.Single(x => x.Contains("id:"))}, cantidad: {Total.ToString("C2")}");
                    }
                }
                Console.WriteLine("");
                if(Totalfactura == TotalSumado || TotalSumado == (Totalfactura + 0.1) || TotalSumado == (Totalfactura - 0.1))
                {
                    string VurlCompleto = "https://us-central1-b2b-hub-82515.cloudfunctions.net/app/api/Ej1?UserID=sammy.as@hotmail.co&companyID=123456789&portalID=oaXh7EU0ExaygAvvZM3y&facturaID=L90107&notification=\"La factura fue adendada correctamente \"";
                    string VRespuestaCompleto = EjecutarApi(VurlCompleto, "PUT");
                    if (VRespuesta.Contains("Error:") == true)
                    {
                        Console.WriteLine(VRespuesta);
                        Console.WriteLine("preciono enter para salir.");
                        Console.ReadLine();
                    }
                    else
                    {
                        Console.WriteLine("Exito!!");
                        Console.WriteLine("preciono enter para salir.");
                        Console.ReadLine();
                    }
                }
                else
                {
                    Console.WriteLine("Error: el total no coincide con el total de las partes.");
                    Console.WriteLine("preciono enter para salir.");
                }
                Console.ReadLine();
            }
        }

        public static double TotalFactura(double total, double costo)
        {
            double TotalSumado;
            TotalSumado = total + costo;
            return TotalSumado;
        }

        public static string EjecutarApi(string url,string metodo)
        {

            string resultado;
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
            webrequest.Method = metodo;
            webrequest.ContentType = "application/x-www-form-urlencoded";
            webrequest.ContentLength = 0;
            HttpWebResponse webresponse;
            try
            {
                webresponse = (HttpWebResponse)webrequest.GetResponse();
                if (webresponse.StatusCode == HttpStatusCode.OK)
                {
                    string ss = webresponse.GetResponseStream().ToString();
                    StreamReader responseStream = new StreamReader(webresponse.GetResponseStream(), Encoding.UTF8);
                    resultado = responseStream.ReadToEnd();
                    Console.WriteLine(responseStream.ReadToEnd());
                    webresponse.Close();
                    return resultado;
                }
                else
                {
                    resultado = $"--Error: {webresponse.StatusCode}";
                    webresponse.Close();
                    return resultado;
                }
            }
            catch (Exception ex) 
            {
                resultado = $"--Error: {ex}";
                return resultado;
            }
            
        }
    }
}
