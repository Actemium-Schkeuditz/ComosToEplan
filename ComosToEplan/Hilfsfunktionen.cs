

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Xml.Serialization;
using Excel = Microsoft.Office.Interop.Excel;

namespace ComosToEplan
{

    /// <summary>
    /// Hilfsfunktionen
    /// </summary>
    public partial class MainWindow

    {

        /// <summary>
        /// FileopenDialog
        /// </summary>
        /// <param name="filterTyp"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        private string selectFileFromDiskFilter(Filetypen filterTyp, string directory)

        {
            string fileSelect = "";

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = filterTyp.filter;
            openFileDialog.Title = filterTyp.title;
            openFileDialog.InitialDirectory = directory;
            if (openFileDialog.ShowDialog() == true)
            {
                fileSelect = openFileDialog.FileName;
            }
            else
            {
                fileSelect = "";
            }
            return fileSelect;
        }
        /// <summary>
        /// Erzeugen des richtigen Geräte BMKs
        /// </summary>
        /// <param name="AS">Steuerung A1.Ax</param>
        /// <param name="pl"></param>
        /// <param name="ioStation"></param>
        /// <returns></returns>
        private string bmkRio(string AS, string pl, string ioStation)
        {
            string bmk = "Fehler";
            int _pl = StringToInt(pl);
            if (AS == "A1.A4") //FS RIOs
            {
                if (ioStation == "UF04")
                    bmk = "=PL12+6m.UF04";
                else if (ioStation == "UF05")
                    bmk = "=PL11+6m.UF05";
                else if (ioStation == "UF06")
                    bmk = "=PL24+6m.UF06";
                else if (ioStation == "UF07")
                    bmk = "=PL21+6m.UF07";
                else if (ioStation == "UF08")
                    bmk = "=PL31+0m.UF08";
                else if (ioStation == "UF09")
                    bmk = "=PL24+6m.UF09";
                else
                    bmk = "Fehler";
            }
                else //normale RIOs
            {
                if ((_pl == 31)|| (_pl == 30))
                {
                    if (ioStation == "UF04")
                        bmk = "=PL31+12m.UF04";
                    else
                    bmk = "=PL31+0m." + ioStation;
                }
                else if ((_pl >= 11) && (_pl < 30))
                {
                    if ((ioStation == "UF01") )
                        bmk = "=PL" + pl + "+0m." + ioStation;
                    else if (ioStation == "UF02")
                        bmk = "=PL" + pl + "+6m." + ioStation;
                    else if ((ioStation == "UF03"))
                        bmk = "=PL" + pl + "+12m." + ioStation;
                    else if (ioStation == "UF04")
                        bmk =  "=PL" + pl + "+12m." + ioStation;
                    else
                    bmk = "Fehler";
                }
                else if ((_pl >= 41) && (_pl < 50))
                {
                    if ((ioStation == "UF02"))
                        bmk = "=PL41+6m." + ioStation;
                    else if (ioStation == "UF03")
                        bmk = "=PL44+6m." + ioStation;                 
                    else
                        bmk = "Fehler";
                }
            }
            return bmk;
        }

        /// <summary>
        /// Erzeugen des richtigen Geräte BMKs für PA-Kästen
        /// </summary>
        /// <param name="pl"></param>
        /// <param name="busstrang"></param>
        /// <param name="sk3"></param>
        /// <param name="trunk"></param>
        /// <returns></returns>
        private string bmkPAKaesten(string pl, string busstrang, byte sk3, byte trunk)
        {
            string bmk = "Fehler";
            string _busStrang = busstrang.Substring(2, 1);
            int _pl = StringToInt(pl);

            if ((_pl == 31) || (_pl == 30))
            {
                if (_busStrang == "2")
                    bmk = "=PL31+0m." + _busStrang + sk3 + trunk;
                else if (_busStrang == "3")
                    switch (trunk)
                    {
                        case 1:
                            bmk = "=PL31+0m." + _busStrang + sk3 + trunk;
                            break;
                        case 2:
                            bmk = "=PL31+12m." + _busStrang + sk3 + trunk;
                            break;
                        default:
                            MessageBox.Show(String.Format("Fehler beim ermitteln des BMK der PA Kästen PL: {0} , Busstrang: {1}, SK3-Koppler: {2}, Trunk: {3} ", pl, busstrang, sk3, trunk));
                            break;
                    }
            }
            else if ((_pl >= 11) && (_pl < 30))
            {
                switch (trunk)
                {
                    case 1:
                        bmk = "=PL" + pl + "+0m." + _busStrang + sk3 + trunk;
                        break;
                    case 2:
                        bmk = "=PL" + pl + "+6m." + _busStrang + sk3 + trunk;
                        break;
                    case 3:
                        bmk = "=PL" + pl + "+12m." + _busStrang + sk3 + trunk;
                        break;
                    case 4:
                        bmk = "=PL" + pl + "+12m." + _busStrang + sk3 + trunk;
                        MessageBox.Show("Ebenen der PA-Kästen prüfen!");
                        break;
                    default:
                        MessageBox.Show(String.Format("Fehler beim ermitteln des BMK der PA Kästen PL: {0} , Busstrang: {1}, SK3-Koppler: {2}, Trunk: {3} ", pl, busstrang, sk3, trunk));
                        break;
                }
            }
            else if ((_pl >= 40) && (_pl <= 49))
            {
                switch (trunk)
                {
                    case 1:
                        bmk = "=PL45+0m." + _busStrang + sk3 + trunk;
                        break;
                    case 2:
                        bmk = "=PL41+6m." + _busStrang + sk3 + trunk;
                        break;
                    case 3:
                        bmk = "=PL44+6m." + _busStrang + sk3 + trunk;
                        break;                    
                    default:
                        MessageBox.Show(String.Format("Fehler beim ermitteln des BMK der PA Kästen PL: {0} , Busstrang: {1}, SK3-Koppler: {2}, Trunk: {3} ", pl, busstrang, sk3, trunk));
                        break;
                }
            }
            else
                MessageBox.Show(String.Format("Fehler beim ermitteln des BMK der PA Kästen PL: {0} , Busstrang: {1}, SK3-Koppler: {2}, Trunk: {3} ", pl, busstrang, sk3, trunk));
            //Rückgabe Ergebnis
            return bmk;
        }

        /// <summary>
        /// BMK der Feldbarieren ermitteln
        /// </summary>
        /// <param name="bmkKasten"></param>
        /// <param name="feldbarriere"></param>
        /// <param name="slaveTyp"></param>
        /// <returns></returns>
        private string bmkFeldbarieren(string bmkKasten, byte feldbarriere, string slaveTyp)
        {
            string _bmk = "Fehler";
           
            var temp = from _paKasten in dataPAConfig.PABestueckung
                       where _paKasten.BMK == bmkKasten
                       select new { _paKasten.anzahlFeldbarrieren, _paKasten.anzahlPAPt100, _paKasten.anzahlSensoranschaltung };

           foreach (var item in temp)
            {
                if (slaveTyp == "R4D0-FB-IA10")
                {
                    if (item.anzahlFeldbarrieren == 1)
                        _bmk = "E1";

                    else if (item.anzahlFeldbarrieren == 2)
                    {
                        if (feldbarriere == 1)
                            _bmk = "E2";
                        else if (feldbarriere == 2)
                            _bmk = "E1";
                        else
                        {
                            MessageBox.Show(String.Format("Für den PA-KAsten {0} passt die Anzahl der Feldbarrieren {1} nicht!", bmkKasten, feldbarriere));
                            _bmk = "Fehler";
                        }
                    }
                    else if (item.anzahlFeldbarrieren == 3)
                    {
                        if (feldbarriere == 1)
                            _bmk = "E3";
                        else if (feldbarriere == 2)
                            _bmk = "E2";
                        else if (feldbarriere == 3)
                            _bmk = "E1";
                        else
                        {
                            MessageBox.Show(String.Format("Für den PA-KAsten {0} passt die Anzahl der Feldbarrieren {1} nicht!", bmkKasten, feldbarriere));
                            _bmk = "Fehler";
                        }
                    }
                }
                else if (slaveTyp == "RD0-TI-EX8.PA")
                {
                    if (item.anzahlPAPt100 == 1)
                        _bmk = "E4";
                    else if (item.anzahlPAPt100 == 2)
                    {
                        if (feldbarriere == 1)
                            _bmk = "E5";
                        else if (feldbarriere == 2)
                            _bmk = "E4";
                        else
                        {
                            MessageBox.Show(String.Format("Für den PA-KAsten {0} passt die Anzahl der Pt100-Anschaltungen nicht {1} nicht!", bmkKasten, feldbarriere));
                            _bmk = "Fehler";
                        }
                    }
                    else if (item.anzahlPAPt100 == 3)
                    {
                        if (feldbarriere == 1)
                            _bmk = "E6";
                        else if (feldbarriere == 2)
                            _bmk = "E5";
                        else if (feldbarriere == 3)
                            _bmk = "E4";
                        else
                        {
                            MessageBox.Show(String.Format("Für den PA-KAsten {0} passt die Anzahl der Pt100-Anschaltungen nicht {1} nicht!", bmkKasten, feldbarriere));
                            _bmk = "Fehler";
                        }
                    }
                }
                else if ((slaveTyp == "FD0-BI-EX12.PA") || (slaveTyp == "R8D0-MIO-Ex12.PA"))
                {
                    if (item.anzahlSensoranschaltung == 1)
                        _bmk = "E7";
                    else if (item.anzahlSensoranschaltung == 2)
                    {
                        if (feldbarriere == 1)
                            _bmk = "E8";
                        else if (feldbarriere == 2)
                            _bmk = "E7";
                        else
                        {
                            MessageBox.Show(String.Format("Für den PA-KAsten {0} passt die Anzahl der PA-DI-Anschaltungen nicht {1} nicht!", bmkKasten, feldbarriere));
                            _bmk = "Fehler";
                        }
                    }
                    else if (item.anzahlSensoranschaltung == 3)
                    {
                        if (feldbarriere == 1)
                            _bmk = "E9";
                        else if (feldbarriere == 2)
                            _bmk = "E8";
                        else if (feldbarriere == 3)
                            _bmk = "E7";
                        else
                        {
                            MessageBox.Show(String.Format("Für den PA-KAsten {0} passt die Anzahl der PA-DI-Anschaltungen nicht {1} nicht!", bmkKasten, feldbarriere));
                            _bmk = "Fehler";
                        }
                    }
                }
                else
                {
                    MessageBox.Show(String.Format("Für den PA-KAsten {0} passt der Slavetyp {1} nicht!", bmkKasten, slaveTyp));
                    _bmk = "Fehler";
                }
            }




            return _bmk;
        }
        /// <summary>
        /// Bezeichnung der Luftanschlüsse der RIO´s
        /// </summary>
        /// <param name="bmkKasten"></param>
        /// <param name="steckplatz"></param>
        /// <param name="Kanal"></param>
        /// <returns></returns>
        private string bmkRioLuftAnschluss(string bmkKasten, int steckplatz, int Kanal)
        {
            string _bmk = "Fehler";
            string _kanal = ((Kanal + 1).ToString());

            var temp = from _rioKasten in dataRioConfig.RemoteIO
                       where _rioKasten.BMK == bmkKasten
                       select new { _rioKasten.Ventilinsel, _rioKasten.ersterSteckplatzVentileInsel, _rioKasten.BMK };

            foreach (var item in temp)
            {
                //_bmk = steckplatz + Offset der Kartenbezeichnung im Rio +XM + (Kanal +1)
                if (item.Ventilinsel == true)
                {
                    _bmk = (51 - item.ersterSteckplatzVentileInsel + steckplatz).ToString() + "XM" + _kanal; 
                }
                else
                {
                    MessageBox.Show(string.Format("Der Rio {0} ist keine Ventilinsel!", item.BMK));
                    _bmk = "Fehler";
                }
            }

            return _bmk;
        }
        /// <summary>
        /// Geräteanschlüsse ermitteln
        /// </summary>
        /// <param name="hersteller"></param>
        /// <param name="geräteTyp"></param>
        /// <param name="polarität"></param>
        /// <returns></returns>
        private string anschlussGeräte(string hersteller, string geräteTyp, string polarität)
        {
            string _bmk = "Fehler";

            if ((hersteller == string.Empty) || (geräteTyp == string.Empty))
            {
                if (polarität == "Plus")
                    _bmk = "+";
                else if (polarität == "Minus")
                    _bmk = "-";
                else
                {
                    MessageBox.Show("Fehler in der Anwendungskonfiguration. Fehler im Modul 'Geräteanschluß'");
                    _bmk = "Fehler";
                    }
            }
            else
            {
                //MSRGeräte_alt.GeräteAnschlüsse[] gerätedaten = MSRGeräte_alt.GetGeräteAnschlüsse();
                
            var temp = from _geräteAnschlüsse in dataGeräteConfig.Feldgerraete  //gerätedaten
                       where _geräteAnschlüsse.Hersteller == hersteller && _geräteAnschlüsse.Gerätetyp == geräteTyp
                       select new { _geräteAnschlüsse.Anschluß_Plus, _geräteAnschlüsse.Anschluß_Minus,};
          
            foreach (var item in temp)
            {
                    if (polarität == "Plus")
                        _bmk = item.Anschluß_Plus;
                    else if (polarität == "Minus")
                        _bmk = item.Anschluß_Minus;
                    else
                    {
                        MessageBox.Show("Fehler in der Anwendungskonfiguration. Fehler im Modul 'Geräteanschluß'");
                        _bmk = "Fehler";
                    }
                }
            }
            return _bmk;
        }
        /// <summary>
        /// Daten für Druckluftpumpen aufbereiten
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private List<PumpenConfig> sensorenAnPumpen(MSRGeraete data)
        {
            
            //Liste der Sensoren
            //Hubregelung
            List<PumpenConfig> sensoren = new List<PumpenConfig>();
            sensoren.AddRange(
                   // var sensor =
                  (from temp in data.Pumpen
                   where temp.Sensoren.Hubregelung != null
                   select new PumpenConfig
                   {
                       BMK = temp.BMK,
                       Bezeichnung = temp.Bezeichnung,
                       Sensor = temp.Sensoren.Hubregelung,
                       PL = temp.PL,
                       Ebene = temp.Ebene}).ToList());

            //Membranüberwachung_außen_1
               sensoren.AddRange(                                  
                    (from temp in data.Pumpen
                     where temp.Sensoren.Membranüberwachung_außen_1 != null
                     select new PumpenConfig
            {
                BMK = temp.BMK,
                Bezeichnung = temp.Bezeichnung,
                Sensor = temp.Sensoren.Membranüberwachung_außen_1,
                         PL = temp.PL,
                         Ebene = temp.Ebene
                     }).ToList());
            //Membranüberwachung_außen_2
             sensoren.AddRange(
             (from temp in data.Pumpen
             where temp.Sensoren.Membranüberwachung_außen_2 != null
              select new PumpenConfig
              {
                  BMK = temp.BMK,
                  Bezeichnung = temp.Bezeichnung,
                  Sensor = temp.Sensoren.Membranüberwachung_außen_2,
                  PL = temp.PL,
                  Ebene = temp.Ebene
              }).ToList());
            //Membranüberwachung_innen_1
             sensoren.AddRange(
             (from temp in data.Pumpen
             where temp.Sensoren.Membranüberwachung_innen_1 != null
              select new PumpenConfig
              {
                  BMK = temp.BMK,
                  Bezeichnung = temp.Bezeichnung,
                  Sensor = temp.Sensoren.Membranüberwachung_innen_1,
                  PL = temp.PL,
                  Ebene = temp.Ebene
              }).ToList());
            //Membranüberwachung_innen_2
            sensoren.AddRange(
             (from temp in data.Pumpen
            where temp.Sensoren.Membranüberwachung_innen_2 != null
            select new PumpenConfig
            {
                BMK = temp.BMK,
                Bezeichnung = temp.Bezeichnung,
                Sensor = temp.Sensoren.Membranüberwachung_innen_2,
                PL = temp.PL,
                Ebene = temp.Ebene
            }).ToList());
            return sensoren;
        }

        // ---> Hilfsfunktionen
        private int RowCount(string xlfile, int sheetindex, int column)
        {
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlBook = xlApp.Workbooks.Open(xlfile);
            Excel.Worksheet xlSheet = xlBook.Sheets[sheetindex];
            Excel.Range xlRange = xlSheet.Cells[xlSheet.Rows.Count, column];
            return xlRange.get_End(Excel.XlDirection.xlUp).Row;
        }
      
        /// <summary>
        ///Buchstabe nach Zahl wandeln 
        /// </summary>
        /// <param name="input"></param>
        /// <returns>int</returns>
        public int StringToInt(string input)
        {
            int output = 0;
            try
            {
                if (!string.IsNullOrEmpty(input))
                {

                    if (!int.TryParse(input, out output))
                    {
                        MessageBox.Show("Fehler beim Zeichen wamdeln! StringToInt");
                    }
                }
                else
                    output = 0;
            }
            catch (NullReferenceException e)
            {
                output = 0;
                MessageBox.Show("Fehler beim Wandeln einer Zahl! StringToInt");
                Console.WriteLine(e.Message);
            }
            return output;

        }

        /// <summary>
        ///  Deep Clone für das umkopieren von Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns> 
        /// Object  welches umkopiert wird 
        /// </returns>
        public static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;    //auf Position 0 springen
                return (T)formatter.Deserialize(ms);
            }
        }

        /// <summary>
        /// Excel schliesen
        /// </summary>
        /// <param name="obj"></param>
        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Unable to release the Object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }

        /// <summary>
        /// noch zu testen und zu optimieren
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="data"></param>
        private void writeXml(string filename, object data)
        {
            // löschen der Datei damit alles neu geschrieben wird
            // sonst sind Datenreste drin
            
            try
            {
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }
            // Ausgabe Datei einrichten
            XmlSerializer serializer = new XmlSerializer(typeof(MSR));
           // XmlSerializer serializer = new XmlSerializer(data.GetType());
            // Konfig schreiben
            FileStream fs = new FileStream(filename, FileMode.OpenOrCreate);

                serializer.Serialize(fs,data);
                //Config Datei schliessen        
                fs.Flush();
                fs.Close();
            }
            catch
            {
                MessageBox.Show(string.Format("Fehler beim Schreiben der XML-Daten {0}", filename));
            }
        }


        /// <summary>
        /// noch zu testen und zu optimieren
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="data"></param>
        private void writeXmlComos(string filename, object data)
        {
            // löschen der Datei damit alles neu geschrieben wird
            // sonst sind Datenreste drin

            try
            {
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }
                // Ausgabe Datei einrichten
                XmlSerializer serializer = new XmlSerializer(typeof(ComosToXML));
                // XmlSerializer serializer = new XmlSerializer(data.GetType());
                // Konfig schreiben
                FileStream fs = new FileStream(filename, FileMode.OpenOrCreate);

                serializer.Serialize(fs, data);
                //Config Datei schliessen        
                fs.Flush();
                fs.Close();
              }
              catch
              {
             MessageBox.Show(string.Format("Fehler beim Schreiben der XML-Daten {0}", filename));
             }
        }

        /// <summary>
        /// laden der Comos XML
        /// </summary>
        /// <returns>class MSR Rückgabe aller Gerätedaten</returns>
        private MSR xmlComosLoad(string comosXmlFilename)
        {
            MSR outData = new MSR();

            // Prüfen ob Dateien existiert
            if (string.IsNullOrEmpty(comosXmlFilename) || !File.Exists(comosXmlFilename))
                MessageBox.Show("Es wurde keine Datei gewählt");
            else
            {
                this.Title = "COMOS Datendatei wird eingelesen";
                //Datei laden             
                XmlSerializer serializer = new XmlSerializer(typeof(MSR));
                FileStream fs = new FileStream(comosXmlFilename, FileMode.Open);
                outData = (MSR)serializer.Deserialize(fs);

                //filestream schliesen
                fs.Close();
            }
            return outData;
        }

        private string extractRevData (string filename)
        {
            string temp = "";

            int start = filename.IndexOf("_R");
            int end = filename.IndexOf("_20");
            if ((start >= 1) && (end >= 1))
            {
             //   MessageBox.Show(String.Format("Start:  {0} Ende: {1} ", start, end));
                temp = filename.Substring(start + 1, end - start - 1) +"_"+ filename.Substring(end + 1,8);
            }
            return temp;
        }

    }


}