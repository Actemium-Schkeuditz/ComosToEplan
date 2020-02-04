using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using System.Windows;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ComosToEplan
{
    /// <summary>
    /// Export Funktionen
    /// </summary>
    public partial class MainWindow
    {
        //Filter für Prozesslinien konfigurieren
        ConfigPL dataPlListe = new ConfigPL();
        string loadFilePLConfig = @"..\..\XML\ConfigPL.xml";

        /// <summary>
        /// Ausgabe der Daten nach Eplan EEC
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnExportToEplan_Click(object sender, RoutedEventArgs e)
        {
            

            // Datei holen
            string loadFile = tbopenComosToEplan.Text;
            // Prüfen ob Dateien existiert
            if (string.IsNullOrEmpty(loadFile) || !File.Exists(loadFile))
                MessageBox.Show("Es wurde keine Datei gewählt");
            else
            {
                //laden der Pumpen Config
                loadPumpenConfig(loadFilePumpenConfig);

                // Datei öffnen und Filtern nach allem was aktuell ist               
                var filterGeräteAktiv =
                    from geräteAktiv in xmlComosLoad(tbopenComosToEplan.Text).MSR_Gerät  
                    where geräteAktiv.Aktiv == 1 && geräteAktiv.Ausbaustufe == 1
                    select geräteAktiv;

                //filtern nach allem was nicht schon in Eplan ist
                if ((bool)(cbEnableFilterInEplan.IsChecked))
                {
                    filterGeräteAktiv =
                        from geräte in filterGeräteAktiv
                        where geräte.EPLAN_DATA.Status_Erstellt != 1
                        select geräte;
                }

                // Filtern nach den Prozesslinien welche ausgewählt wurden
                var gerätePLFilter =                   
                    filterGeräteAktiv
                   .Join(dataPlListe.Config.Where(temp => temp.Aktiv == true),
                    geräte => geräte.PL,
                   tempPl => tempPl.Linie,
                   (a,b) => a );

               


                    // Filtern nach den Geräten welche nicht zu den Druckluftpumpen gehören
                    List<PumpenConfig> sensorDataPump = sensorenAnPumpen(dataPumpenConfig);

                var filterGeräteNotOnPumps =
                    from  geräte in gerätePLFilter
                    where ! (from sens in sensorDataPump select sens.Sensor).Contains( geräte.TAG_NAME)
                    select geräte;
           
//                Console.WriteLine(gerätePLFilter.Count());
  //              Console.WriteLine(filterGeräteNotOnPumps.Count());
                

                //Export für PA Geräte nach Eplan
                //Filtern nach PA Geräten (auch Pt100)               
                var filterPAGeräte =
                    from paGeräte in filterGeräteNotOnPumps// gerätePLFilter         
                    where paGeräte.Signale[0].SignalKonfig.Anbindung == "Bus" && paGeräte.Signale[0].HW_Anbindung.Kartentyp == "SK3"
                    select paGeräte;

                //Filtern nach PA_DI Geräten              
                var filterPAGeräteDI =
                    from paGeräte in filterGeräteNotOnPumps// gerätePLFilter                
                    where paGeräte.Signale[0].SignalKonfig.Anbindung == "Klemme" && paGeräte.Signale[0].HW_Anbindung.Kartentyp == "SK3"
                    select paGeräte;

                // Filtern nach allem was ET200isp ist, aber nicht FS 
                var filterKlemmeGeräteET200isp =
                    from et200ispGeräte in filterGeräteNotOnPumps// gerätePLFilter               
                    where et200ispGeräte.Signale[0].SignalKonfig.Anbindung == "Klemme" && et200ispGeräte.Signale[0].HW_Anbindung.Kartentyp == "ET200iSP"
                    select et200ispGeräte;

                // Filtern nach allem was ET200isp FS ist              
                var filterKlemmeGeräteET200ispFS =
                    from et200ispGeräte in filterGeräteNotOnPumps// gerätePLFilter              
                    where et200ispGeräte.Signale[0].SignalKonfig.Anbindung == "Klemme" && et200ispGeräte.Signale[0].HW_Anbindung.Kartentyp == "ET200isP-F"
                    select et200ispGeräte;
              
                // Filtern nach allem was ET200S ist 
                var filterKlemmeGeräteET200S =
                    from et200sGeräte in gerätePLFilter             
                    where et200sGeräte.Signale[0].SignalKonfig.Anbindung == "Klemme" && et200sGeräte.Signale[0].HW_Anbindung.Kartentyp == "ET200S"
                    select et200sGeräte;
               
                
                // Ausgabe der Daten
                string outputDirectoryEplan = outputDirectory + @"Eplan";

               

                //Prüfen ob es das Ausgabeverzeichnis gibt, wenn nicht anlegen
                try
                {
                    if (!Directory.Exists(outputDirectoryEplan))
                    {
                        Directory.CreateDirectory(outputDirectoryEplan);
                    }
                }
                catch
                {
                    MessageBox.Show("Fehler beim Anlegen des Ausgabeverzeichnis!");
                }
                // Ausgabe der PA-Geräte
                if ((bool)cbEnableOutputPA.IsChecked)
                    XMLwritePA(filterPAGeräte, loadFile, outputDirectoryEplan);

                // Ausgabe der PA-Pt100 Geräte
                if ((bool)cbEnableOutputPt100PA.IsChecked)
                    XMLwritePAPt100(filterPAGeräte, loadFile, outputDirectoryEplan);

                // Ausgabe der PA-DI Geräte
                if ((bool)cbEnableOutputPADi.IsChecked)
                   XMLwritePADi(filterPAGeräteDI, loadFile, outputDirectoryEplan);

                // Ausgabe der nicht FS-Ventile
                if ((bool)cbEnableOutputET200ispYS.IsChecked)
                    XMLWriteValves(filterKlemmeGeräteET200isp, loadFile, outputDirectoryEplan);
                // Vor-Ort-Steuerstellen
                if ((bool)cbEnableOutputHSO.IsChecked)
                    XMLwriteHSO(filterKlemmeGeräteET200S, loadFile, outputDirectoryEplan);

                //Ausgabe 3-Leiter PT100 mit Doppeltemperaturfühler
                if ((bool)cbEnableOutputTRCZ.IsChecked)
                    XMLwriteTRCZ(filterGeräteNotOnPumps, loadFile, outputDirectoryEplan);

                // Ausgabe Handventile
                if ((bool)cbEnableOutputGO.IsChecked)
                XMLWriteET200ValvesGO(filterKlemmeGeräteET200isp, loadFile, outputDirectoryEplan);

                // Ausgabe FS-Ventile
                if ((bool)cbEnableOutputValvesFS.IsChecked)
                    XMLWriteET200FSValves(filterKlemmeGeräteET200ispFS, loadFile, outputDirectoryEplan);

                //Ausgabe FS Analoggeräte
                if ((bool)cbEnableOutputAnaFS.IsChecked)
                    XMLWriteET200FSAna(filterKlemmeGeräteET200ispFS, loadFile, outputDirectoryEplan);

                //Ausgabe der Analoggeräte
                if ((bool)cbEnableOutputAna.IsChecked)
                xmlWriteET200Ana(filterKlemmeGeräteET200isp, loadFile, outputDirectoryEplan);
                // Ausgabe Druckluftpumpen
                if ((bool)cbEnableOutputPumpenDrl.IsChecked)
                xmlWritePumpen(gerätePLFilter, dataPumpenConfig, loadFile, outputDirectoryEplan);


                Console.ReadLine();


                this.Title = "Fertig";
            }
        }

        /// <summary>
        /// Ausgabefunktion für PA-Geräte
        /// </summary>
        /// <param name="filterPAGeräte"></param>
        /// <param name="inputFile"></param>
        /// <param name="outputDirectoryEplan"></param>

        private void XMLwritePA(IEnumerable<MSRMSR_Gerät> filterPAGeräte, string inputFile, string outputDirectoryEplan)
        {

            // filtern nach den Geräten welche echte PA-Geräte sind
            var dataWrite =
                 from paAiGerät in filterPAGeräte
                 where paAiGerät.Signale[0].HW_Anbindung.SlaveTyp == "R4D0-FB-IA10" || paAiGerät.Signale[0].HW_Anbindung.SlaveTyp == "F2D0-FB-Ex4"
                 select paAiGerät;

            string bmkKasten;
            string bmkGerät;
            string hwTypical;
            string anschlussPlus;
            string anschlussMinus;
            // ermitteln der ursprünglichen Comos Export Datei
            string arbeitsStandComosDB = extractRevData(inputFile);//inputFile.Substring((inputFile.Length > 0 ? inputFile.Length - 19 : 0), 6);

            string outputFile = (string.Format(outputDirectoryEplan + "\\PA_{0}{1}{2}.xml", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));

            // Aufruf Hilfsfunktion zum Laden der PA Config Daten
            loadPaConfig();
            // Aufruf Hilfsfunktion zum Laden der Geräte Config Daten       
            loadGeräteConfig();

            string schemaXSD = "DatenExportPA.xsd";
            const string ns = "http://www.w3.org/2001/XMLSchema-instance";
            this.Title = "XML-Datei wird geschrieben.";
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  "; // 2 Leerzeichen
            XmlWriter writer = XmlWriter.Create(@outputFile, settings);
            writer.WriteStartDocument();

            this.Title = "Eplan Daten XML werden exportiert";

            //XML schreiben
            // Starttag des Stammelements
            writer.WriteStartElement("EEC_PA");
            writer.WriteAttributeString("xmlns", "xsi", "", ns);
            writer.WriteAttributeString("xsi", "noNamespaceSchemaLocation",
                  ns, schemaXSD);
            writer.WriteComment(String.Format("Diese Datei wurde aus {0} mit XmlWriter am {1} erzeugt", inputFile, DateTime.Now));

            {
                try
                {
                    foreach (var item in dataWrite)
                    {
                        // BMK des Kastens ermitteln
                        bmkKasten = bmkPAKaesten(item.PL, item.Signale[0].HW_Anbindung.Busstrang, item.Signale[0].HW_Anbindung.Profibus_PA.SK3, item.Signale[0].HW_Anbindung.Profibus_PA.Trunk);
                        // BMK der Geräte im PA-Kasten ermitteln
                        bmkGerät = bmkFeldbarieren(bmkKasten, item.Signale[0].HW_Anbindung.Profibus_PA.Feldbarriere, item.Signale[0].HW_Anbindung.SlaveTyp);
                        // Geräte Anschlüsse ermitteln
                        anschlussPlus = anschlussGeräte(item.Hersteller, item.Gerät, "Plus");
                        anschlussMinus = anschlussGeräte(item.Hersteller, item.Gerät, "Minus");
                        // ermitteln des HW-Typicals
                        if ((item.Funktion == "YC") || (item.Funktion == "YS") || (item.Funktion == "YV"))
                            hwTypical = @"@PROJEKT\PCH02\PA\PA_YC";
                        else
                            hwTypical = @"@PROJEKT\PCH02\PA\PA";

                        // Daten schreiben
                        //EEC Seitendaten
                        writer.WriteStartElement("TYPICAL_DATA");
                        writer.WriteElementString("HW_Typical", hwTypical);
                        writer.WriteElementString("Functional_assignment", string.Empty);
                        writer.WriteElementString("Higher_level_function", "PL" + item.PL);
                        writer.WriteElementString("Intallation_site", string.Empty);
                        writer.WriteElementString("Mounting_location", item.Ebene);
                        writer.WriteElementString("Higher_level_function_number", string.Empty);
                        writer.WriteElementString("Document_type", string.Empty);
                        writer.WriteElementString("User_defined", item.Funktion.Substring(0, 1));
                        writer.WriteElementString("Page_name", item.TAG_NAME.Substring((item.TAG_NAME.Length > 0 ? item.TAG_NAME.Length - 3 : 0), 3));
                        writer.WriteElementString("Representation_Type", string.Empty);
                        writer.WriteElementString("Variant", string.Empty);
                        writer.WriteElementString("erzeugen", item.EPLAN_DATA.Status_Erstellt == 0 ? string.Empty : "!"); // Wenn schon erzeugt dann, EEC anweisen diese Seite auslassen 
                        writer.WriteElementString("Seitenbeschreibung", item.TAG_NAME + "¶" + item.Bezeichnung);
                        //Typical Daten Seite
                        writer.WriteElementString("Funktionstext", item.TAG_NAME + "¶" + item.Bezeichnung);
                        writer.WriteElementString("DB_Stand", arbeitsStandComosDB); //"R" + item.Signale[0].Revision.RevEPC.ToString() + "." + item.Signale[0].Revision.RevPMS.ToString() + "." + item.Signale[0].Revision.RevACT.ToString());
                        writer.WriteElementString("Messstelle", item.TAG_NAME);
                        writer.WriteElementString("Messstelle_Kabel", item.Geräte_Bezeichnung);
                        // Typical Daten
                        writer.WriteElementString("BMK_RIO", bmkKasten);
                        writer.WriteElementString("Feldbarriere", bmkGerät);
                        writer.WriteElementString("Spur_NR", item.Signale[0].HW_Anbindung.Steckplatz);
                        writer.WriteElementString("Signalname_AI", item.Signale[0].Signal);
                        writer.WriteElementString("Adresse_AI", item.Signale[0].HW_Anbindung.HW_Adresse);
                        writer.WriteElementString("Typ_Gerät", item.Hersteller + "¶" + item.Gerät + "¶" + item.Geräte_Beschreibung);
                        writer.WriteElementString("Gerät_Funktion", item.Funktion.Substring(0, 1));
                        writer.WriteElementString("Gerät_Anschluss_Plus", anschlussPlus);
                        writer.WriteElementString("Gerät_Anschluss_Minus", anschlussMinus);
                        writer.WriteElementString("AS", "AS" + item.Signale[0].AS.Substring(4, 1));
                        writer.WriteElementString("DP_Adresse", item.Signale[0].HW_Anbindung.PB_Adresse);
                        writer.WriteElementString("DP_Strang", item.Signale[0].HW_Anbindung.Busstrang + Convert.ToString(item.Signale[0].HW_Anbindung.Profibus_PA.SK3) + Convert.ToString(item.Signale[0].HW_Anbindung.Profibus_PA.Trunk));
                        writer.WriteEndElement();// end
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Es ist ein Fehler beim XML erstellen aufgetreten!");
                }

                // Endtag des Stammelements
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
                MessageBox.Show(String.Format(@"Datei {0} erzeugt.", outputFile));
                System.Diagnostics.Process.Start(outputFile);
            }
        }

        /// <summary>
        /// Ausgabefunktion für Pt100 PA-Geräte 
        /// </summary>
        /// <param name="filterPAGeräte"></param>
        /// <param name="inputFile"></param>
        /// <param name="outputDirectoryEplan"></param>

        private void XMLwritePAPt100(IEnumerable<MSRMSR_Gerät> filterPAGeräte, string inputFile, string outputDirectoryEplan)
        {

            // filtern nach den Geräten welche Pt100 an PA sind
            var dataWrite =
                 from paAiGerät in filterPAGeräte
                 where paAiGerät.Signale[0].HW_Anbindung.SlaveTyp == "RD0-TI-EX8.PA" && paAiGerät.Funktion != "TRCZ"
                 select paAiGerät;

            string bmkKasten;
            string bmkGerät;
            string hwTypical;
            // string anschlussPlus;
            // string anschlussMinus;
            // ermitteln der ursprünglichen Comos Export Datei
            string arbeitsStandComosDB = extractRevData(inputFile);// inputFile.Substring((inputFile.Length > 0 ? inputFile.Length - 19 : 0), 6);
            string outputFile = (string.Format(outputDirectoryEplan + "\\PAPt100_{0}{1}{2}.xml", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
            // Aufruf Hilfsfunktion zum Laden der PA Config Daten
            loadPaConfig();
            // Aufruf Hilfsfunktion zum Laden der Geräte Config Daten       
            loadGeräteConfig();

            string schemaXSD = "DatenExportPAPt100.xsd";
            const string ns = "http://www.w3.org/2001/XMLSchema-instance";
            this.Title = "XML-Datei wird geschrieben.";
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  "; // 2 Leerzeichen
            XmlWriter writer = XmlWriter.Create(@outputFile, settings);
            writer.WriteStartDocument();

            this.Title = "Eplan Daten XML werden exportiert";

            //XML schreiben
            // Starttag des Stammelements
            writer.WriteStartElement("EEC_PAPt100");
            writer.WriteAttributeString("xmlns", "xsi", "", ns);
            writer.WriteAttributeString("xsi", "noNamespaceSchemaLocation",
                  ns, schemaXSD);
            writer.WriteComment(String.Format("Diese Datei wurde aus {0} mit XmlWriter am {1} erzeugt", inputFile, DateTime.Now));

            {
                try
                {
                    foreach (var item in dataWrite)
                    {
                        // BMK des Kastens ermitteln
                        bmkKasten = bmkPAKaesten(item.PL, item.Signale[0].HW_Anbindung.Busstrang, item.Signale[0].HW_Anbindung.Profibus_PA.SK3, item.Signale[0].HW_Anbindung.Profibus_PA.Trunk);
                        // BMK der Geräte im PA-Kasten ermitteln
                        bmkGerät = bmkFeldbarieren(bmkKasten, item.Signale[0].HW_Anbindung.Profibus_PA.Feldbarriere, item.Signale[0].HW_Anbindung.SlaveTyp);
                        // Geräte Anschlüsse ermitteln
                        // anschlussPlus = anschlussGeräte(item.Hersteller, item.Gerät, "Plus");
                        //anschlussMinus = anschlussGeräte(item.Hersteller, item.Gerät, "Minus");
                        // ermitteln des HW-Typicals
                        hwTypical = @"@PROJEKT\PCH02\PA\3L_RTD_PA";

                        // Daten schreiben
                        //EEC Seitendaten
                        writer.WriteStartElement("TYPICAL_DATA");
                        writer.WriteElementString("HW_Typical", hwTypical);
                        writer.WriteElementString("Functional_assignment", string.Empty);
                        writer.WriteElementString("Higher_level_function", "PL" + item.PL);
                        writer.WriteElementString("Intallation_site", string.Empty);
                        writer.WriteElementString("Mounting_location", item.Ebene);
                        writer.WriteElementString("Higher_level_function_number", string.Empty);
                        writer.WriteElementString("Document_type", string.Empty);
                        writer.WriteElementString("User_defined", item.Funktion.Substring(0, 1));
                        writer.WriteElementString("Page_name", item.TAG_NAME.Substring((item.TAG_NAME.Length > 0 ? item.TAG_NAME.Length - 3 : 0), 3));
                        writer.WriteElementString("Representation_Type", string.Empty);
                        writer.WriteElementString("Variant", string.Empty);
                        writer.WriteElementString("erzeugen", item.EPLAN_DATA.Status_Erstellt == 0 ? string.Empty : "!"); // Wenn schon erzeugt dann, EEC anweisen diese Seite auslassen 
                        writer.WriteElementString("Seitenbeschreibung", item.TAG_NAME + "¶" + item.Bezeichnung);
                        //Typical Daten Seite
                        writer.WriteElementString("Funktionstext", item.TAG_NAME + "¶" + item.Bezeichnung);
                        writer.WriteElementString("DB_Stand", arbeitsStandComosDB); //"R" + item.Signale[0].Revision.RevEPC.ToString() + "." + item.Signale[0].Revision.RevPMS.ToString() + "." + item.Signale[0].Revision.RevACT.ToString());
                        writer.WriteElementString("Messstelle", item.TAG_NAME);
                        writer.WriteElementString("Messstelle_Kabel", item.Geräte_Bezeichnung);
                        // Typical Daten
                        writer.WriteElementString("Typ_Gerät", item.Hersteller + "¶" + item.Gerät + "¶" + item.Geräte_Beschreibung);
                        writer.WriteElementString("Gerät_Funktion", item.Funktion.Substring(0, 1));
                        writer.WriteElementString("Signalname_AI", item.Signale[0].Signal);
                        writer.WriteElementString("BMK_PA", bmkKasten);
                        writer.WriteElementString("BMK_TI", bmkGerät);
                        writer.WriteElementString("Kanal", item.Signale[0].HW_Anbindung.Kanal);
                        writer.WriteElementString("Adresse_AI", item.Signale[0].HW_Anbindung.HW_Adresse);
                       // writer.WriteElementString("Spur_NR", item.Signale[0].HW_Anbindung.Steckplatz);
                        
                        //writer.WriteElementString("Gerät_Anschluss_Plus", anschlussPlus);
                        //writer.WriteElementString("Gerät_Anschluss_Minus", anschlussMinus);
                        //writer.WriteElementString("AS", "AS" + item.Signale[0].AS.Substring(4, 1));
                        //writer.WriteElementString("DP_Adresse", item.Signale[0].HW_Anbindung.PB_Adresse);
                        //writer.WriteElementString("DP_Strang", item.Signale[0].HW_Anbindung.Busstrang + Convert.ToString(item.Signale[0].HW_Anbindung.Profibus_PA.SK3) + Convert.ToString(item.Signale[0].HW_Anbindung.Profibus_PA.Trunk));
                        writer.WriteEndElement();// end
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Es ist ein Fehler beim XML erstellen für die PA-Pt100 Geräte aufgetreten!");
                }

                // Endtag des Stammelements
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
                MessageBox.Show(String.Format(@"Datei {0} erzeugt.", outputFile));
                System.Diagnostics.Process.Start(outputFile);
            }
        }


        /// <summary>
        /// Ausgabe der digitalen Geräte am PA-Multiinput
        /// </summary>
        /// <param name="filterPAGeräte"></param>
        /// <param name="inputFile"></param>
        /// <param name="outputDirectoryEplan"></param>

        private void XMLwritePADi(IEnumerable<MSRMSR_Gerät> filterPAGeräte, string inputFile, string outputDirectoryEplan)
        {

            // filtern nach den Geräten welche am PA-Multiinput angeschlossen sind
            var dataWrite =
                 from paAiGerät in filterPAGeräte
                 where paAiGerät.Signale[0].HW_Anbindung.SlaveTyp == "R8D0-MIO-Ex12.PA"
                 select paAiGerät;

            string bmkKasten;
            string bmkGerät;
            string hwTypical;
            string anschlussPlus;
            string anschlussMinus;
            string kanal0;
            string kanal1;
            bool zweitesSignal = false;
            // ermitteln der ursprünglichen Comos Export Datei
            string arbeitsStandComosDB = extractRevData(inputFile);//inputFile.Substring((inputFile.Length > 0 ? inputFile.Length - 19 : 0), 6);
            string outputFile = (string.Format(outputDirectoryEplan + "\\PA_DI_{0}{1}{2}.xml", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
            // Aufruf Hilfsfunktion zum Laden der PA Config Daten
            loadPaConfig();
            // Aufruf Hilfsfunktion zum Laden der Geräte Config Daten       
            loadGeräteConfig();

            string schemaXSD = "DatenExportPA_DI.xsd";
            const string ns = "http://www.w3.org/2001/XMLSchema-instance";
            this.Title = "XML-Datei wird geschrieben.";
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  "; // 2 Leerzeichen
            XmlWriter writer = XmlWriter.Create(@outputFile, settings);
            writer.WriteStartDocument();

            this.Title = "Eplan Daten XML werden exportiert";

            //XML schreiben
            // Starttag des Stammelements
            writer.WriteStartElement("EEC_PA_DI");
            writer.WriteAttributeString("xmlns", "xsi", "", ns);
            writer.WriteAttributeString("xsi", "noNamespaceSchemaLocation",
                  ns, schemaXSD);
            writer.WriteComment(String.Format("Diese Datei wurde aus {0} mit XmlWriter am {1} erzeugt", inputFile, DateTime.Now));

            {
                try
                {
                    foreach (var item in dataWrite)
                    {
                        // BMK des Kastens ermitteln
                        bmkKasten = bmkPAKaesten(item.PL, item.Signale[0].HW_Anbindung.Busstrang, item.Signale[0].HW_Anbindung.Profibus_PA.SK3, item.Signale[0].HW_Anbindung.Profibus_PA.Trunk);
                        // BMK der Geräte im PA-Kasten ermitteln
                        bmkGerät = bmkFeldbarieren(bmkKasten, item.Signale[0].HW_Anbindung.Profibus_PA.Feldbarriere, item.Signale[0].HW_Anbindung.SlaveTyp);
                        // Geräte Anschlüsse ermitteln
                        anschlussPlus = anschlussGeräte(item.Hersteller, item.Gerät, "Plus");
                        anschlussMinus = anschlussGeräte(item.Hersteller, item.Gerät, "Minus");
                        //ermitteln des HW-Typicals, nach Kanal
                        kanal0 = item.Signale[0].HW_Anbindung.Kanal;

                        // ermitteln ob es ein Gerät mit zwei Signalen gibt
                        if (item.Signale.Count() == 2)
                        {
                            zweitesSignal = true;
                            kanal1 = item.Signale[1].HW_Anbindung.Kanal;
                            Console.WriteLine(kanal1);
                        }
                        else
                        {
                            zweitesSignal = false;
                            kanal1 = "99";
                        }

                        // HW Typical ermitteln
                        if (((kanal0 == "1") || (kanal0 == "2") || (kanal0 == "3") || (kanal0 == "4")) && !zweitesSignal)
                        {
                            hwTypical = @"@PROJEKT\PCH02\PA\FDO_BI_1_DI_KONTAKT";
                        }
                        else if (((kanal0 == "6") || (kanal0 == "8") || (kanal0 == "10") || (kanal0 == "12")) && !zweitesSignal)
                        {
                            hwTypical = @"@PROJEKT\PCH02\PA\FDO_BI_1_DI_KONTAKT";
                        }
                        else if (((kanal0 == "5") || (kanal0 == "7") || (kanal0 == "9") || (kanal0 == "11")) && !zweitesSignal)
                        {
                            hwTypical = @"@PROJEKT\PCH02\PA\FDO_BI_1_DI_KONTAKT";
                        }
                        else if (zweitesSignal == true)
                        {
                            hwTypical = @"@PROJEKT\PCH02\PA\FDO_BI_2_DI_KONTAKT_Variante_1";
                        }
                        else
                            hwTypical = "Fehler";
                        // Daten schreiben
                        //EEC Seitendaten
                        writer.WriteStartElement("TYPICAL_DATA");
                        writer.WriteElementString("HW_Typical", hwTypical);
                        writer.WriteElementString("Functional_assignment", string.Empty);
                        writer.WriteElementString("Higher_level_function", "PL" + item.PL);
                        writer.WriteElementString("Intallation_site", string.Empty);
                        writer.WriteElementString("Mounting_location", item.Ebene);
                        writer.WriteElementString("Higher_level_function_number", string.Empty);
                        writer.WriteElementString("Document_type", string.Empty);
                        writer.WriteElementString("User_defined", item.Funktion.Substring(0, 1));
                        writer.WriteElementString("Page_name", item.TAG_NAME.Substring((item.TAG_NAME.Length > 0 ? item.TAG_NAME.Length - 3 : 0), 3));
                        writer.WriteElementString("Representation_Type", string.Empty);
                        writer.WriteElementString("Variant", string.Empty);
                        writer.WriteElementString("erzeugen", item.EPLAN_DATA.Status_Erstellt == 0 ? string.Empty : "!"); // Wenn schon erzeugt dann, EEC anweisen diese Seite auslassen 
                        writer.WriteElementString("Seitenbeschreibung", item.TAG_NAME + "¶" + item.Bezeichnung);
                        //Typical Daten Seite
                        writer.WriteElementString("Funktionstext", item.TAG_NAME + "¶" + item.Bezeichnung);
                        writer.WriteElementString("DB_Stand", arbeitsStandComosDB); //"R" + item.Signale[0].Revision.RevEPC.ToString() + "." + item.Signale[0].Revision.RevPMS.ToString() + "." + item.Signale[0].Revision.RevACT.ToString());
                        writer.WriteElementString("Messstelle", item.TAG_NAME);
                        writer.WriteElementString("BMK_RIO", bmkKasten);
                        writer.WriteElementString("BMK_TI", bmkGerät);
                        writer.WriteElementString("Signalname_GOH", item.Signale[0].Signal);
                        writer.WriteElementString("Adresse_GOH", item.Signale[0].HW_Anbindung.HW_Adresse);
                        writer.WriteElementString("Gerät_Anschluss_Minus", anschlussMinus);
                        writer.WriteElementString("Gerät_Anschluss_Plus", anschlussPlus);
                        writer.WriteElementString("Typ_Gerät", item.Hersteller + "¶" + item.Gerät + "¶" + item.Geräte_Beschreibung);
                        writer.WriteElementString("Messstelle_Kabel", item.Geräte_Bezeichnung);
                        // Typical Daten
                        
                       
                        // writer.WriteElementString("Spur_NR", item.Signale[0].HW_Anbindung.Steckplatz);
                        writer.WriteElementString("Kanal0", kanal0);
                        if (zweitesSignal == true)
                            writer.WriteElementString("Kanal1", kanal1);
                        
                      
                        writer.WriteElementString("Gerät_Funktion", item.Funktion.Substring(0, 1));
                        
                      
                        //writer.WriteElementString("AS", "AS" + item.Signale[0].AS.Substring(4, 1));
                        //writer.WriteElementString("DP_Adresse", item.Signale[0].HW_Anbindung.PB_Adresse);
                        //writer.WriteElementString("DP_Strang", item.Signale[0].HW_Anbindung.Busstrang + Convert.ToString(item.Signale[0].HW_Anbindung.Profibus_PA.SK3) + Convert.ToString(item.Signale[0].HW_Anbindung.Profibus_PA.Trunk));
                        writer.WriteEndElement();// end
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Es ist ein Fehler beim XML erstellen für die PA-DI Geräte aufgetreten!");
                }

                // Endtag des Stammelements
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
                MessageBox.Show(String.Format(@"Datei {0} erzeugt.", outputFile));
                System.Diagnostics.Process.Start(outputFile);
            }
        }

        /// <summary>
        /// Ausgabe der Daten für die Ventile
        /// </summary>
        /// <param name="filterKlemmeGeräte"></param>
        /// <param name="inputFile"></param>
        /// <param name="outputDirectoryEplan"></param>
        private void XMLWriteValves(IEnumerable<MSRMSR_Gerät> filterKlemmeGeräte, string inputFile, string outputDirectoryEplan)
        {
            // filtern nach den Ventilen YS und YZ (YZ weil es welche gibt die nicht mehr Fs sind)
            var dataWrite =
                 from Geräte in filterKlemmeGeräte
                 where Geräte.Funktion == "YS" || Geräte.Funktion == "YZ"
                 select Geräte;

            string bmkKasten;
            string bmkRioLuft;
            string bmkRioLuftZu;
            string hwTypical;
            bool hatZweiEndlagen = false;
            bool hatZweiMagnetventile = false;
            bool zweiMagnetventileZweiEndlagen = false;

            // ermitteln der ursprünglichen Comos Export Datei
            string arbeitsStandComosDB = extractRevData(inputFile);//inputFile.Substring((inputFile.Length > 0 ? inputFile.Length - 19 : 0), 6);
            string outputFile = (string.Format(outputDirectoryEplan + "\\ET200isp_Valves_{0}{1}{2}.xml", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
            // Aufruf Hilfsfunktion zum Laden der PA Config Daten
            //loadPaConfig();
            // Aufruf Hilfsfunktion zum Laden der Geräte Config Daten       
            loadGeräteConfig();
            loadRioConfig();

            string schemaXSD = "DatenExportET200ispValves.xsd";
            const string ns = "http://www.w3.org/2001/XMLSchema-instance";
            this.Title = "Eplan XML-Datei für Ventile wird geschrieben.";
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  "; // 2 Leerzeichen
            XmlWriter writer = XmlWriter.Create(@outputFile, settings);
            writer.WriteStartDocument();

            //XML schreiben
            // Starttag des Stammelements
            writer.WriteStartElement("EEC_ET200ISP_Valve");
            writer.WriteAttributeString("xmlns", "xsi", "", ns);
            writer.WriteAttributeString("xsi", "noNamespaceSchemaLocation",
                  ns, schemaXSD);
            writer.WriteComment(String.Format("Diese Datei wurde aus {0} mit XmlWriter am {1} erzeugt", inputFile, DateTime.Now));

            {
             //     try
              //    {
                foreach (var item in dataWrite)
                    {
                    // ermitteln ob Ventil mit Endlagen
                    if (item.Signale[0].SignalKonfig.Signalerweiterung == "NN")
                    {
                        hatZweiEndlagen = true;
                        if (item.Signale.Length == 4)   //prüfen ob zwei Endlagen + 2MV
                        {
                            zweiMagnetventileZweiEndlagen = true;
                        }
                        else
                        {
                            zweiMagnetventileZweiEndlagen = false;
                        }
                    }
                    else
                    {
                        hatZweiEndlagen = false;
                        zweiMagnetventileZweiEndlagen = false;
                    }

                    // ermitteln ob Ventil mit zwei Ansteuerungen ohne Endlagen
                    if (item.Signale[0].SignalKonfig.Signalerweiterung == "TY")
                    {
                        hatZweiMagnetventile = true;
                    }
                    else
                    {
                        hatZweiMagnetventile = false;
                    }

                        // BMK des Kastens ermitteln                        
                        bmkKasten = bmkRio(item.Signale[0].AS ,item.PL, item.Signale[0].HW_Anbindung.IO_Station);
                    if (hatZweiEndlagen == true)
                    {
                        if (zweiMagnetventileZweiEndlagen == true)
                        {
                            bmkRioLuft = bmkRioLuftAnschluss(bmkKasten, StringToInt(item.Signale[2].HW_Anbindung.Steckplatz), StringToInt(item.Signale[2].HW_Anbindung.Kanal));
                            bmkRioLuftZu = bmkRioLuftAnschluss(bmkKasten, StringToInt(item.Signale[3].HW_Anbindung.Steckplatz), StringToInt(item.Signale[3].HW_Anbindung.Kanal));
                        }
                        else
                        {
                            bmkRioLuft = bmkRioLuftAnschluss(bmkKasten, StringToInt(item.Signale[2].HW_Anbindung.Steckplatz), StringToInt(item.Signale[2].HW_Anbindung.Kanal));
                            bmkRioLuftZu = "Fehler";
                        }
                    }
                    else if (hatZweiMagnetventile == true)
                    {
                       
                            bmkRioLuft = bmkRioLuftAnschluss(bmkKasten, StringToInt(item.Signale[0].HW_Anbindung.Steckplatz), StringToInt(item.Signale[0].HW_Anbindung.Kanal));
                            bmkRioLuftZu = bmkRioLuftAnschluss(bmkKasten, StringToInt(item.Signale[1].HW_Anbindung.Steckplatz), StringToInt(item.Signale[1].HW_Anbindung.Kanal));
                        
                        }

                    else 
                    {
                        bmkRioLuft = bmkRioLuftAnschluss(bmkKasten, StringToInt(item.Signale[0].HW_Anbindung.Steckplatz), StringToInt(item.Signale[0].HW_Anbindung.Kanal));
                        bmkRioLuftZu = "Fehler";
                    }
                    // Geräte Anschlüsse ermitteln
                    // anschlussPlus = anschlussGeräte(item.Hersteller, item.Gerät, "Plus");
                    // anschlussMinus = anschlussGeräte(item.Hersteller, item.Gerät, "Minus");

                    if (hatZweiEndlagen == true)
                    {
                        if (zweiMagnetventileZweiEndlagen == true)
                            {
                            hwTypical = @"@PROJEKT\PCH02\ET200ISP\VALVE_2AIR_2RM";
                        }
                        else
                        { 
                        hwTypical = @"@PROJEKT\PCH02\ET200ISP\VALVE_1AIR_2RM";
                    }
                    }

                    else if (hatZweiMagnetventile == true)
                    {
                        hwTypical = @"@PROJEKT\PCH02\ET200ISP\VALVE_2AIR";
                    }
                    else
                    {
                        hwTypical = @"@PROJEKT\PCH02\ET200ISP\VALVE_1AIR";                        
                    }
                    
                        // Daten schreiben
                        //EEC Seitendaten
                        writer.WriteStartElement("TYPICAL_DATA");
                        writer.WriteElementString("HW_Typical", hwTypical);
                        writer.WriteElementString("Functional_assignment", string.Empty);
                        writer.WriteElementString("Higher_level_function", "PL" + item.PL);
                        writer.WriteElementString("Intallation_site", string.Empty);
                        writer.WriteElementString("Mounting_location", item.Ebene);
                        writer.WriteElementString("Higher_level_function_number", string.Empty);
                        writer.WriteElementString("Document_type", string.Empty);
                        writer.WriteElementString("User_defined", item.Funktion.Substring(0, 1));
                        writer.WriteElementString("Page_name", item.TAG_NAME.Substring((item.TAG_NAME.Length > 0 ? item.TAG_NAME.Length - 3 : 0), 3));
                        writer.WriteElementString("Representation_Type", string.Empty);
                        writer.WriteElementString("Variant", string.Empty);
                        writer.WriteElementString("erzeugen", item.EPLAN_DATA.Status_Erstellt == 0 ? string.Empty : "!"); // Wenn schon erzeugt dann, EEC anweisen diese Seite auslassen 
                        writer.WriteElementString("Seitenbeschreibung", item.TAG_NAME + "¶" + item.Bezeichnung);
                        //Typical Daten Seite
                        writer.WriteElementString("Funktionstext", item.TAG_NAME + "¶" + item.Bezeichnung);
                        writer.WriteElementString("DB_Stand", arbeitsStandComosDB); //"R" + item.Signale[0].Revision.RevEPC.ToString() + "." + item.Signale[0].Revision.RevPMS.ToString() + "." + item.Signale[0].Revision.RevACT.ToString());
                        writer.WriteElementString("Messstelle", item.TAG_NAME);
                       // writer.WriteElementString("Messstelle_Kabel", item.Geräte_Bezeichnung);
                        // Typical Daten
                        writer.WriteElementString("BMK_RIO", bmkKasten);
                        writer.WriteElementString("BMK_DO", string.Empty);
                        writer.WriteElementString("Signalname_DO", string.Empty);
                        writer.WriteElementString("Adresse_DO", string.Empty);

                    if (hatZweiEndlagen == true)
                    {
                        if (zweiMagnetventileZweiEndlagen == true)
                        {
                            writer.WriteElementString("BMK_GOH", "11A" + ((StringToInt(item.Signale[0].HW_Anbindung.Steckplatz)) - 1));
                            writer.WriteElementString("Signalname_GOH", item.Signale[0].Signal);
                            writer.WriteElementString("Adresse_GOH", item.Signale[0].HW_Anbindung.HW_Adresse);
                            writer.WriteElementString("BMK_GOL", "11A" + ((StringToInt(item.Signale[1].HW_Anbindung.Steckplatz)) - 1));
                            writer.WriteElementString("Signalname_GOL", item.Signale[1].Signal);
                            writer.WriteElementString("Adresse_GOL", item.Signale[1].HW_Anbindung.HW_Adresse);
                            writer.WriteElementString("Anschlüsse_GOH-", "4");
                            writer.WriteElementString("Anschlüsse_GOH_Plus", "3");
                            writer.WriteElementString("Anschlüsse_GOL-", "2");
                            writer.WriteElementString("Anschlüsse_GOL_Plus", "1");
                            writer.WriteElementString("Typ_ventil", string.Empty);
                            writer.WriteElementString("Typ_Endlagen", "P+F, SJ3,5-S1N");
                            writer.WriteElementString("Abgang_RIO_LUFT", bmkRioLuft);
                            writer.WriteElementString("ET200isp_DO_Kanal", string.Empty);
                            writer.WriteElementString("ET200_ISP_GOH_Kanal", item.Signale[0].HW_Anbindung.Kanal);
                            writer.WriteElementString("ET200_ISP_GOL_Kanal", item.Signale[1].HW_Anbindung.Kanal);
                            writer.WriteElementString("Abgang_RIO_LUFT_ZU", bmkRioLuftZu);
                        }
                        else
                        {
                            writer.WriteElementString("BMK_GOH", "11A" + ((StringToInt(item.Signale[0].HW_Anbindung.Steckplatz)) - 1));
                            writer.WriteElementString("Signalname_GOH", item.Signale[0].Signal);
                            writer.WriteElementString("Adresse_GOH", item.Signale[0].HW_Anbindung.HW_Adresse);
                            writer.WriteElementString("BMK_GOL", "11A" + ((StringToInt(item.Signale[1].HW_Anbindung.Steckplatz)) - 1));
                            writer.WriteElementString("Signalname_GOL", item.Signale[1].Signal);
                            writer.WriteElementString("Adresse_GOL", item.Signale[1].HW_Anbindung.HW_Adresse);
                            writer.WriteElementString("Anschlüsse_GOH-", "4");
                            writer.WriteElementString("Anschlüsse_GOH_Plus", "3");
                            writer.WriteElementString("Anschlüsse_GOL-", "2");
                            writer.WriteElementString("Anschlüsse_GOL_Plus", "1");
                            writer.WriteElementString("Typ_ventil", string.Empty);
                            writer.WriteElementString("Typ_Endlagen", "P+F, SJ3,5-S1N");
                            writer.WriteElementString("Abgang_RIO_LUFT", bmkRioLuft);
                            writer.WriteElementString("ET200isp_DO_Kanal", string.Empty);
                            writer.WriteElementString("ET200_ISP_GOH_Kanal", item.Signale[0].HW_Anbindung.Kanal);
                            writer.WriteElementString("ET200_ISP_GOL_Kanal", item.Signale[1].HW_Anbindung.Kanal);
                            writer.WriteElementString("Abgang_RIO_LUFT_ZU", string.Empty);
                        }
                    }
                   
                    else if (hatZweiMagnetventile== true)
                    {
                       
                            writer.WriteElementString("BMK_GOH", string.Empty);
                            writer.WriteElementString("Signalname_GOH", string.Empty);
                            writer.WriteElementString("Adresse_GOH", string.Empty);
                            writer.WriteElementString("BMK_GOL", string.Empty);
                            writer.WriteElementString("Signalname_GOL", string.Empty);
                            writer.WriteElementString("Adresse_GOL", string.Empty);
                            writer.WriteElementString("Anschlüsse_GOH-", string.Empty);
                            writer.WriteElementString("Anschlüsse_GOH_Plus", string.Empty);
                            writer.WriteElementString("Anschlüsse_GOL-", string.Empty);
                            writer.WriteElementString("Anschlüsse_GOL_Plus", string.Empty);
                            writer.WriteElementString("Typ_ventil", string.Empty);
                            writer.WriteElementString("Typ_Endlagen", string.Empty);
                            writer.WriteElementString("Abgang_RIO_LUFT", bmkRioLuft);
                            writer.WriteElementString("ET200isp_DO_Kanal", string.Empty);
                            writer.WriteElementString("ET200_ISP_GOH_Kanal", string.Empty);
                            writer.WriteElementString("ET200_ISP_GOL_Kanal", string.Empty);
                            writer.WriteElementString("Abgang_RIO_LUFT_ZU", bmkRioLuftZu);
                        
                    }
                    else 
                    {
                        writer.WriteElementString("BMK_GOH", string.Empty);
                        writer.WriteElementString("Signalname_GOH", string.Empty);
                        writer.WriteElementString("Adresse_GOH", string.Empty);
                        writer.WriteElementString("BMK_GOL", string.Empty);
                        writer.WriteElementString("Signalname_GOL", string.Empty);
                        writer.WriteElementString("Adresse_GOL", string.Empty);
                        writer.WriteElementString("Anschlüsse_GOH-", string.Empty);
                        writer.WriteElementString("Anschlüsse_GOH_Plus", string.Empty);
                        writer.WriteElementString("Anschlüsse_GOL-", string.Empty);
                        writer.WriteElementString("Anschlüsse_GOL_Plus", string.Empty);
                        writer.WriteElementString("Typ_ventil", string.Empty);
                        writer.WriteElementString("Typ_Endlagen", string.Empty);
                        writer.WriteElementString("Abgang_RIO_LUFT", bmkRioLuft);
                        writer.WriteElementString("ET200isp_DO_Kanal", string.Empty);
                        writer.WriteElementString("ET200_ISP_GOH_Kanal", string.Empty);
                        writer.WriteElementString("ET200_ISP_GOL_Kanal", string.Empty);
                        writer.WriteElementString("Abgang_RIO_LUFT_ZU", string.Empty);
                    }
                    // writer.WriteElementString("Typ_Gerät", item.Hersteller + "¶" + item.Gerät + "¶" + item.Geräte_Beschreibung);
                    // writer.WriteElementString("Gerät_Funktion", item.Funktion.Substring(0, 1));
                    //  writer.WriteElementString("Gerät_Anschluss_Plus", anschlussPlus);
                    // writer.WriteElementString("Gerät_Anschluss_Minus", anschlussMinus);

                    //writer.WriteElementString("DP_Adresse", item.Signale[0].HW_Anbindung.PB_Adresse);
                    //writer.WriteElementString("DP_Strang", item.Signale[0].HW_Anbindung.Busstrang + Convert.ToString(item.Signale[0].HW_Anbindung.Profibus_PA.SK3) + Convert.ToString(item.Signale[0].HW_Anbindung.Profibus_PA.Trunk));
                    writer.WriteEndElement();// end
                    }
                //    }
                //  catch (Exception)
                // {
                //MessageBox.Show("Es ist ein Fehler beim XML erstellen für die YS Ventile aufgetreten!");
                // }

                // Endtag des Stammelements
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
                MessageBox.Show(String.Format(@"Datei {0} erzeugt.", outputFile));
                System.Diagnostics.Process.Start(outputFile);
            }

        }

        /// <summary>
        /// Ausgabe Daten für Vor-Ort-Steuerstellen
        /// </summary>
        /// <param name="filterKlemmeGeräte"></param>
        /// <param name="inputFile"></param>
        /// <param name="outputDirectoryEplan"></param>
        private void XMLwriteHSO(IEnumerable<MSRMSR_Gerät> filterKlemmeGeräte, string inputFile, string outputDirectoryEplan)
        {
            // filtern nach den Bedienstellen HSO
            var dataWrite =
                 from Geräte in filterKlemmeGeräte
                 where Geräte.Funktion == "HSO"
                 select Geräte;

            string bmkKasten;
            //string bmkRioLuft;
            string hwTypical;

            // ermitteln der ursprünglichen Comos Export Datei
            string arbeitsStandComosDB = extractRevData(inputFile);//inputFile.Substring((inputFile.Length > 0 ? inputFile.Length - 19 : 0), 6);
            string outputFile = (string.Format(outputDirectoryEplan + "\\HSO_{0}{1}{2}.xml", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
            // Aufruf Hilfsfunktion zum Laden der PA Config Daten
            //loadPaConfig();
            // Aufruf Hilfsfunktion zum Laden der Geräte Config Daten       
            loadGeräteConfig();
            loadRioConfig();

            string schemaXSD = "DatenExportHSO.xsd";
            const string ns = "http://www.w3.org/2001/XMLSchema-instance";
            this.Title = "Eplan XML-Datei für HSO wird geschrieben.";
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  "; // 2 Leerzeichen
            XmlWriter writer = XmlWriter.Create(@outputFile, settings);
            writer.WriteStartDocument();

            //XML schreiben
            // Starttag des Stammelements
            writer.WriteStartElement("EEC");
            writer.WriteAttributeString("xmlns", "xsi", "", ns);
            writer.WriteAttributeString("xsi", "noNamespaceSchemaLocation",
                  ns, schemaXSD);
            writer.WriteComment(String.Format("Diese Datei wurde aus {0} mit XmlWriter am {1} erzeugt", inputFile, DateTime.Now));

            {
                try
                {
                    foreach (var item in dataWrite)
                    {
                        // BMK des Kastens ermitteln                        
                        bmkKasten = bmkRio(item.Signale[0].AS, item.PL, item.Signale[0].HW_Anbindung.IO_Station);
                        // bmkRioLuft = bmkRioLuftAnschluss(bmkKasten, StringToInt(item.Signale[2].HW_Anbindung.Steckplatz), StringToInt(item.Signale[2].HW_Anbindung.Kanal));
                        // Geräte Anschlüsse ermitteln
                        // anschlussPlus = anschlussGeräte(item.Hersteller, item.Gerät, "Plus");
                        // anschlussMinus = anschlussGeräte(item.Hersteller, item.Gerät, "Minus");

                        hwTypical = @"@PROJEKT\PCH02\HS\HSO";

                        // Daten schreiben
                        //EEC Seitendaten
                        writer.WriteStartElement("TYPICAL_DATA");
                        writer.WriteElementString("HW_Typical", hwTypical);
                        writer.WriteElementString("Functional_assignment", string.Empty);
                        writer.WriteElementString("Higher_level_function", "PL" + item.PL);
                        writer.WriteElementString("Intallation_site", string.Empty);
                        writer.WriteElementString("Mounting_location", item.Ebene);
                        writer.WriteElementString("Higher_level_function_number", string.Empty);
                        writer.WriteElementString("Document_type", string.Empty);
                        writer.WriteElementString("User_defined", item.Funktion.Substring(0, 1));
                        writer.WriteElementString("Page_name", item.TAG_NAME.Substring((item.TAG_NAME.Length > 0 ? item.TAG_NAME.Length - 3 : 0), 3));
                        writer.WriteElementString("Representation_Type", string.Empty);
                        writer.WriteElementString("Variant", string.Empty);
                        writer.WriteElementString("erzeugen", item.EPLAN_DATA.Status_Erstellt == 0 ? string.Empty : "!"); // Wenn schon erzeugt dann, EEC anweisen diese Seite auslassen 
                        writer.WriteElementString("Seitenbeschreibung", item.TAG_NAME + "¶" + item.Bezeichnung);
                        //Typical Daten Seite
                        writer.WriteElementString("Funktionstext", item.TAG_NAME + "¶" + item.Bezeichnung);
                        writer.WriteElementString("DB_Stand", arbeitsStandComosDB); //"R" + item.Signale[0].Revision.RevEPC.ToString() + "." + item.Signale[0].Revision.RevPMS.ToString() + "." + item.Signale[0].Revision.RevACT.ToString());
                        writer.WriteElementString("Messstelle", item.TAG_NAME);
                        writer.WriteElementString("Messstelle_Kabel", item.Geräte_Bezeichnung);
                        // Typical Daten
                        writer.WriteElementString("PL", "PL" + item.PL);
                        writer.WriteElementString("Ebene", item.Ebene);
                        writer.WriteElementString("BMK_RIO", bmkKasten);
                       
                       
                        //Befehl ein
                        writer.WriteElementString(" BMK_SPS_DE_AUF", item.Signale[0].HW_Anbindung.RACK + "." + ((item.Signale[0].HW_Anbindung.Steckplatz.Length == 1) ? "0" + item.Signale[0].HW_Anbindung.Steckplatz : item.Signale[0].HW_Anbindung.Steckplatz));
                        writer.WriteElementString("Signal_AUF", item.Signale[0].Signal);
                        writer.WriteElementString("Adresse_AUF", item.Signale[0].HW_Anbindung.HW_Adresse);
                        writer.WriteElementString("ET200S_8DI_Kanal_Ein", item.Signale[0].HW_Anbindung.Kanal);

                        //Befehl aus
                        writer.WriteElementString(" BMK_SPS_DE_ZU", item.Signale[1].HW_Anbindung.RACK + "." + ((item.Signale[1].HW_Anbindung.Steckplatz.Length == 1) ? "0" + item.Signale[1].HW_Anbindung.Steckplatz : item.Signale[1].HW_Anbindung.Steckplatz));
                        writer.WriteElementString("Signal_ZU", item.Signale[1].Signal);
                        writer.WriteElementString("Adresse_ZU", item.Signale[1].HW_Anbindung.HW_Adresse);
                        writer.WriteElementString("ET200S_8DI_Kanal_ZU", item.Signale[1].HW_Anbindung.Kanal);

                        //Ventil verrigelt
                        writer.WriteElementString("BMK_SPS_DA_verriegelt", item.Signale[2].HW_Anbindung.RACK + "." + ((item.Signale[2].HW_Anbindung.Steckplatz.Length == 1) ? "0" + item.Signale[2].HW_Anbindung.Steckplatz : item.Signale[2].HW_Anbindung.Steckplatz));
                        writer.WriteElementString("Signal_Verrigelt", item.Signale[2].Signal);
                        writer.WriteElementString("Adresse_offen", item.Signale[2].HW_Anbindung.HW_Adresse);
                        writer.WriteElementString("ET200S_8DO_Kanal_Verrigelt", item.Signale[2].HW_Anbindung.Kanal);

                        //Ventil offen
                        writer.WriteElementString("BMK_SPS_DA_offen", item.Signale[3].HW_Anbindung.RACK + "." + ((item.Signale[3].HW_Anbindung.Steckplatz.Length == 1) ? "0" + item.Signale[3].HW_Anbindung.Steckplatz : item.Signale[3].HW_Anbindung.Steckplatz));
                        writer.WriteElementString("Signal_offen", item.Signale[3].Signal);
                        writer.WriteElementString("Adresse_offen", item.Signale[3].HW_Anbindung.HW_Adresse);
                        writer.WriteElementString("ET200S_8DO_Kanal_Ventil_offen", item.Signale[3].HW_Anbindung.Kanal);


                        writer.WriteEndElement();// end
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Es ist ein Fehler beim XML erstellen für die PA-DI Geräte aufgetreten!");
                }

                // Endtag des Stammelements
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
                MessageBox.Show(String.Format(@"Datei {0} erzeugt.", outputFile));
                System.Diagnostics.Process.Start(outputFile);

            }

        }

        private void XMLwriteTRCZ(IEnumerable<MSRMSR_Gerät> filterGeräte, string inputFile, string outputDirectoryEplan)
        {
            // filtern nach den Bedienstellen TRCZ
            var dataWrite =
                 from Geräte in filterGeräte
                 where Geräte.Funktion == "TRCZ" ||(Geräte.Funktion == "TRC" &&(Geräte.Signale.Count() == 2) )
                 select Geräte;

            string bmkET200ispKasten;
            string bmkPaKasten;
            string hwTypical;
            string bmkPaAnschaltung = " ";
            string anschlussPlus;
            string anschlussMinus;

            // ermitteln der ursprünglichen Comos Export Datei
            string arbeitsStandComosDB = extractRevData(inputFile);// inputFile.Substring((inputFile.Length > 0 ? inputFile.Length - 19 : 0), 6);
            string outputFile = (string.Format(outputDirectoryEplan + "\\TRCZ_{0}{1}{2}.xml", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
            // Aufruf Hilfsfunktion zum Laden der PA Config Daten
            loadPaConfig();
            // Aufruf Hilfsfunktion zum Laden der Geräte Config Daten       
            loadGeräteConfig();
            loadRioConfig();

            string schemaXSD = "DatenExportTRCZ.xsd";
            const string ns = "http://www.w3.org/2001/XMLSchema-instance";
            this.Title = "Eplan XML-Datei für TRCZ wird geschrieben.";
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  "; // 2 Leerzeichen
            XmlWriter writer = XmlWriter.Create(@outputFile, settings);
            writer.WriteStartDocument();

            //XML schreiben
            // Starttag des Stammelements
            writer.WriteStartElement("EEC");
            writer.WriteAttributeString("xmlns", "xsi", "", ns);
            writer.WriteAttributeString("xsi", "noNamespaceSchemaLocation",
                  ns, schemaXSD);
            writer.WriteComment(String.Format("Diese Datei wurde aus {0} mit XmlWriter am {1} erzeugt", inputFile, DateTime.Now));

            {
                   try
                    {
                    foreach (var item in dataWrite)
                    {
                        // BMK des Kastens ermitteln                        
                        bmkET200ispKasten = bmkRio(item.Signale[1].AS, item.PL, item.Signale[1].HW_Anbindung.IO_Station);

                        // BMK des Kastens ermitteln
                        bmkPaKasten = bmkPAKaesten(item.PL, item.Signale[0].HW_Anbindung.Busstrang, item.Signale[0].HW_Anbindung.Profibus_PA.SK3, item.Signale[0].HW_Anbindung.Profibus_PA.Trunk);

                    bmkPaAnschaltung = bmkFeldbarieren(bmkPaKasten, item.Signale[0].HW_Anbindung.Profibus_PA.Feldbarriere, item.Signale[0].HW_Anbindung.SlaveTyp);
                    // bmkRioLuft = bmkRioLuftAnschluss(bmkKasten, StringToInt(item.Signale[2].HW_Anbindung.Steckplatz), StringToInt(item.Signale[2].HW_Anbindung.Kanal));
                    // Geräte Anschlüsse ermitteln
                    anschlussPlus = anschlussGeräte(item.Hersteller, item.Gerät, "Plus");
                         anschlussMinus = anschlussGeräte(item.Hersteller, item.Gerät, "Minus");

                        if (item.PL == "31")
                            hwTypical = @"@PROJEKT\PCH02\ET200ISP\ANA_FS_TRCZ_ÜSS";
                        else
                            hwTypical = @"@PROJEKT\PCH02\ET200ISP\ANA_FS_TRCZ";

                        // Daten schreiben
                        //EEC Seitendaten
                        writer.WriteStartElement("TYPICAL_DATA");
                        writer.WriteElementString("HW_Typical", hwTypical);
                        writer.WriteElementString("Functional_assignment", string.Empty);
                        writer.WriteElementString("Higher_level_function", "PL" + item.PL);
                        writer.WriteElementString("Intallation_site", string.Empty);
                        writer.WriteElementString("Mounting_location", item.Ebene);
                        writer.WriteElementString("Higher_level_function_number", string.Empty);
                        writer.WriteElementString("Document_type", string.Empty);
                        writer.WriteElementString("User_defined", item.Funktion.Substring(0, 1));
                        writer.WriteElementString("Page_name", item.TAG_NAME.Substring((item.TAG_NAME.Length > 0 ? item.TAG_NAME.Length - 3 : 0), 3));
                        writer.WriteElementString("Representation_Type", string.Empty);
                        writer.WriteElementString("Variant", string.Empty);
                        writer.WriteElementString("erzeugen", item.EPLAN_DATA.Status_Erstellt == 0 ? string.Empty : "!"); // Wenn schon erzeugt dann, EEC anweisen diese Seite auslassen 
                        writer.WriteElementString("Seitenbeschreibung", item.TAG_NAME + "¶" + item.Bezeichnung);
                        //Typical Daten Seite
                        writer.WriteElementString("Funktionstext", item.TAG_NAME + "¶" + item.Bezeichnung);
                        writer.WriteElementString("DB_Stand", arbeitsStandComosDB); //"R" + item.Signale[0].Revision.RevEPC.ToString() + "." + item.Signale[0].Revision.RevPMS.ToString() + "." + item.Signale[0].Revision.RevACT.ToString());
                        writer.WriteElementString("Messstelle", item.TAG_NAME);

                        // Typical Daten
                        writer.WriteElementString("BMK_RIO", bmkET200ispKasten);
                        //ET200isp
                        writer.WriteElementString("BMK_AI", item.Signale[1].HW_Anbindung.RACK + "." + ((item.Signale[1].HW_Anbindung.Steckplatz.Length == 1) ? "0" + item.Signale[1].HW_Anbindung.Steckplatz : item.Signale[1].HW_Anbindung.Steckplatz));
                        writer.WriteElementString("Adresse_AI", item.Signale[1].HW_Anbindung.HW_Adresse);
                        writer.WriteElementString("Signalname_AI", item.Signale[1].Signal);
                        //Gerätedaten
                        writer.WriteElementString("Typ_Gerät", item.Hersteller + "¶" + item.Gerät + "¶" + item.Geräte_Beschreibung);
                        writer.WriteElementString("Gerät_Funktion", item.Funktion.Substring(0, 1));
                        writer.WriteElementString("Gerät_Anschluss_Plus", anschlussPlus);
                        writer.WriteElementString("Gerät_Anschluss_Minus", anschlussMinus);

                        writer.WriteElementString("ET200isp_AI_Kanal", item.Signale[1].HW_Anbindung.Kanal);
                        writer.WriteElementString("Signalname_RTD", item.Signale[0].Signal);
                        writer.WriteElementString("BMK_PA", bmkPaKasten);
                        writer.WriteElementString("BMK_TI", bmkPaAnschaltung);
                        writer.WriteElementString("Kanal_RTD", item.Signale[0].HW_Anbindung.Kanal);
                        writer.WriteElementString("Messstelle_Kabel", item.Geräte_Bezeichnung);

                       // writer.WriteElementString("PL", "PL" + item.PL);
                       // writer.WriteElementString("Ebene", item.Ebene);                       
                        //PA 
                       // writer.WriteElementString("Adresse_RTD", item.Signale[0].HW_Anbindung.HW_Adresse);
                        writer.WriteEndElement();// end
                    }
                    // Endtag des Stammelements
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Close();
                }
                    catch (Exception)
                    {
                    MessageBox.Show("Es ist ein Fehler beim XML erstellen für die TRCZ Geräte aufgetreten!");
                   }                           
                MessageBox.Show(String.Format(@"Datei {0} erzeugt.", outputFile));
                System.Diagnostics.Process.Start(outputFile);
            }

        }


        private void XMLWriteET200ValvesGO(IEnumerable<MSRMSR_Gerät> filterKlemmeGeräte, string inputFile, string outputDirectoryEplan)
        {

            // filtern nach den Handventilen GO 
            var dataWrite =
                 from Geräte in filterKlemmeGeräte
                 where Geräte.Funktion == "GO" || Geräte.Funktion == "GOA" || Geräte.Funktion == "GS"
                 select Geräte;

            string bmkKasten;
          //  string bmkRioLuft;
            string hwTypical;

            // ermitteln der ursprünglichen Comos Export Datei
            string arbeitsStandComosDB = extractRevData(inputFile);//inputFile.Substring((inputFile.Length > 0 ? inputFile.Length - 19 : 0), 6);
            string outputFile = (string.Format(outputDirectoryEplan + "\\ET200isp_ValvesGO_{0}{1}{2}.xml", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
            // Aufruf Hilfsfunktion zum Laden der PA Config Daten
            //loadPaConfig();
            // Aufruf Hilfsfunktion zum Laden der Geräte Config Daten       
            loadGeräteConfig();
            loadRioConfig();

            string schemaXSD = "DatenExportET200ispValvesGO.xsd";
            const string ns = "http://www.w3.org/2001/XMLSchema-instance";
            this.Title = "Eplan XML-Datei für Ventile wird geschrieben.";
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  "; // 2 Leerzeichen
            XmlWriter writer = XmlWriter.Create(@outputFile, settings);
            writer.WriteStartDocument();

            //XML schreiben
            // Starttag des Stammelements
            writer.WriteStartElement("EEC");
            writer.WriteAttributeString("xmlns", "xsi", "", ns);
            writer.WriteAttributeString("xsi", "noNamespaceSchemaLocation",
                  ns, schemaXSD);
            writer.WriteComment(String.Format("Diese Datei wurde aus {0} mit XmlWriter am {1} erzeugt", inputFile, DateTime.Now));

            {
                try
                {
                    foreach (var item in dataWrite)
                    {
                        // BMK des Kastens ermitteln                        
                        bmkKasten = bmkRio(item.Signale[0].AS, item.PL, item.Signale[0].HW_Anbindung.IO_Station);
                        // bmkRioLuft = bmkRioLuftAnschluss(bmkKasten, StringToInt(item.Signale[2].HW_Anbindung.Steckplatz), StringToInt(item.Signale[2].HW_Anbindung.Kanal));
                        // Geräte Anschlüsse ermitteln
                        // anschlussPlus = anschlussGeräte(item.Hersteller, item.Gerät, "Plus");
                        // anschlussMinus = anschlussGeräte(item.Hersteller, item.Gerät, "Minus");

                        if (item.Signale.Length == 2)
                        {
                            hwTypical = @"@PROJEKT\PCH02\ET200ISP\HOV_2RM";
                        }
                        else
                        {
                            hwTypical = @"@PROJEKT\PCH02\ET200ISP\HOV_1RM";
                        }
                        // Daten schreiben
                        //EEC Seitendaten
                        writer.WriteStartElement("TYPICAL_DATA");
                        writer.WriteElementString("HW_Typical", hwTypical);
                        writer.WriteElementString("Functional_assignment", string.Empty);
                        writer.WriteElementString("Higher_level_function", "PL" + item.PL);
                        writer.WriteElementString("Intallation_site", string.Empty);
                        writer.WriteElementString("Mounting_location", item.Ebene);
                        writer.WriteElementString("Higher_level_function_number", string.Empty);
                        writer.WriteElementString("Document_type", string.Empty);
                        writer.WriteElementString("User_defined", item.Funktion.Substring(0, 1));
                        writer.WriteElementString("Page_name", item.TAG_NAME.Substring((item.TAG_NAME.Length > 0 ? item.TAG_NAME.Length - 3 : 0), 3));
                        writer.WriteElementString("Representation_Type", string.Empty);
                         writer.WriteElementString("Variant", string.Empty);
                            writer.WriteElementString("erzeugen", item.EPLAN_DATA.Status_Erstellt == 0 ? string.Empty : "!"); // Wenn schon erzeugt dann, EEC anweisen diese Seite auslassen 
                           writer.WriteElementString("Seitenbeschreibung", item.TAG_NAME + "¶" + item.Bezeichnung);
                        //Typical Daten Seite
                         writer.WriteElementString("Funktionstext", item.TAG_NAME + "¶" + item.Bezeichnung);
                         writer.WriteElementString("DB_Stand", arbeitsStandComosDB); //"R" + item.Signale[0].Revision.RevEPC.ToString() + "." + item.Signale[0].Revision.RevPMS.ToString() + "." + item.Signale[0].Revision.RevACT.ToString());
                         writer.WriteElementString("Messstelle", item.TAG_NAME);
                         writer.WriteElementString("Messstelle_Kabel", item.Geräte_Bezeichnung);
                        // Typical Daten
                        writer.WriteElementString("BMK_RIO", bmkKasten);
                        writer.WriteElementString("BMK_GOH", "11A" + ((StringToInt(item.Signale[0].HW_Anbindung.Steckplatz)) - 1));
                        writer.WriteElementString("Signalname_GOH", item.Signale[0].Signal);
                        writer.WriteElementString("Adresse_GOH", item.Signale[0].HW_Anbindung.HW_Adresse);

                        if (item.Signale.Length == 2)
                         {
                        writer.WriteElementString("BMK_GOL", "11A" + ((StringToInt(item.Signale[1].HW_Anbindung.Steckplatz)) - 1));
                        writer.WriteElementString("Signalname_GOL", item.Signale[1].Signal);
                        writer.WriteElementString("Adresse_GOL", item.Signale[1].HW_Anbindung.HW_Adresse);
                        writer.WriteElementString("ET200_ISP_GOH_Kanal", item.Signale[0].HW_Anbindung.Kanal);
                        writer.WriteElementString("ET200_ISP_GOL_Kanal", item.Signale[1].HW_Anbindung.Kanal);
                         }
                           else
                         {
                        writer.WriteElementString("BMK_GOL", string.Empty); 
                        writer.WriteElementString("Signalname_GOL", string.Empty);
                        writer.WriteElementString("Adresse_GOL", string.Empty);
                        writer.WriteElementString("ET200_ISP_GOH_Kanal", item.Signale[0].HW_Anbindung.Kanal);
                        writer.WriteElementString("ET200_ISP_GOL_Kanal", string.Empty);
                         }
                        // writer.WriteElementString("Abgang_RIO_LUFT", bmkRioLuft);
                        // writer.WriteElementString("Typ_Gerät", item.Hersteller + "¶" + item.Gerät + "¶" + item.Geräte_Beschreibung);
                        // writer.WriteElementString("Gerät_Funktion", item.Funktion.Substring(0, 1));
                        //  writer.WriteElementString("Gerät_Anschluss_Plus", anschlussPlus);
                        // writer.WriteElementString("Gerät_Anschluss_Minus", anschlussMinus);

                        //writer.WriteElementString("DP_Adresse", item.Signale[0].HW_Anbindung.PB_Adresse);
                        //writer.WriteElementString("DP_Strang", item.Signale[0].HW_Anbindung.Busstrang + Convert.ToString(item.Signale[0].HW_Anbindung.Profibus_PA.SK3) + Convert.ToString(item.Signale[0].HW_Anbindung.Profibus_PA.Trunk));
                        writer.WriteEndElement();// end
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Es ist ein Fehler beim XML erstellen für die GO Handventile aufgetreten!");
                }

                // Endtag des Stammelements
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
                MessageBox.Show(String.Format(@"Datei {0} erzeugt.", outputFile));
                System.Diagnostics.Process.Start(outputFile);
            }

        }

        private void XMLWriteET200FSValves(IEnumerable<MSRMSR_Gerät> filterKlemmeGeräte, string inputFile, string outputDirectoryEplan)
        {


            // filtern nach den Ventilen YZ
            var dataWrite =
                 from Geräte in filterKlemmeGeräte
                 where Geräte.Funktion == "YZ"
                 select Geräte;

            string bmkKasten;
           // string bmkRioLuft;
            string hwTypical;

            // ermitteln der ursprünglichen Comos Export Datei
            string arbeitsStandComosDB = extractRevData(inputFile);// inputFile.Substring((inputFile.Length > 0 ? inputFile.Length - 19 : 0), 6);
            string outputFile = (string.Format(outputDirectoryEplan + "\\ET200isp_FS_Valves_{0}{1}{2}.xml", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
            // Aufruf Hilfsfunktion zum Laden der PA Config Daten
            //loadPaConfig();
            // Aufruf Hilfsfunktion zum Laden der Geräte Config Daten       
            loadGeräteConfig();
            loadRioConfig();

            string schemaXSD = "DatenExportET200FSispValves.xsd";
            const string ns = "http://www.w3.org/2001/XMLSchema-instance";
            this.Title = "Eplan XML-Datei für Ventile wird geschrieben.";
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  "; // 2 Leerzeichen
            XmlWriter writer = XmlWriter.Create(@outputFile, settings);
            writer.WriteStartDocument();

            //XML schreiben
            // Starttag des Stammelements
            writer.WriteStartElement("EEC");
            writer.WriteAttributeString("xmlns", "xsi", "", ns);
            writer.WriteAttributeString("xsi", "noNamespaceSchemaLocation",
                  ns, schemaXSD);
            writer.WriteComment(String.Format("Diese Datei wurde aus {0} mit XmlWriter am {1} erzeugt", inputFile, DateTime.Now));

            {
                try
                {
                    foreach (var item in dataWrite)
                    {
                        // BMK des Kastens ermitteln                        
                        bmkKasten = bmkRio(item.Signale[0].AS, item.PL, item.Signale[0].HW_Anbindung.IO_Station);
                       // bmkRioLuft = bmkRioLuftAnschluss(bmkKasten, StringToInt(item.Signale[2].HW_Anbindung.Steckplatz), StringToInt(item.Signale[2].HW_Anbindung.Kanal));
                        // Geräte Anschlüsse ermitteln
                        // anschlussPlus = anschlussGeräte(item.Hersteller, item.Gerät, "Plus");
                        // anschlussMinus = anschlussGeräte(item.Hersteller, item.Gerät, "Minus");

                        hwTypical = @"@PROJEKT\PCH02\ET200ISP\VALVE_FS";

                        // Daten schreiben
                        //EEC Seitendaten
                        writer.WriteStartElement("TYPICAL_DATA");
                        writer.WriteElementString("HW_Typical", hwTypical);
                        writer.WriteElementString("Functional_assignment", string.Empty);
                        writer.WriteElementString("Higher_level_function", "PL" + item.PL);
                        writer.WriteElementString("Intallation_site", string.Empty);
                        writer.WriteElementString("Mounting_location", item.Ebene);
                        writer.WriteElementString("Higher_level_function_number", string.Empty);
                        writer.WriteElementString("Document_type", string.Empty);
                        writer.WriteElementString("User_defined", item.Funktion.Substring(0, 1));
                        writer.WriteElementString("Page_name", item.TAG_NAME.Substring((item.TAG_NAME.Length > 0 ? item.TAG_NAME.Length - 3 : 0), 3));
                        writer.WriteElementString("Representation_Type", string.Empty);
                        writer.WriteElementString("Variant", string.Empty);
                        writer.WriteElementString("erzeugen", item.EPLAN_DATA.Status_Erstellt == 0 ? string.Empty : "!"); // Wenn schon erzeugt dann, EEC anweisen diese Seite auslassen 
                        writer.WriteElementString("Seitenbeschreibung", item.TAG_NAME + "¶" + item.Bezeichnung);
                        //Typical Daten Seite
                        writer.WriteElementString("Funktionstext", item.TAG_NAME + "¶" + item.Bezeichnung);
                        writer.WriteElementString("DB_Stand", arbeitsStandComosDB); //"R" + item.Signale[0].Revision.RevEPC.ToString() + "." + item.Signale[0].Revision.RevPMS.ToString() + "." + item.Signale[0].Revision.RevACT.ToString());
                        writer.WriteElementString("Messstelle", item.TAG_NAME);
                        writer.WriteElementString("Messstelle_Kabel", item.Geräte_Bezeichnung);
                        // Typical Daten
                        writer.WriteElementString("BMK_RIO", bmkKasten);
                        //Ansteuerung DO
                        writer.WriteElementString("BMK_DO", item.Signale[2].HW_Anbindung.RACK + "." + ((item.Signale[2].HW_Anbindung.Steckplatz.Length == 1) ? "0" + item.Signale[2].HW_Anbindung.Steckplatz : item.Signale[2].HW_Anbindung.Steckplatz));
                        writer.WriteElementString("Signalname_DO", item.Signale[2].Signal);
                        writer.WriteElementString("Adresse_DO", item.Signale[2].HW_Anbindung.HW_Adresse);
                        writer.WriteElementString("ET200_ISP_DO_Kanal", item.Signale[2].HW_Anbindung.Kanal);
                        //GOH
                        writer.WriteElementString("BMK_GOH", item.Signale[0].HW_Anbindung.RACK + "." + ((item.Signale[0].HW_Anbindung.Steckplatz.Length == 1) ? "0" + item.Signale[0].HW_Anbindung.Steckplatz : item.Signale[0].HW_Anbindung.Steckplatz));
                        writer.WriteElementString("Signalname_GOH", item.Signale[0].Signal);
                        writer.WriteElementString("Adresse_GOH", item.Signale[0].HW_Anbindung.HW_Adresse);
                        writer.WriteElementString("ET200_ISP_GOH_Kanal", item.Signale[0].HW_Anbindung.Kanal);
                        //GOL
                        writer.WriteElementString("BMK_GOL", item.Signale[1].HW_Anbindung.RACK + "." + ((item.Signale[1].HW_Anbindung.Steckplatz.Length == 1) ? "0" + item.Signale[1].HW_Anbindung.Steckplatz : item.Signale[1].HW_Anbindung.Steckplatz));
                        writer.WriteElementString("Signalname_GOL", item.Signale[1].Signal);
                        writer.WriteElementString("Adresse_GOL", item.Signale[1].HW_Anbindung.HW_Adresse);
                        writer.WriteElementString("ET200_ISP_GOL_Kanal", item.Signale[1].HW_Anbindung.Kanal);
                       //Geräte


                        //  writer.WriteElementString("Abgang_RIO_LUFT", bmkRioLuft);
                        // writer.WriteElementString("Typ_Gerät", item.Hersteller + "¶" + item.Gerät + "¶" + item.Geräte_Beschreibung);
                        // writer.WriteElementString("Gerät_Funktion", item.Funktion.Substring(0, 1));
                        //  writer.WriteElementString("Gerät_Anschluss_Plus", anschlussPlus);
                        // writer.WriteElementString("Gerät_Anschluss_Minus", anschlussMinus);

                        //writer.WriteElementString("DP_Adresse", item.Signale[0].HW_Anbindung.PB_Adresse);
                        //writer.WriteElementString("DP_Strang", item.Signale[0].HW_Anbindung.Busstrang + Convert.ToString(item.Signale[0].HW_Anbindung.Profibus_PA.SK3) + Convert.ToString(item.Signale[0].HW_Anbindung.Profibus_PA.Trunk));
                        writer.WriteEndElement();// end
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Es ist ein Fehler beim XML erstellen für die PA-DI Geräte aufgetreten!");
                }

                // Endtag des Stammelements
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
                MessageBox.Show(String.Format(@"Datei {0} erzeugt.", outputFile));
                System.Diagnostics.Process.Start(outputFile);
            }

        }

        private void XMLWriteET200FSAna(IEnumerable<MSRMSR_Gerät> filterKlemmeGeräte, string inputFile, string outputDirectoryEplan)
        {
            // filtern nach den Analogen Geräten
            var dataWrite =
                 from Geräte in filterKlemmeGeräte
                 where Geräte.Signale[0].SignalKonfig.Signalrichtung == "Input" && Geräte.Signale[0].SignalKonfig.Signalgrundtyp == "AI"
                 select Geräte;

            string bmkKasten;
            // string bmkRioLuft;
            string hwTypical;
            string anschlussPlus;
            string anschlussMinus;

            // ermitteln der ursprünglichen Comos Export Datei
            string arbeitsStandComosDB = extractRevData(inputFile);//inputFile.Substring((inputFile.Length > 0 ? inputFile.Length - 19 : 0), 6);
            string outputFile = (string.Format(outputDirectoryEplan + "\\ET200isp_FS_ANA_{0}{1}{2}.xml", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
            // Aufruf Hilfsfunktion zum Laden der PA Config Daten
            //loadPaConfig();
            // Aufruf Hilfsfunktion zum Laden der Geräte Config Daten       
            loadGeräteConfig();
            loadRioConfig();

            string schemaXSD = "DatenExportET200FSispAnaFS.xsd";
            const string ns = "http://www.w3.org/2001/XMLSchema-instance";
            this.Title = "Eplan XML-Datei für Ventile wird geschrieben.";
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  "; // 2 Leerzeichen
            XmlWriter writer = XmlWriter.Create(@outputFile, settings);
            writer.WriteStartDocument();

            //XML schreiben
            // Starttag des Stammelements
            writer.WriteStartElement("EEC");
            writer.WriteAttributeString("xmlns", "xsi", "", ns);
            writer.WriteAttributeString("xsi", "noNamespaceSchemaLocation",
                  ns, schemaXSD);
            writer.WriteComment(String.Format("Diese Datei wurde aus {0} mit XmlWriter am {1} erzeugt", inputFile, DateTime.Now));

            {                  
                try 
                {
                    foreach (var item in dataWrite)
                    {
                        // BMK des Kastens ermitteln                        
                        bmkKasten = bmkRio(item.Signale[0].AS, item.PL, item.Signale[0].HW_Anbindung.IO_Station);
                        // bmkRioLuft = bmkRioLuftAnschluss(bmkKasten, StringToInt(item.Signale[2].HW_Anbindung.Steckplatz), StringToInt(item.Signale[2].HW_Anbindung.Kanal));
                        // Geräte Anschlüsse ermitteln
                         anschlussPlus = anschlussGeräte(item.Hersteller, item.Gerät, "Plus");
                         anschlussMinus = anschlussGeräte(item.Hersteller, item.Gerät, "Minus");

                        hwTypical = @"@PROJEKT\PCH02\ET200ISP\ANA_FS";

                        // Daten schreiben
                        //EEC Seitendaten
                        writer.WriteStartElement("TYPICAL_DATA");
                        writer.WriteElementString("HW_Typical", hwTypical);
                        writer.WriteElementString("Functional_assignment", string.Empty);
                        writer.WriteElementString("Higher_level_function", "PL" + item.PL);
                        writer.WriteElementString("Intallation_site", string.Empty);
                        writer.WriteElementString("Mounting_location", item.Ebene);
                        writer.WriteElementString("Higher_level_function_number", string.Empty);
                        writer.WriteElementString("Document_type", string.Empty);
                        writer.WriteElementString("User_defined", item.Funktion.Substring(0, 1));
                        writer.WriteElementString("Page_name", item.TAG_NAME.Substring((item.TAG_NAME.Length > 0 ? item.TAG_NAME.Length - 3 : 0), 3));
                        writer.WriteElementString("Representation_Type", string.Empty);
                        writer.WriteElementString("Variant", string.Empty);
                        writer.WriteElementString("erzeugen", item.EPLAN_DATA.Status_Erstellt == 0 ? string.Empty : "!"); // Wenn schon erzeugt dann, EEC anweisen diese Seite auslassen 
                        writer.WriteElementString("Seitenbeschreibung", item.TAG_NAME + "¶" + item.Bezeichnung);
                        //Typical Daten Seite
                        writer.WriteElementString("Funktionstext", item.TAG_NAME + "¶" + item.Bezeichnung);
                        writer.WriteElementString("DB_Stand", arbeitsStandComosDB); //"R" + item.Signale[0].Revision.RevEPC.ToString() + "." + item.Signale[0].Revision.RevPMS.ToString() + "." + item.Signale[0].Revision.RevACT.ToString());
                        writer.WriteElementString("Messstelle", item.TAG_NAME);
                        writer.WriteElementString("Messstelle_Kabel", item.Geräte_Bezeichnung);
                        // Typical Daten
                        writer.WriteElementString("BMK_RIO", bmkKasten);
                        writer.WriteElementString("BMK_AI", item.Signale[0].HW_Anbindung.RACK + "." + ((item.Signale[0].HW_Anbindung.Steckplatz.Length == 1) ? "0" + item.Signale[0].HW_Anbindung.Steckplatz : item.Signale[0].HW_Anbindung.Steckplatz));
                        writer.WriteElementString("ET200ISP_AI_Kanal", item.Signale[0].HW_Anbindung.Kanal);
                        writer.WriteElementString("Adresse_AI", item.Signale[0].HW_Anbindung.HW_Adresse);
                        writer.WriteElementString("Signalname_AI", item.Signale[0].Signal);
                        
                        //  writer.WriteElementString("Abgang_RIO_LUFT", bmkRioLuft);
                        writer.WriteElementString("Typ_Gerät", item.Hersteller + "¶" + item.Gerät + "¶" + item.Geräte_Beschreibung);
                        writer.WriteElementString("Gerät_Funktion", item.Funktion.Substring(0, 1));
                        writer.WriteElementString("Gerät_Anschluss_Plus", anschlussPlus);
                        writer.WriteElementString("Gerät_Anschluss_Minus", anschlussMinus);

                        //writer.WriteElementString("DP_Adresse", item.Signale[0].HW_Anbindung.PB_Adresse);
                        //writer.WriteElementString("DP_Strang", item.Signale[0].HW_Anbindung.Busstrang + Convert.ToString(item.Signale[0].HW_Anbindung.Profibus_PA.SK3) + Convert.ToString(item.Signale[0].HW_Anbindung.Profibus_PA.Trunk));
                        writer.WriteEndElement();// end
                    }          
                }
                catch (Exception)
                {
                    MessageBox.Show("Es ist ein Fehler beim XML erstellen für die Anlogen FS-Messungen aufgetreten!"); 
                }

                // Endtag des Stammelements
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
                MessageBox.Show(String.Format(@"Datei {0} erzeugt.", outputFile));
                System.Diagnostics.Process.Start(outputFile);
            }

        }
        
        /// <summary>
        /// Ausgabe Daten für Typical der analogen Geräte
        /// </summary>
        /// <param name="filterKlemmeGeräte"></param>
        /// <param name="inputFile"></param>
        /// <param name="outputDirectoryEplan"></param>
        private void xmlWriteET200Ana (IEnumerable<MSRMSR_Gerät> filterKlemmeGeräte, string inputFile, string outputDirectoryEplan)
        {
            
            // filtern nach den Analogen Geräten
            var dataWrite =
                 from Geräte in filterKlemmeGeräte
                 where Geräte.Signale[0].SignalKonfig.Signalrichtung == "Input" && Geräte.Signale[0].SignalKonfig.Signalgrundtyp == "AI"
                 select Geräte;

            string bmkKasten;
            // string bmkRioLuft;
            string hwTypical;
            string anschlussPlus;
            string anschlussMinus;

            // ermitteln der ursprünglichen Comos Export Datei
            string arbeitsStandComosDB = extractRevData(inputFile);// inputFile.Substring((inputFile.Length > 0 ? inputFile.Length - 19 : 0), 6);
            string outputFile = (string.Format(outputDirectoryEplan + "\\ET200isp_ANA_{0}{1}{2}.xml", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
            // Aufruf Hilfsfunktion zum Laden der PA Config Daten
            //loadPaConfig();
            // Aufruf Hilfsfunktion zum Laden der Geräte Config Daten       
            loadGeräteConfig();
            loadRioConfig();

            string schemaXSD = "DatenExportET200ispAna.xsd";
            const string ns = "http://www.w3.org/2001/XMLSchema-instance";
            this.Title = "Eplan XML-Datei für Ventile wird geschrieben.";
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  "; // 2 Leerzeichen
            XmlWriter writer = XmlWriter.Create(@outputFile, settings);
            writer.WriteStartDocument();

            //XML schreiben
            // Starttag des Stammelements
            writer.WriteStartElement("EEC");
            writer.WriteAttributeString("xmlns", "xsi", "", ns);
            writer.WriteAttributeString("xsi", "noNamespaceSchemaLocation",
                  ns, schemaXSD);
            writer.WriteComment(String.Format("Diese Datei wurde aus {0} mit XmlWriter am {1} erzeugt", inputFile, DateTime.Now));

            {
                try
                {
                    foreach (var item in dataWrite)
                    {
                        // BMK des Kastens ermitteln                        
                        bmkKasten = bmkRio(item.Signale[0].AS, item.PL, item.Signale[0].HW_Anbindung.IO_Station);
                        // bmkRioLuft = bmkRioLuftAnschluss(bmkKasten, StringToInt(item.Signale[2].HW_Anbindung.Steckplatz), StringToInt(item.Signale[2].HW_Anbindung.Kanal));
                        // Geräte Anschlüsse ermitteln
                        anschlussPlus = anschlussGeräte(item.Hersteller, item.Gerät, "Plus");
                        anschlussMinus = anschlussGeräte(item.Hersteller, item.Gerät, "Minus");

                        hwTypical = @"@PROJEKT\PCH02\ET200ISP\ANA";

                        // Daten schreiben
                        //EEC Seitendaten
                        writer.WriteStartElement("TYPICAL_DATA");
                        writer.WriteElementString("HW_Typical", hwTypical);
                        writer.WriteElementString("Functional_assignment", string.Empty);
                        writer.WriteElementString("Higher_level_function", "PL" + item.PL);
                        writer.WriteElementString("Intallation_site", string.Empty);
                        writer.WriteElementString("Mounting_location", item.Ebene);
                        writer.WriteElementString("Higher_level_function_number", string.Empty);
                        writer.WriteElementString("Document_type", string.Empty);
                        writer.WriteElementString("User_defined", item.Funktion.Substring(0, 1));
                        writer.WriteElementString("Page_name", item.TAG_NAME.Substring((item.TAG_NAME.Length > 0 ? item.TAG_NAME.Length - 3 : 0), 3));
                        writer.WriteElementString("Representation_Type", string.Empty);
                        writer.WriteElementString("Variant", string.Empty);
                        writer.WriteElementString("erzeugen", item.EPLAN_DATA.Status_Erstellt == 0 ? string.Empty : "!"); // Wenn schon erzeugt dann, EEC anweisen diese Seite auslassen 
                        writer.WriteElementString("Seitenbeschreibung", item.TAG_NAME + "¶" + item.Bezeichnung);
                        //Typical Daten Seite
                        writer.WriteElementString("Funktionstext", item.TAG_NAME + "¶" + item.Bezeichnung);
                        writer.WriteElementString("DB_Stand", arbeitsStandComosDB); //"R" + item.Signale[0].Revision.RevEPC.ToString() + "." + item.Signale[0].Revision.RevPMS.ToString() + "." + item.Signale[0].Revision.RevACT.ToString());
                        writer.WriteElementString("Messstelle", item.TAG_NAME);
                        writer.WriteElementString("Messstelle_Kabel", item.Geräte_Bezeichnung);
                        // Typical Daten
                        writer.WriteElementString("BMK_RIO", bmkKasten);
                       // writer.WriteElementString("BMK_AI", item.Signale[0].HW_Anbindung.RACK + "." + ((item.Signale[0].HW_Anbindung.Steckplatz.Length == 1) ? "0" + item.Signale[0].HW_Anbindung.Steckplatz : item.Signale[0].HW_Anbindung.Steckplatz));
                        writer.WriteElementString("BMK_AI", "11A" + ((StringToInt(item.Signale[0].HW_Anbindung.Steckplatz)) - 1));
                        writer.WriteElementString("Adresse_AI", item.Signale[0].HW_Anbindung.HW_Adresse);
                        writer.WriteElementString("Signalname_AI", item.Signale[0].Signal);


                        //  writer.WriteElementString("Abgang_RIO_LUFT", bmkRioLuft);
                        writer.WriteElementString("Typ_Gerät", item.Hersteller + "¶" + item.Gerät + "¶" + item.Geräte_Beschreibung);
                        writer.WriteElementString("Gerät_Funktion", item.Funktion.Substring(0, 1));
                        writer.WriteElementString("Gerät_Anschluss_Plus", anschlussPlus);
                        writer.WriteElementString("Gerät_Anschluss_Minus", anschlussMinus);
                        writer.WriteElementString("ET200ISP_AI_Kanal", item.Signale[0].HW_Anbindung.Kanal);

                        //writer.WriteElementString("DP_Adresse", item.Signale[0].HW_Anbindung.PB_Adresse);
                        //writer.WriteElementString("DP_Strang", item.Signale[0].HW_Anbindung.Busstrang + Convert.ToString(item.Signale[0].HW_Anbindung.Profibus_PA.SK3) + Convert.ToString(item.Signale[0].HW_Anbindung.Profibus_PA.Trunk));
                        writer.WriteEndElement();// end
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Es ist ein Fehler beim XML erstellen für die Anlogen FS-Messungen aufgetreten!");
                }

                // Endtag des Stammelements
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
                MessageBox.Show(String.Format(@"Datei {0} erzeugt.", outputFile));
                System.Diagnostics.Process.Start(outputFile);
            }
        }

        /// <summary>
        /// Ausgabe Typical Druckluftmembran-Pumpe
        /// </summary>
        /// <param name="filterGeräte"></param>
        /// <param name="pumpenConfig"></param>
        /// <param name="inputFile"></param>
        /// <param name="outputDirectoryEplan"></param>
        private void xmlWritePumpen(IEnumerable<MSRMSR_Gerät> filterGeräte, MSRGeraete pumpenConfig, string inputFile, string outputDirectoryEplan)
        {

            List<PumpenConfig> sensorListe = new List<PumpenConfig>();
            //Sensoren der Pumpen ermitteln
            sensorListe = sensorenAnPumpen(pumpenConfig);
            // Filtern nach den Geräten der Pumpen
            //   foreach (var item in sensorListe)
            //     Console.WriteLine(item.Sensor);

            IEnumerable<MSRMSRGerät_Pumpe> pumpExportdata = new List<MSRMSRGerät_Pumpe>();    
            pumpExportdata =
               from geräte in filterGeräte
               join pumptemp in sensorListe
               on geräte.TAG_NAME equals pumptemp.Sensor
               orderby pumptemp.BMK
              select new MSRMSRGerät_Pumpe { MSR_Gerät = geräte, BMK = pumptemp.BMK , PumpeBezeichnung = pumptemp.Bezeichnung, PL = pumptemp.PL, Ebene = pumptemp.Ebene };
            //select new MSR {MSR_Gerät.};

            // umkopieren von IEnumerable nach List
            List<MSRMSRGerät_Pumpe> dataWrite = new List<MSRMSRGerät_Pumpe>();                      
            foreach (var item in pumpExportdata)
            {
                dataWrite.Add(item);
            }

                  foreach (var item in dataWrite)
                 {
                    Console.WriteLine("Pumpe: {0}, Pumpenbezeichnnug: {1} Sensor: {2}", item.BMK, item.PumpeBezeichnung ,item.MSR_Gerät.TAG_NAME );
                }

            string bmkKasten;
            //string bmkRioLuft;
            string hwTypical;

            // ermitteln der ursprünglichen Comos Export Datei
            string arbeitsStandComosDB = extractRevData(inputFile);//inputFile.Substring((inputFile.Length > 0 ? inputFile.Length - 19 : 0), 6);
            string outputFile = (string.Format(outputDirectoryEplan + "\\DRL_{0}{1}{2}.xml", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));

          
            // Aufruf Hilfsfunktion zum Laden der PA Config Daten
            loadPaConfig();
            // Aufruf Hilfsfunktion zum Laden der Geräte Config Daten       
            loadGeräteConfig();
            loadRioConfig();

          
            string schemaXSD = "DatenExportET200ispDRL.xsd";
            const string ns = "http://www.w3.org/2001/XMLSchema-instance";
            this.Title = "Eplan XML-Datei für Druckluftpumpen wird geschrieben.";
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  "; // 2 Leerzeichen
            XmlWriter writer = XmlWriter.Create(@outputFile, settings);
            writer.WriteStartDocument();

            //XML schreiben
            // Starttag des Stammelements
             writer.WriteStartElement("EEC");
             writer.WriteAttributeString("xmlns", "xsi", "", ns);
            writer.WriteAttributeString("xsi", "noNamespaceSchemaLocation",
             ns, schemaXSD);
             writer.WriteComment(String.Format("Diese Datei wurde aus {0} mit XmlWriter am {1} erzeugt", inputFile, DateTime.Now));

            // {
            // try
            // {
            foreach (var item in dataWrite)
             {
                // BMK des Kastens ermitteln 
                bmkKasten = "0";
                if (item.MSR_Gerät.Signale[0].HW_Anbindung.Kartentyp == "ET200iSP")
                    bmkKasten = bmkRio(item.MSR_Gerät.Signale[0].AS, item.MSR_Gerät.PL, item.MSR_Gerät.Signale[0].HW_Anbindung.IO_Station);
                if (item.MSR_Gerät.Signale[0].HW_Anbindung.Kartentyp == "SK3")
                    bmkKasten = bmkPAKaesten(item.MSR_Gerät.PL,item.MSR_Gerät.Signale[0].HW_Anbindung.Busstrang, item.MSR_Gerät.Signale[0].HW_Anbindung.Profibus_PA.SK3, item.MSR_Gerät.Signale[0].HW_Anbindung.Profibus_PA.Trunk);
                    // Geräte Anschlüsse ermitteln
                    // anschlussPlus = anschlussGeräte(item.Hersteller, item.Gerät, "Plus");
                    // anschlussMinus = anschlussGeräte(item.Hersteller, item.Gerät, "Minus");

                    // if (item.PumpeBezeichnung == "!")
                    hwTypical = @"@  @PROJEKT\PCH02\DRL_P\DRLP_ZÄHLER_1Überwachnung";

            // @PROJEKT\PCH02\DRL_P\DRLP_ZÄHLER_4Überwachnungen;
            // Daten schreiben
            //EEC Seitendaten
            writer.WriteStartElement("TYPICAL_DATA");
            writer.WriteElementString("HW_Typical", hwTypical);
            writer.WriteElementString("Functional_assignment", string.Empty);
            writer.WriteElementString("Higher_level_function", "PL" + item.PL);
            writer.WriteElementString("Intallation_site", string.Empty);
            writer.WriteElementString("Mounting_location", (item.Ebene).ToString());
            writer.WriteElementString("Higher_level_function_number", string.Empty);
            writer.WriteElementString("Document_type", string.Empty);
                writer.WriteElementString("User_defined", "DRLP");// item.MSR_Gerät.Funktion.Substring(0, 1));
            writer.WriteElementString("Page_name", item.MSR_Gerät.TAG_NAME.Substring((item.BMK.Length > 0 ? item.BMK.Length - 3 : 0), 2));
            writer.WriteElementString("Representation_Type", string.Empty);
            writer.WriteElementString("Variant", string.Empty);
            writer.WriteElementString("erzeugen", item.MSR_Gerät.EPLAN_DATA.Status_Erstellt == 0 ? string.Empty : "!"); // Wenn schon erzeugt dann, EEC anweisen diese Seite auslassen 
            writer.WriteElementString("Seitenbeschreibung", item.BMK + "¶" + item.PumpeBezeichnung);
            //Typical Daten Seite
            writer.WriteElementString("Funktionstext", item.MSR_Gerät.TAG_NAME + "¶" + item.MSR_Gerät.Bezeichnung);
            writer.WriteElementString("DB_Stand", arbeitsStandComosDB); //"R" + item.Signale[0].Revision.RevEPC.ToString() + "." + item.Signale[0].Revision.RevPMS.ToString() + "." + item.Signale[0].Revision.RevACT.ToString());
            writer.WriteElementString("Messstelle", item.MSR_Gerät.TAG_NAME);
            writer.WriteElementString("Messstelle_Kabel", item.MSR_Gerät.Geräte_Bezeichnung);
                // Typical Daten
                //Ansteuerung

                writer.WriteElementString("BMK_RIO", bmkKasten);
                writer.WriteElementString("BMK_SPS", item.MSR_Gerät.Signale[0].HW_Anbindung.RACK + "." + ((item.MSR_Gerät.Signale[0].HW_Anbindung.Steckplatz.Length == 1) ? "0" + item.MSR_Gerät.Signale[0].HW_Anbindung.Steckplatz : item.MSR_Gerät.Signale[0].HW_Anbindung.Steckplatz));
                writer.WriteElementString("Signalname", item.MSR_Gerät.Signale[0].Signal);
            writer.WriteElementString("Adresse", item.MSR_Gerät.Signale[0].HW_Anbindung.HW_Adresse);
            writer.WriteElementString("Kanal", item.MSR_Gerät.Signale[0].HW_Anbindung.Kanal);

            writer.WriteElementString("DP_Adresse", item.MSR_Gerät.Signale[0].HW_Anbindung.PB_Adresse);
             writer.WriteElementString("DP_Strang", item.MSR_Gerät.Signale[0].HW_Anbindung.Busstrang + Convert.ToString(item.MSR_Gerät.Signale[0].HW_Anbindung.Profibus_PA.SK3) + Convert.ToString(item.MSR_Gerät.Signale[0].HW_Anbindung.Profibus_PA.Trunk));
            writer.WriteEndElement();// end
            }
            //   }
            // catch (Exception)
            // {
            // MessageBox.Show("Es ist ein Fehler beim XML erstellen für die PA-DI Geräte aufgetreten!");
            // }

            // Endtag des Stammelements
             writer.WriteEndElement();
             writer.WriteEndDocument();
             writer.Close();
             MessageBox.Show(String.Format(@"Datei {0} erzeugt.", outputFile));
             System.Diagnostics.Process.Start(outputFile);
            //  }
        }
       
    }
}