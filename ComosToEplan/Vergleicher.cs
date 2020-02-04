
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace ComosToEplan
{

    /// <summary>
    /// Funktionen für den Vergleich von Daten in XML 
    /// </summary>
    public partial class MainWindow

    {

        /// <summary>
        /// vergleicht Eplan Inhaltsverzeichnis mit EPC Geräte Liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnXmlCompareMerge_click(object sender, RoutedEventArgs e)
        {
            // Merge
            // Datendatei EPC
            string comosFile = tbopenComosContent.Text;
            string eplanFile = tbopenEplanContent.Text;

            // Prüfen ob Dateien existiert
            if (string.IsNullOrEmpty(comosFile) || !File.Exists(comosFile))
                MessageBox.Show("Es wurde keine Datei gewählt");
            else if  (string.IsNullOrEmpty(eplanFile) || !File.Exists(eplanFile))
                MessageBox.Show("Es wurde kein Eplan Inhaltsverzeichnis gewählt");
            else
            {

                this.Title = "Start Eplan gegen COMOS Daten Vergleich";
                IEnumerable<ClassMSREPC> comosData = CompareEplanComos(comosFile);
                IEnumerable<ClassMSREPC> eplanContent = CompareEplanComos(eplanFile);
                TagComparer oComparer = new TagComparer();
                var comparerListInEplan = eplanContent.Intersect(comosData, oComparer).ToList();

                // Vergleicher für fehlende TAG-Nummern im COMOS
                var comparerListTagOnlyInEplan = eplanContent.Except(comosData, oComparer).ToList();

                // Vergleicher für fehlende TAG-Nummern im COMOS
                var comparerListComosNotInEplan = comosData.Except(eplanContent, oComparer).ToList();
                
                // hier Ausgabe aller TAG-Nummern in eine einfache XML Liste, welche in Eplan drin sind    
                string mergeXmlDirectoy = outputDirectory + @"Ausgabe\";
                string equilXmlFile = (string.Format(mergeXmlDirectoy + @"\STATUS_MSR_in_Eplan_{0}{1}{2}.xml", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));

                if (!Directory.Exists(mergeXmlDirectoy))
                {
                    Directory.CreateDirectory(mergeXmlDirectoy);
                }

                writeXmlTagListClassMSREPC(equilXmlFile, comparerListInEplan);

                // hier Ausgabe aller TAG-Nummern in eine einfache XML Liste, welche in Eplan  nicht drin sind                  
                string openXmlFile = (string.Format(mergeXmlDirectoy + @"\STATUS_MSR_fehlt_in_Comos_{0}{1}{2}.xml", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
                writeXmlTagListClassMSREPC(openXmlFile, comparerListTagOnlyInEplan);

                // hier Ausgabe aller TAG-Nummern in eine einfache XML Liste, welche in Eplan  nicht drin sind                  
                string exportXmlFile = (string.Format(mergeXmlDirectoy + @"\STATUS_MSR_fehlt_in_Eplan_{0}{1}{2}.xml", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
                 writeXmlTagListClassMSREPC(exportXmlFile, comparerListComosNotInEplan);
                //writeXml(exportXmlFile, typeof(ClassMSREPC), comparerListComosNotInEplan);

                // XML Datei Kopieren und Status Eplan einschreiben
                try
                    {
                        XDocument outDoc = XDocument.Load(comosFile);

                        //Rücksetzen von Eplan_Data
                        foreach (var item in (from item in outDoc.Descendants("Geräte_Bezeichnung")
                                              where item.FirstNode.ToString() != null

                                              select item).ToList())
                        {
                            item.Parent.Descendants("EPLAN_DATA").Elements("Status_Erstellt").First().Value = "0";
                            item.Parent.Descendants("EPLAN_DATA").Elements("Erstellungsdatum").First().Value = "0";
                            item.Parent.Descendants("EPLAN_DATA").Elements("Makroname").First().Value = "0";
                            item.Parent.Descendants("EPLAN_DATA").Elements("Bemerkungen").First().Value = "0";
                        }
                        // für jedes Ergebnis ein Suchlauf
                        // unsauber aber funktioniert
                        // tempCompare MSRMSR_GerätEPLAN_DATA
                        // string tempCompare;
                        // string tempOutErstellDatum;

                        for (int i = 0; i < comparerListInEplan.Count; i++)
                        {
                            //  tempCompare = comparerListInEplan[i].TAG;
                            //  tempOutErstellDatum = comparerListInEplan[i].Erstellungsdatum;
                            //string 

                            foreach (var item in (from item in outDoc.Descendants("Geräte_Bezeichnung")
                                                  where item.FirstNode.ToString() == comparerListInEplan[i].TAG//tempCompare
                                                  select item).ToList())
                            {
                                //Ändern des Elementes unterhalb des Knotens EPLAN_DATA
                                //Navigieren nach Oben und dann tiefer
                                item.Parent.Descendants("EPLAN_DATA").Elements("Status_Erstellt").First().Value = "1";
                                item.Parent.Descendants("EPLAN_DATA").Elements("Erstellungsdatum").First().Value = comparerListInEplan[i].Erstellungsdatum;
                                item.Parent.Descendants("EPLAN_DATA").Elements("Makroname").First().Value = comparerListInEplan[i].Makroname;
                                item.Parent.Descendants("EPLAN_DATA").Elements("Bemerkungen").First().Value = comparerListInEplan[i].Bemerkungen;
                            }
                        }
                        //schreiben der Ausgabe in selbe Comos Datei
                      //  string mergeXmlFile = (string.Format(mergeXmlDirectoy + "Vergleich_EPC_Eplan_{0}{1}{2}.xml", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
                        outDoc.Save(comosFile);

                    //EplanDATA einfügen
                    this.Title = "Daten Vergleich ist fertig";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(null, ex.Message, "Fehler");
                }
            }
        }
 

        /// <summary>
        /// Ausgabe in XML Datei als einfach TAG Liste
        /// </summary>
        /// <param name="outputXmlFile"></param>
        /// <param name="comparerList"></param>
        private void writeXmlTagListClassMSREPC(string outputXmlFile, List<ClassMSREPC> comparerList)
        {
            // hier Ausgabe in einfache XML Liste
            XElement xmlEquil = new XElement("MSR",

                from xmlList in comparerList
                select new XElement("MSR_Gerät",
                new XAttribute("TAG", xmlList.TAG.ToString())
                //  xmlList.
                )
                );
            xmlEquil.Save(outputXmlFile);
            System.Diagnostics.Process.Start(outputXmlFile);
        }

        private void writeXmlTagList(string outputXmlFile, List<string> comparerList)
        {
            // hier Ausgabe in einfache XML Liste
            XElement xmlEquil = new XElement("MSR",

                from xmlList in comparerList
                select new XElement("MSR_Gerät",
                new XAttribute("TAG", xmlList.ToString())
                //  xmlList.
                )
                );
            xmlEquil.Save(outputXmlFile);
            System.Diagnostics.Process.Start(outputXmlFile);
        }

        // Vergleicher 
        IEnumerable<ClassMSREPC> CompareEplanComos(string filename)
        {
            XElement xml = XElement.Load((filename));
            
               // ComosDaten
                var query = (from book in xml.Descendants("MSR_Gerät")
                             orderby (string)book.Attribute("TAG") ascending
                             select new ClassMSREPC(
                                   (string)book.Attribute("TAG") != null ? (string)book.Attribute("TAG") : string.Empty,
                                  (string)(book.Element("Änderungsdatum")) != null ? (string)(book.Element("Änderungsdatum")) : string.Empty,
                                 (string)book.Descendants("EPLAN_DATA").Elements("Makroname").First() != null ? (string)book.Descendants("EPLAN_DATA").Elements("Makroname").First() : string.Empty,                                                             
                                 (string)(book.Descendants("Bemerkungen").ToString()) != null ? (string)(book.Descendants("Bemerkungen").First()) : string.Empty
                                  )
               );
                return query;          
        }

        // ab hier Vergleich COMOS

        /// <summary>
        /// Vergelich Comos Daten Listen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnComosCompare_click(object sender, RoutedEventArgs e)
        {
            this.Title = "Start Vergleich neu gegen alte COMOS Daten";

            MSR comosDataOld = new MSR();
            MSR comosDataNew = new MSR();
            comosDataOld = xmlComosLoad(tbopenComosContent_Old.Text);
            comosDataNew = xmlComosLoad(tbopenComosContent_New.Text);



            // Gleichheit Signale
            var list3 = comosDataNew.MSR_Gerät.Intersect(comosDataOld.MSR_Gerät, new KeyEqualityComparer<MSRMSR_Gerät>(s => s.Signale));

            foreach (var item in list3)
            {
                Console.WriteLine(item.TAG_NAME);
            }

            // Ungleichheit Signale Index 0
            var list4 = comosDataNew.MSR_Gerät.Except(comosDataOld.MSR_Gerät, new KeyEqualityComparer<MSRMSR_Gerät>(s => s.Signale[0].Signal));

            // ungleichheit HW Kartentyp
            var list5 = (comosDataNew.MSR_Gerät.Except(comosDataOld.MSR_Gerät, new KeyEqualityComparer<MSRMSR_Gerät>(s => s.Signale[0].HW_Anbindung.Kartentyp))).ToList();


            foreach (var item in list4)
            {
                //  Console.WriteLine(item.TAG_NAME);
            }

            // Ausgabe aller Datensätze die geänderte Feldgeräte haben  
            //funktioniert
            var vrglResultHW = from x in comosDataNew.MSR_Gerät
                               join y in comosDataOld.MSR_Gerät
                                on new { x.TAG_NAME, x.Hersteller, x.Gerät } equals new { y.TAG_NAME, y.Hersteller, y.Gerät }
                               select x;
            // Ausgabe aller Datensätze die nicht in der vorherigen Liste enthalten waren
           var result = from x in comosDataNew.MSR_Gerät
                         where !(from y in vrglResultHW select y.TAG_NAME).Contains(x.TAG_NAME)
                         select x.TAG_NAME;


            //Ausgabe in Datei
            List<string> dataOutput = new List<string>();
            foreach (var item in result)
                dataOutput.Add(item);

            string outputXMlfile =  (string.Format(outputDirectory + "\\Vergleich_unterschiedliche_Feldgeräte_{0}{1}{2}.xml", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));      
            writeXmlTagList(outputXMlfile, dataOutput);

            // Ausgabe der Daten die in neuer und alter Liste sind
            // Liste aus beiden Datensätzen bauen
            var listTagEqual =
                  from x in comosDataNew.MSR_Gerät
                  join y in comosDataOld.MSR_Gerät              
                  on x.TAG_NAME equals y.TAG_NAME
                  orderby x.TAG_NAME
                  select new MSRMSR_Gerät_Vergleicher {Daten_new = x, Daten_old = y};

            // Ausgabe der TAG-Nummern mit geänderten Datensätzen 
            List<string> dataOutputÄnderung = new List<string>();
            foreach (var item in listTagEqual)
            {
                if ((item.Daten_new.Equals(item.Daten_old))== false)
                    dataOutputÄnderung.Add(item.Daten_new.TAG_NAME);
            }
             outputXMlfile = (string.Format(outputDirectory + "\\Vergleich_unterschiedliche_Datensätze_{0}{1}{2}.xml", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
            writeXmlTagList(outputXMlfile, dataOutputÄnderung);

            // Ausgabe der TAG-Nummern mit geänderter Hardware
            List<string> dataOutputÄnderungHW = new List<string>();
            foreach (var item in listTagEqual)
            {
                for (int i = 0; i < item.Daten_new.Signale.Count(); i++)
                {
                    if (item.Daten_new.Signale[i].Signal != null)
                    {
                        if (item.Daten_old.Signale[i].Signal == null) //wenn es im alten Datensatz nicht das Signal gibt
                        {
                            dataOutputÄnderungHW.Add(item.Daten_new.TAG_NAME);
                            continue;
                        }
                        else if ((item.Daten_new.Signale[i].HW_Anbindung.Equals(item.Daten_old.Signale[i].HW_Anbindung)) == false)
                            dataOutputÄnderungHW.Add(item.Daten_new.TAG_NAME);
                        continue;
                    }
                    else
                        continue;
                }
            }
            outputXMlfile = (string.Format(outputDirectory + "\\Vergleich_unterschiedliche_HW_Anbindung_{0}{1}{2}.xml", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
            writeXmlTagList(outputXMlfile, dataOutputÄnderungHW);



            //Ausgabe der neuen Tag-Nummern
            var listTagNeu =
                from x in comosDataNew.MSR_Gerät
                where !(from y in comosDataOld.MSR_Gerät select y.TAG_NAME).Contains(x.TAG_NAME)
                select x;

            // Ausgabe der TAG-Nummern mit geänderten Datensätzen 
            List<string> dataOutputNeueTag = new List<string>();
            foreach (var item in listTagNeu)
            {                
                    dataOutputNeueTag.Add(item.TAG_NAME);
            }
            outputXMlfile = (string.Format(outputDirectory + "\\Vergleich_neue_Datensätze_{0}{1}{2}.xml", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
            writeXmlTagList(outputXMlfile, dataOutputNeueTag);





            this.Title = "Vergleich ist fertig";
        }
       

        /// <summary>
        /// Vergleicher mit Lambdafunktion
        /// </summary>
        /// <typeparam name="T"></typeparam>

        public class KeyEqualityComparer<T> : IEqualityComparer<T>
        {
            private readonly Func<T, object> keyExtractor;
            /// <summary>
            /// Vergleicher
            /// </summary>
            /// <param name="keyExtractor">Typ Object</param>
            public KeyEqualityComparer(Func<T, object> keyExtractor)
            {
                this.keyExtractor = keyExtractor;
            }
            /// <summary>
            /// Rückgabe Vergeleich
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns>bool Rückgabe Vergeleich</returns>
            public bool Equals(T x, T y)
            {
                return this.keyExtractor(x).Equals(this.keyExtractor(y));
            }
   
            /// <summary>
            /// Hashcode
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int GetHashCode(T obj)
            {
                return this.keyExtractor(obj).GetHashCode();
            }
        }

       

    }

}