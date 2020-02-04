
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Xml;
using System.Linq;
using Excel = Microsoft.Office.Interop.Excel;
using System.Collections.Generic;

namespace ComosToEplan
{

    /// <summary>
    /// Funktionen für den Import von Daten nach XML 
    /// </summary>
    public partial class MainWindow

    {
        /// <summary>
        /// Import der Konfigurationsliste aus Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnImportComosData_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            Excel.Range range;
            object misValue = Missing.Value;

            ComosDataImport[] importEPC;
            int offsetZeile = 0;

            // Offset aus Eingabe lesen
            int offsetSpalte = 0;

            int offsetSpalteSignalbezeichnung = 0;
            // MessageBox.Show(String.Format("Der Offset ist {0}.", offsetEXC));

            // Prüfen ob Datei existiert
            if (string.IsNullOrEmpty(tbImportExcel.Text) || !File.Exists(tbImportExcel.Text))
                MessageBox.Show("Es wurde keine Datei gewählt");
            else
            {
                
                Title = "Exceldatei wird ausgelesen";

                string revData = extractRevData(tbImportExcel.Text);
                xlApp = new Excel.Application();
                xlWorkBook = xlApp.Workbooks.Open(tbImportExcel.Text, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                // Prüfen wieviel Tabellenblätter da sind
                int error = 0;
                foreach (Excel.Worksheet xlworksheet in xlWorkBook.Worksheets)
                {
                    error++;
                }
                if (error == 1)
                    MessageBox.Show(string.Format("Nur {0} Arbeitsblatt vorhanden! Falsche Datei?", error));
                else
                {
                    xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(2);
                    range = xlWorkSheet.UsedRange;

                    if (range.Count <= 2)
                        MessageBox.Show(string.Format("Keine Daten vorhanden! Falsche Datei?"));
                    else
                    {
                        // Offset der Spalte "Revision" auslesen aus Datei Startpunkt ist: Revision 
                        bool abbruch = false;

                        for (int i = 1; i < 10; i++)
                        {
                            if (abbruch) break;
                            for (int j = 1; j < 10; j++)
                            {
                                string temp = ((string)(range.Cells[i, j] as Excel.Range).Value);
                                if (string.Equals(temp, "Revision"))
                                {
                                    abbruch = true;
                                    offsetSpalte = j;
                                    offsetZeile = i + 1;//um erste Datenzeile zu selektieren
                                    break;
                                }
                            }
                        }

                        // Offset der Spalte "Signalbezeichnung" wegen Signal-Status" auslesen und Offset ab Spalte "Signalbezeichnung" 
                        abbruch = false;
                        for (int i = (offsetSpalte); i < 40; i++)
                        {
                            string temp = ((string)(range.Cells[2, i] as Excel.Range).Value2);
                            if (string.Equals(temp, "Signalbezeichnung"))
                            {
                                abbruch = true;
                                offsetSpalteSignalbezeichnung = i;
                                break;
                            }
                        }
                        if ((offsetZeile >= 10) || (offsetSpalte >= 10)||(offsetSpalteSignalbezeichnung >= 40) || (offsetSpalteSignalbezeichnung <= 0))
                        {
                            MessageBox.Show(string.Format("Fehler beim einlesen der Exceldatei!"));
                        }
                        else
                        {
                            //Offset Spalte korrigieren
                            offsetSpalte = offsetSpalte - 1;
                            offsetSpalteSignalbezeichnung = offsetSpalteSignalbezeichnung - 30;

                            // Größe des Arrys festlegen
                            importEPC = new ComosDataImport[range.Rows.Count + 1];
                            pBprogress.Minimum = 0;
                            pBprogress.Maximum = range.Rows.Count;
                            pBprogress.Value = 0;
                            pBprogress.Visibility = Visibility.Visible;
                            // Daten einlesen
                            for (int i = offsetZeile; i <= range.Rows.Count; i++)
                            {
                                try
                                {
                                    progressbar(i);
                                string tempString;
                                    // Variante mit comosData
                                    importEPC[i].RevEPC = StringToInt((range.Cells[i, 1 + offsetSpalte] as Excel.Range).Value2);
                                    importEPC[i].RevEPCKommentar = (string)(range.Cells[i, 2 + offsetSpalte] as Excel.Range).Value2;
                                    if ((string)(range.Cells[i, 3 + offsetSpalte] as Excel.Range).Value2 == "in Arbeit")
                                    {
                                        importEPC[i].aktiv = true;
                                    }
                                    else
                                    {
                                        importEPC[i].aktiv = false;
                                    }
                                    importEPC[i].RevPMS = StringToInt((range.Cells[i, 4 + offsetSpalte] as Excel.Range).Text.ToString());
                                    importEPC[i].RevPMSKommentar = (string)(range.Cells[i, 5 + offsetSpalte] as Excel.Range).Value2;
                                    importEPC[i].RevACT = StringToInt((range.Cells[i, 6 + offsetSpalte] as Excel.Range).Text.ToString());
                                    importEPC[i].RevACTKommentar = (string)(range.Cells[i, 7 + offsetSpalte] as Excel.Range).Value2;
                                    importEPC[i].Erzeugt = DateTime.ParseExact(((string)(range.Cells[i, 8 + offsetSpalte] as Excel.Range).Value2), "yyyy.MM.dd", CultureInfo.InvariantCulture);
                                    importEPC[i].Funktion = (string)(range.Cells[i, 9 + offsetSpalte] as Excel.Range).Value2;
                                    importEPC[i].Tag_NR = (string)(range.Cells[i, 10 + offsetSpalte] as Excel.Range).Value2;
                                    importEPC[i].TAG_NAME = (string)(range.Cells[i, 11 + offsetSpalte] as Excel.Range).Value2;
                                    importEPC[i].TAG_NAME_ACT = (string)(range.Cells[i, 12 + offsetSpalte] as Excel.Range).Value2;
                                    importEPC[i].PL = (string)(range.Cells[i, 13 + offsetSpalte] as Excel.Range).Value2;                                     
                                    importEPC[i].Beschreibung = (string)(range.Cells[i, 14 + offsetSpalte] as Excel.Range).Value2;
                                    importEPC[i].Ebene = (string)(range.Cells[i, 15 + offsetSpalte] as Excel.Range).Value2;
                                    importEPC[i].Koordinate = (string)(range.Cells[i, 16 + offsetSpalte] as Excel.Range).Value2;
                                    importEPC[i].Hersteller = (string)(range.Cells[i, 17 + offsetSpalte] as Excel.Range).Value2;
                                    importEPC[i].Gerät = (string)(range.Cells[i, 18 + offsetSpalte] as Excel.Range).Value2;
                                    importEPC[i].gerätBeschreibung = (string)(range.Cells[i, 19 + offsetSpalte] as Excel.Range).Value2;
                                    importEPC[i].Anbindung = (string)(range.Cells[i, 20 + offsetSpalte] as Excel.Range).Value2;
                                    importEPC[i].Signalrichtung = (string)(range.Cells[i, 21 + offsetSpalte] as Excel.Range).Value2;
                                    importEPC[i].Wertebereich = (string)(range.Cells[i, 22 + offsetSpalte] as Excel.Range).Value2;
                                    importEPC[i].Signalphysik = (string)(range.Cells[i, 23 + offsetSpalte] as Excel.Range).Value2;
                                    importEPC[i].Signalindex = StringToInt((range.Cells[i, 24 + offsetSpalte] as Excel.Range).Value2);
                                    importEPC[i].SafePosition = (string)(range.Cells[i, 25 + offsetSpalte] as Excel.Range).Value2;
                                    importEPC[i].SW_Typical = (string)(range.Cells[i, 26 + offsetSpalte] as Excel.Range).Value2;
                                    importEPC[i].Signal_Erzeugt = DateTime.ParseExact(((string)(range.Cells[i, 28 + offsetSpalte] as Excel.Range).Value2), "yyyy.MM.dd", CultureInfo.InvariantCulture);
                                    importEPC[i].Signalerweiterung = (string)(range.Cells[i, 29 + offsetSpalte] as Excel.Range).Value2;
                                   
                                    if (offsetSpalteSignalbezeichnung <= 0)
                                        importEPC[i].signalaktiv = true;
                                    else {
                                        if ((string)(range.Cells[i, 29 + offsetSpalteSignalbezeichnung] as Excel.Range).Value2 == "in Arbeit")
                                        {
                                            importEPC[i].signalaktiv = true;
                                        }
                                        else
                                        {
                                            importEPC[i].signalaktiv = false;
                                        }
                                    }
                                    importEPC[i].Signal = (string)(range.Cells[i, 30 + offsetSpalteSignalbezeichnung] as Excel.Range).Value2;
                                    importEPC[i].Verbalbeschreibung = (string)(range.Cells[i, 31 + offsetSpalteSignalbezeichnung] as Excel.Range).Value2;
                                    importEPC[i].Signalgrundtyp = (string)(range.Cells[i, 33 + offsetSpalteSignalbezeichnung] as Excel.Range).Value2;
                                    importEPC[i].messbereichMin = (string)(range.Cells[i, 34 + offsetSpalteSignalbezeichnung] as Excel.Range).Value2;
                                    importEPC[i].messbereichMax = (string)(range.Cells[i, 35 + offsetSpalteSignalbezeichnung] as Excel.Range).Value2;
                                    importEPC[i].messbereichEinheit = (string)(range.Cells[i, 36 + offsetSpalteSignalbezeichnung] as Excel.Range).Value2;
                                    importEPC[i].Hysterese = (string)(range.Cells[i, 37 + offsetSpalteSignalbezeichnung] as Excel.Range).Value2;
                                    importEPC[i].Reglerwirkung = (string)(range.Cells[i, 38 + offsetSpalteSignalbezeichnung] as Excel.Range).Value2;
                                    importEPC[i].StellgrenzeMin = (string)(range.Cells[i, 39 + offsetSpalteSignalbezeichnung] as Excel.Range).Value2;
                                    importEPC[i].StellgrenzeMax = (string)(range.Cells[i, 40 + offsetSpalteSignalbezeichnung] as Excel.Range).Value2;
                                    importEPC[i].AlarmUnten = (string)(range.Cells[i, 41 + offsetSpalteSignalbezeichnung] as Excel.Range).Value2;
                                    importEPC[i].WarnungUnten = (string)(range.Cells[i, 42 + offsetSpalteSignalbezeichnung] as Excel.Range).Value2;
                                    importEPC[i].TolereanzUnten = (string)(range.Cells[i, 43 + offsetSpalteSignalbezeichnung] as Excel.Range).Value2;
                                    importEPC[i].AlarmOben = (string)(range.Cells[i, 44 + offsetSpalteSignalbezeichnung] as Excel.Range).Value2;
                                    importEPC[i].WarnungOben = (string)(range.Cells[i, 45 + offsetSpalteSignalbezeichnung] as Excel.Range).Value2;
                                    importEPC[i].ToleranzOben = (string)(range.Cells[i, 46 + offsetSpalteSignalbezeichnung] as Excel.Range).Value2;
                                    importEPC[i].BedeutungFalse = (string)(range.Cells[i, 47 + offsetSpalteSignalbezeichnung] as Excel.Range).Value2;
                                    importEPC[i].BedeutungTrue = (string)(range.Cells[i, 48 + offsetSpalteSignalbezeichnung] as Excel.Range).Value2;
                                    importEPC[i].revers = (string)(range.Cells[i, 49 + offsetSpalteSignalbezeichnung] as Excel.Range).Value2;
                                    importEPC[i].Busstrang = (string)(range.Cells[i, 50 + offsetSpalteSignalbezeichnung] as Excel.Range).Value2;
                                    importEPC[i].SK3 = StringToInt((range.Cells[i, 51 + offsetSpalteSignalbezeichnung] as Excel.Range).Text.ToString());
                                    importEPC[i].Trunk = StringToInt((range.Cells[i, 52 + offsetSpalteSignalbezeichnung] as Excel.Range).Text.ToString());
                                    importEPC[i].Feldbarriere = StringToInt((range.Cells[i, 53 + offsetSpalteSignalbezeichnung] as Excel.Range).Text.ToString());
                                    importEPC[i].AS = (string)(range.Cells[i, 54 + offsetSpalteSignalbezeichnung] as Excel.Range).Value2;
                                    importEPC[i].IO_Station = (string)(range.Cells[i, 55 + offsetSpalteSignalbezeichnung] as Excel.Range).Value2;
                                    importEPC[i].RACK = (string)(range.Cells[i, 56 + offsetSpalteSignalbezeichnung] as Excel.Range).Value2;
                                    importEPC[i].Steckplatz = (string)(range.Cells[i, 57 + offsetSpalteSignalbezeichnung] as Excel.Range).Text.ToString();
                                    importEPC[i].Kanal = (string)(range.Cells[i, 58 + offsetSpalteSignalbezeichnung] as Excel.Range).Text.ToString();
                                    importEPC[i].HW_Adresse = (string)(range.Cells[i, 60 + offsetSpalteSignalbezeichnung] as Excel.Range).Value2;
                                    importEPC[i].PB_Adresse = (string)(range.Cells[i, 61 + offsetSpalteSignalbezeichnung] as Excel.Range).Text.ToString();
                                    importEPC[i].Kartentyp = (string)(range.Cells[i, 62 + offsetSpalteSignalbezeichnung] as Excel.Range).Value2;
                                    importEPC[i].SlaveTyp = (string)(range.Cells[i, 63 + offsetSpalteSignalbezeichnung] as Excel.Range).Value2;
                                    tempString = (string)(range.Cells[i, 67 + offsetSpalteSignalbezeichnung] as Excel.Range).Value2;
                                    importEPC[i].Ausbaustufe = int.Parse(tempString.Remove(1));
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show(String.Format("Fehler beim einlesen der Exceldatei Datensatz {0}!", i));
                                }
                            }

                            // sortieren nach Signalname, um alle falsch eingelesenen Elemente am Anfng zu haben
                            Array.Sort(importEPC, delegate (ComosDataImport x, ComosDataImport y) { return string.Compare(x.Signal, y.Signal); });

                            // zählen wie viele Elemente beschrieben sind
                            int g = 0;
                            foreach (ComosDataImport temp in importEPC)
                            {
                                if (temp.Tag_NR != null)
                                {
                                    g++;
                                }
                            }
                            // Erstellen des Temp-Elementes                         
                            ComosDataImport[] importEPC_2;
                            importEPC_2 = new ComosDataImport[g];

                            // Elemente in neue Struktur umkopieren
                            for (int i = 0; i < g; i++)
                            {
                                try
                                {
                                    importEPC_2[i] = importEPC[i + offsetZeile];
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show(string.Format("Fehler beim einlesen der Exceldatei Datensatz {0}!", i));
                                }
                            }

                            // sortieren nach Signalname, um Reihenfolge richtig ein zu halten
                            Array.Sort(importEPC_2, delegate (ComosDataImport x, ComosDataImport y) { return string.Compare(x.Signal, y.Signal); });


                            // Daten schreiben
                            string XMLFile = string.Format(outputDirectory + @"\Ausgabe_ComosDaten_{0}.xml", revData);

                            if (!Directory.Exists(outputDirectory))
                            {

                                Directory.CreateDirectory(outputDirectory);
                            }
                            //Daten für Eplan
                            XMLwrite(importEPC_2, XMLFile);
                            tbopenComosContent.Text = XMLFile;
                            // Daten für Vergleich
                            // Daten schreiben
                            XMLFile = string.Format(outputDirectory + @"\Ausgabe_ComosDaten_für_Vergleich_{0}.xml", revData);

                            XMLVergleichWrite(importEPC_2, XMLFile);
                   // Worksheet schliesen
                            releaseObject(xlWorkSheet);
                        }
                    }
                }
                // Excel beenden
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();
                releaseObject(xlWorkBook);
                releaseObject(xlApp);
                Title = "Exceldatei ist geschlossen";
                pBprogress.Visibility = Visibility.Hidden;
            }
        }

        // schreiben der XML-Datei für Comos Data
        private void XMLwrite(ComosDataImport[] dataWrite, string OutputFile)
        {
            string schemaXSD = "ComosDaten.xsd";
            const string ns = "http://www.w3.org/2001/XMLSchema-instance";

            this.Title = "XML-Datei wird geschrieben.";
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  "; // 2 Leerzeichen
            XmlWriter writer = XmlWriter.Create(@OutputFile, settings);
            writer.WriteStartDocument();

            this.Title = "XML wird exportiert";

            //XML schreiben
            // Starttag des Stammelements

            writer.WriteStartElement("MSR");
            writer.WriteAttributeString("xmlns", "xsi", "", ns);
            writer.WriteAttributeString("xsi", "noNamespaceSchemaLocation",
                  ns, schemaXSD);
            // writer.WriteRaw(schemaXSD);

            // writer.WriteAttributeString("xsi","ComosDaten.xsd");
            writer.WriteComment(String.Format("Diese Datei wurde aus {0} mit XmlWriter am {1} erzeugt", tbImportExcel.Text, DateTime.Now));

            int i = 0;
            do
            {
                try
                {
                    // Starttag von 'TAG'
                    writer.WriteStartElement("MSR_Gerät");
                    writer.WriteStartAttribute("TAG");
                    writer.WriteValue(dataWrite[i].TAG_NAME.Remove(1) + dataWrite[i].TAG_NAME.Substring((dataWrite[i].TAG_NAME.Length > 0 ? dataWrite[i].TAG_NAME.Length - 5 : 0), 5));
                    writer.WriteEndAttribute();
                    writer.WriteElementString("Erzeugt", dataWrite[i].Erzeugt.ToShortDateString());
                    writer.WriteElementString("Geräte_Bezeichnung", dataWrite[i].TAG_NAME.Remove(1) + dataWrite[i].TAG_NAME.Substring((dataWrite[i].TAG_NAME.Length > 0 ? dataWrite[i].TAG_NAME.Length - 5 : 0), 5));
                    // writer.WriteElementString("TAG-Nr", dataWrite[i].TAG_NAME);
                    writer.WriteElementString("Bezeichnung", dataWrite[i].Beschreibung);
                    writer.WriteElementString("Aktiv", dataWrite[i].aktiv ? "1" : "0");
                    writer.WriteElementString("Ausbaustufe", ((int)dataWrite[i].Ausbaustufe).ToString());
                    writer.WriteElementString("Funktion", dataWrite[i].Funktion);
                    writer.WriteElementString("Tag_NR", dataWrite[i].Tag_NR);
                    writer.WriteElementString("TAG_NAME", dataWrite[i].TAG_NAME);
                    writer.WriteElementString("TAG_NAME_ACT", dataWrite[i].TAG_NAME_ACT);
                    writer.WriteElementString("PL", dataWrite[i].PL);
                    writer.WriteElementString("Ebene", dataWrite[i].Ebene);
                    writer.WriteElementString("Koordinate", dataWrite[i].Koordinate);
                    writer.WriteElementString("Gerät", dataWrite[i].Gerät);
                    writer.WriteElementString("Hersteller", dataWrite[i].Hersteller);
                    writer.WriteElementString("Geräte_Beschreibung", dataWrite[i].gerätBeschreibung);

                    //Eplan Daten
                    writer.WriteStartElement("EPLAN_DATA");
                    writer.WriteElementString("Status_Erstellt", ("0"));
                    writer.WriteElementString("Erstellungsdatum", ("0"));
                    writer.WriteElementString("Makroname", ("0"));
                    writer.WriteElementString("Bemerkungen", ("0"));
                    writer.WriteEndElement(); //End Eplan_Data
                    writer.WriteStartElement("Signale");
                    do

                    {        //solange TAG gleich ist wie davor     
                        writer.WriteStartElement("Signal");
                        writer.WriteStartAttribute("Signal");
                        writer.WriteValue(dataWrite[i].Signal);
                        writer.WriteEndAttribute();
                        writer.WriteElementString("Signal", dataWrite[i].Signal);
                        writer.WriteElementString("Erzeugt", dataWrite[i].Erzeugt.ToShortDateString());
                        writer.WriteElementString("Signal_Aktiv", dataWrite[i].signalaktiv ? "1" : "0");
                        writer.WriteElementString("AS", dataWrite[i].AS);

                        writer.WriteStartElement("SignalKonfig"); // Signalkonfig
                        writer.WriteElementString("Anbindung", dataWrite[i].Anbindung);
                        writer.WriteElementString("Signalrichtung", dataWrite[i].Signalrichtung);
                        writer.WriteElementString("Wertebereich", dataWrite[i].Wertebereich);
                        writer.WriteElementString("Signalgrundtyp", dataWrite[i].Signalgrundtyp);
                        writer.WriteElementString("Signalphysik", dataWrite[i].Signalphysik);
                        writer.WriteElementString("Signalindex", dataWrite[i].Signalindex.ToString());
                        writer.WriteElementString("Safeposition", dataWrite[i].SafePosition);
                        writer.WriteElementString("Signalerweiterung", dataWrite[i].Signalerweiterung);
                        writer.WriteElementString("MessbereichMin", dataWrite[i].messbereichMin);
                        writer.WriteElementString("MessbereichMax", dataWrite[i].messbereichMax);
                        writer.WriteElementString("Einheit", dataWrite[i].messbereichEinheit);
                        writer.WriteEndElement(); //End

                        //Revision
                        writer.WriteStartElement("Revision");
                        writer.WriteElementString("RevEPC", dataWrite[i].RevEPC.ToString());
                        writer.WriteElementString("RevEPC_Komentar", dataWrite[i].RevEPCKommentar);
                        writer.WriteElementString("RevPMS", ((int)dataWrite[i].RevPMS).ToString());
                        writer.WriteElementString("RevPMS_Komentar", dataWrite[i].RevPMSKommentar);
                        writer.WriteElementString("RevACT", ((int)dataWrite[i].RevACT).ToString());
                        writer.WriteElementString("RevACT_Komentar", dataWrite[i].RevACTKommentar);
                        writer.WriteEndElement(); //End Revision

                        writer.WriteStartElement("HW_Anbindung"); // Hardwareanbindung
                        writer.WriteElementString("Busstrang", dataWrite[i].Busstrang);
                        writer.WriteStartElement("Profibus_PA"); // Profibus PA
                        writer.WriteElementString("SK3", ((int)dataWrite[i].SK3).ToString());
                        writer.WriteElementString("Trunk", ((int)dataWrite[i].Trunk).ToString());
                        writer.WriteElementString("Feldbarriere", ((int)dataWrite[i].Feldbarriere).ToString());
                        writer.WriteEndElement(); //End Profibus_PA
                        writer.WriteElementString("Kartentyp", dataWrite[i].Kartentyp);
                        writer.WriteElementString("SlaveTyp", dataWrite[i].SlaveTyp);
                        writer.WriteElementString("IO_Station", dataWrite[i].IO_Station);
                        writer.WriteElementString("RACK", dataWrite[i].RACK);
                        writer.WriteElementString("Steckplatz", dataWrite[i].Steckplatz);
                        writer.WriteElementString("Kanal", dataWrite[i].Kanal);
                        writer.WriteElementString("HW_Adresse", dataWrite[i].HW_Adresse);
                        writer.WriteElementString("PB_Adresse", dataWrite[i].PB_Adresse);
                        writer.WriteEndElement(); //End HW_Anbindung
                        writer.WriteStartElement("Software");
                        writer.WriteElementString("Verbalbeschreibung", dataWrite[i].Verbalbeschreibung);
                        writer.WriteElementString("Hysterese", dataWrite[i].Hysterese);
                        writer.WriteElementString("Reglerwirkung", dataWrite[i].Reglerwirkung);
                        writer.WriteElementString("StellgrenzeMin", dataWrite[i].StellgrenzeMin);
                        writer.WriteElementString("StellgrenzeMax", dataWrite[i].StellgrenzeMax);
                        writer.WriteElementString("AlarmUnten", dataWrite[i].AlarmUnten);
                        writer.WriteElementString("WarnungUnten", dataWrite[i].WarnungUnten);
                        writer.WriteElementString("ToleranzUnten", dataWrite[i].TolereanzUnten);
                        writer.WriteElementString("TolerenzOben", dataWrite[i].ToleranzOben);
                        writer.WriteElementString("WarnungOben", dataWrite[i].WarnungOben);
                        writer.WriteElementString("AlarmOben", dataWrite[i].AlarmOben);
                        writer.WriteElementString("SignalBedeutungFalse", dataWrite[i].BedeutungFalse);
                        writer.WriteElementString("SignalBedeutungTrue", dataWrite[i].BedeutungTrue);
                        writer.WriteElementString("Reverse", dataWrite[i].revers);
                        writer.WriteEndElement(); //End Software                        
                        writer.WriteEndElement(); //signal
                        i++;
                        if (i == dataWrite.Length)//Abbruch wenn am letzten Element angekommen wurde
                        {
                            break;
                        }
                    }
                    while (dataWrite[i].TAG_NAME == dataWrite[i - 1].TAG_NAME);
                    writer.WriteEndElement(); // End Signale                                           
                    writer.WriteEndElement();
                }
                catch (Exception)
                {
                    MessageBox.Show("Es ist ein Fehler beim XML erstellen aufgetreten!");
                    i++;
                }

            } while (i < (dataWrite.Length));

            // Endtag des Stammelements

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
            MessageBox.Show(String.Format(@"Datei {0} erzeugt.", OutputFile));
            System.Diagnostics.Process.Start(OutputFile);

        }

        private void XMLVergleichWrite (ComosDataImport[] dataWrite, string outputFile)
        {



            string schemaXSD = "ComosToXML.xsd";
            const string ns = "http://www.w3.org/2001/XMLSchema-instance";

            this.Title = "XML-Datei wird geschrieben.";
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  "; // 2 Leerzeichen
            XmlWriter writer = XmlWriter.Create(@outputFile, settings);
            writer.WriteStartDocument();

            this.Title = "XML wird exportiert";

            //XML schreiben
            // Starttag des Stammelements

            writer.WriteStartElement("ComosToXML");
            writer.WriteAttributeString("xmlns", "xsi", "", ns);
            writer.WriteAttributeString("xsi", "noNamespaceSchemaLocation",
                  ns, schemaXSD);
          //   writer.WriteRaw(schemaXSD);

            // writer.WriteAttributeString("xsi","ComosDaten.xsd");
            writer.WriteComment(String.Format("Diese Datei wurde aus {0} mit XmlWriter am {1} erzeugt", tbImportExcel.Text, DateTime.Now));

            int i = 0;
            do
            {
                try
                {
                    // Starttag von 'COMOS'
                    writer.WriteStartElement("DATA");
                    writer.WriteStartAttribute("Signal");
                    writer.WriteValue(dataWrite[i].Signal);
                    writer.WriteEndAttribute();
                    writer.WriteElementString("RevEPC", dataWrite[i].RevEPC.ToString());
                    writer.WriteElementString("RevEPC_Komentar", dataWrite[i].RevEPCKommentar);
                    writer.WriteElementString("RevPMS", ((int)dataWrite[i].RevPMS).ToString());
                    writer.WriteElementString("RevPMS_Komentar", dataWrite[i].RevPMSKommentar);
                    writer.WriteElementString("RevACT", ((int)dataWrite[i].RevACT).ToString());
                    writer.WriteElementString("RevACT_Komentar", dataWrite[i].RevACTKommentar);
                    writer.WriteElementString("MSR_Status", dataWrite[i].aktiv ? "1" : "0");
                    writer.WriteElementString("Funktion_erzeugt_am", dataWrite[i].Erzeugt.ToShortDateString());
                    writer.WriteElementString("Funktion", dataWrite[i].Funktion);
                    writer.WriteElementString("Tag_NR", dataWrite[i].Tag_NR);
                    writer.WriteElementString("TAG_NAME", dataWrite[i].TAG_NAME);
                    writer.WriteElementString("TAG_NAME_ACT", dataWrite[i].TAG_NAME_ACT);
                    writer.WriteElementString("Teilanlage", dataWrite[i].PL);
                    writer.WriteElementString("Beschreibung", dataWrite[i].Beschreibung);
                    writer.WriteElementString("Ebene", dataWrite[i].Ebene);
                    writer.WriteElementString("XYZ_Koordinate", dataWrite[i].Koordinate);
                    writer.WriteElementString("Hersteller", dataWrite[i].Hersteller);
                    writer.WriteElementString("Typ_Gerät", dataWrite[i].Gerät);
                    writer.WriteElementString("Geräte_Beschreibung", dataWrite[i].gerätBeschreibung);
                    writer.WriteElementString("Anbindung", dataWrite[i].Anbindung);
                    writer.WriteElementString("Signalrichtung", dataWrite[i].Signalrichtung);
                    writer.WriteElementString("Wertebereich", dataWrite[i].Wertebereich);
                    writer.WriteElementString("Signalphysik", dataWrite[i].Signalphysik);
                    writer.WriteElementString("Safeposition", dataWrite[i].SafePosition);
                    writer.WriteElementString("SW_Typical", dataWrite[i].SW_Typical);
                    writer.WriteElementString("Signal_erzeugt_am", dataWrite[i].Signal_Erzeugt.ToLongDateString());
                    writer.WriteElementString("Signalerweiterung", dataWrite[i].Signalerweiterung);
                    writer.WriteElementString("Signal_Status", dataWrite[i].signalaktiv ? "aktiv" : "löschen");
                    writer.WriteElementString("Signalbezeichnung", dataWrite[i].Signal);
                    writer.WriteElementString("Verbalbeschreibung", dataWrite[i].Verbalbeschreibung);
                    writer.WriteElementString("PLS_Signal", "");
                    writer.WriteElementString("Signalgrundtyp", dataWrite[i].Signalgrundtyp);
                    writer.WriteElementString("MB_min", dataWrite[i].messbereichMin);
                    writer.WriteElementString("MB_max", dataWrite[i].messbereichMax);
                    writer.WriteElementString("Einheit_Messbereich", dataWrite[i].messbereichEinheit);
                    writer.WriteElementString("Hysterese", dataWrite[i].Hysterese);
                    writer.WriteElementString("Reglerwirkung", dataWrite[i].Reglerwirkung);
                    writer.WriteElementString("StellgrenzeMin", dataWrite[i].StellgrenzeMin);
                    writer.WriteElementString("StellgrenzeMax", dataWrite[i].StellgrenzeMax);
                    writer.WriteElementString("AlarmUnten", dataWrite[i].AlarmUnten);
                    writer.WriteElementString("WarnungUnten", dataWrite[i].WarnungUnten);
                    writer.WriteElementString("ToleranzUnten", dataWrite[i].TolereanzUnten);
                    writer.WriteElementString("TolerenzOben", dataWrite[i].ToleranzOben);
                    writer.WriteElementString("WarnungOben", dataWrite[i].WarnungOben);
                    writer.WriteElementString("AlarmOben", dataWrite[i].AlarmOben);
                    writer.WriteElementString("SignalBedeutungFalse", dataWrite[i].BedeutungFalse);
                    writer.WriteElementString("SignalBedeutungTrue", dataWrite[i].BedeutungTrue);
                    writer.WriteElementString("Reverse", dataWrite[i].revers);
                    writer.WriteElementString("Busstrang", dataWrite[i].Busstrang);
                    writer.WriteElementString("SK3", ((int)dataWrite[i].SK3).ToString());
                    writer.WriteElementString("Trunk", ((int)dataWrite[i].Trunk).ToString());
                    writer.WriteElementString("Feldbarriere", ((int)dataWrite[i].Feldbarriere).ToString());
                    writer.WriteElementString("AS_Zuordnung", dataWrite[i].AS);
                    writer.WriteElementString("IO_Station", dataWrite[i].IO_Station);
                    writer.WriteElementString("RACK", dataWrite[i].RACK);
                    writer.WriteElementString("Steckplatz", dataWrite[i].Steckplatz);
                    writer.WriteElementString("Kanal", dataWrite[i].Kanal);
                    writer.WriteElementString("HW_Adresse", dataWrite[i].HW_Adresse);
                    writer.WriteElementString("PB_Adresse", dataWrite[i].PB_Adresse);
                    writer.WriteElementString("Kartentyp", dataWrite[i].Kartentyp);
                    writer.WriteElementString("SlaveTyp", dataWrite[i].SlaveTyp);
                    writer.WriteElementString("Ausbaustufe", ((int)dataWrite[i].Ausbaustufe).ToString());                                              
                    writer.WriteEndElement();
                    i++;
                }
                catch (Exception)
                {
                    MessageBox.Show("Es ist ein Fehler beim XML erstellen aufgetreten!");
                    i++;
                }

            } while (i < (dataWrite.Length));

            // Endtag des Stammelements

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
            MessageBox.Show(String.Format(@"Datei {0} erzeugt.", outputFile));
            System.Diagnostics.Process.Start(outputFile);      

        }

        /// <summary>
        /// Import des Eplan Inhaltsverzeichnis nach XML
        /// </summary>
        /// <remarks>
        /// Filter müssen angepasst werden!!
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenFileEplan_Click(object sender, RoutedEventArgs e)
        {
            Filetypen selectFileTyp = new Filetypen();
            selectFileTyp.filter = "Excel";
            string fileEplan = selectFileFromDiskFilter(selectFileTyp, @"c:\Daten\");
            this.Title = "Eplan Inhaltsverzeichnis ist gewählt";

            // Prüfen ob Datei existiert
            if (string.IsNullOrEmpty(fileEplan) || !File.Exists(fileEplan))
                MessageBox.Show("Es wurde keine Datei gewählt");
            else
            {
                Excel.Application xlApp;
                Excel.Workbook xlWorkBook;
                Excel.Worksheet xlWorkSheet;
                Excel.Range range;
                object misValue = Missing.Value;
                // Prüfen ob Datei existiert

                Title = "Exceldatei wird ausgelesen";
                xlApp = new Excel.Application();
                xlWorkBook = xlApp.Workbooks.Open(fileEplan, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                range = xlWorkSheet.UsedRange;

                // Größe des Arrys festlegen
                EplanDataContent[] importEplan;
                importEplan = new EplanDataContent[range.Rows.Count + 1];
                // Daten einlesen
                for (int i = 6; i <= range.Rows.Count; i++)
                {
                    // Zugriff auf jede Zeile
                    if ((range.Cells[i, 1] as Excel.Range).Value2 != null)
                    {
                        try
                        {
                            // Variante mit importEplan
                            importEplan[i].Anlage = (string)(range.Cells[i, 2] as Excel.Range).Value2;
                            importEplan[i].Einbauort = (string)(range.Cells[i, 3] as Excel.Range).Value2;
                            importEplan[i].Struktur = (string)(range.Cells[i, 4] as Excel.Range).Value2;
                            importEplan[i].Seitenname = (string)(range.Cells[i, 5] as Excel.Range).Value2;
                            importEplan[i].Seitenbeschreibung = (string)(range.Cells[i, 7] as Excel.Range).Value2;
                            importEplan[i].Änderungsdatum = (string)(range.Cells[i, 8] as Excel.Range).Value2;
                            importEplan[i].Erstellungsdatum = (string)(range.Cells[i, 9] as Excel.Range).Value2;
                            importEplan[i].Makroname = (string)(range.Cells[i, 10] as Excel.Range).Value2;
                            importEplan[i].Makrobeschreibung = (string)(range.Cells[i, 11] as Excel.Range).Value2;
                            importEplan[i].Bemerkungen = (string)(range.Cells[i, 12] as Excel.Range).Value2;
                        }
                        catch (Exception)
                        {
                            MessageBox.Show(string.Format("Fehler beim einlesen der Exceldatei Datensatz {0}!", i));
                        }
                    }
                }
                // Excel beenden
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();
                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);
                Title = "Exceldatei ist geschlossen";

                //Filtern
                EplanDataContent[] filterDataEplan;
                filterDataEplan = new EplanDataContent[importEplan.Length];
                int j = 0;
                foreach (EplanDataContent temp in importEplan)
                {
                    if ((temp.Struktur != "BFS") & (temp.Struktur != null))
                    {
                        //Anlagenfilter entfernt
                        //if ((temp.Anlage == "PL21") | (temp.Anlage == "PL22") | (temp.Anlage == "PL23") | (temp.Anlage == "PL30") | (temp.Anlage == "PL31") | (temp.Anlage == "PL15") | (temp.Anlage == "PL41") | (temp.Anlage == "PL42") | (temp.Anlage == "PL43") | (temp.Anlage == "PL44") | (temp.Anlage == "PL45") | (temp.Anlage == "PL46"))
                        //{
                            filterDataEplan[j].Anlage = temp.Anlage;
                            filterDataEplan[j].Einbauort = temp.Einbauort;
                            filterDataEplan[j].Struktur = temp.Struktur;
                            filterDataEplan[j].Seitenname = temp.Seitenname;
                            filterDataEplan[j].Seitenbeschreibung = temp.Seitenbeschreibung;
                            filterDataEplan[j].Änderungsdatum = temp.Änderungsdatum;
                            filterDataEplan[j].Erstellungsdatum = temp.Erstellungsdatum;
                            filterDataEplan[j].Makroname = temp.Makroname;
                            filterDataEplan[j].Makrobeschreibung = temp.Makrobeschreibung;
                            filterDataEplan[j].Bemerkungen = temp.Bemerkungen;
                            j++;
                        //}
                    }
                }
                // zählen wie viele Elemente beschrieben sind
                int g = 0;
                foreach (EplanDataContent temp in filterDataEplan)
                {
                    if (temp.Anlage != null)
                    {
                        g++;
                    }
                }
                // Erstellen des Temp-Elementes
                EplanDataContent[] filterDataEplan_2;
                filterDataEplan_2 = new EplanDataContent[g];

                // Elemente in neue Struktur umkopieren
                for (int i = 0; i < g; i++)
                {
                    try
                    {
                        filterDataEplan_2[i] = filterDataEplan[i];
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(string.Format("Fehler beim einlesen der Exceldatei Datensatz {0}!", i));
                    }
                }

                string XMLFile = (string.Format(outputDirectory + "\\Eplan_Inhaltsverzeichnis_{0}{1}{2}.xml", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
                XMLwriteEplan(filterDataEplan_2, fileEplan, XMLFile);
                tbopenEplanContent.Text = XMLFile;

            }

        }

        // Eplan Inhaltsverzeichnis in XML schreiben
        private void XMLwriteEplan(EplanDataContent[] dataWrite, string inputFile, string outputFile)
        {
            this.Title = "XML-Datei wird geschrieben.";
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  "; // 2 Leerzeichen
            XmlWriter writer = XmlWriter.Create(@outputFile, settings);
            writer.WriteStartDocument();

            this.Title = "Eplan XML wird exportiert";

            //XML schreiben
            // Starttag des Stammelements
            writer.WriteStartElement("Eplan_Inhalt");
            writer.WriteComment(String.Format("Diese Datei wurde aus {0} mit XmlWriter am {1} erzeugt", inputFile, DateTime.Now));

            int i = 0;
            do
            {
                try
                {
                    // Starttag von 'TAG'
                    writer.WriteStartElement("MSR_Gerät");
                    writer.WriteStartAttribute("TAG");
                    writer.WriteValue(dataWrite[i].Struktur + dataWrite[i].Anlage.Substring((dataWrite[i].Anlage.Length > 0 ? dataWrite[i].Anlage.Length - 2 : 0), 2) + dataWrite[i].Seitenname);
                    writer.WriteEndAttribute();
                    writer.WriteElementString("Geräte_Bezeichnung", dataWrite[i].Struktur + dataWrite[i].Anlage.Substring((dataWrite[i].Anlage.Length > 0 ? dataWrite[i].Anlage.Length - 2 : 0), 2) + dataWrite[i].Seitenname);
                    writer.WriteElementString("Anlage", dataWrite[i].Anlage);
                    writer.WriteElementString("Einbauort", dataWrite[i].Einbauort);
                    writer.WriteElementString("Struktur", dataWrite[i].Struktur);
                    writer.WriteElementString("Seitenname", dataWrite[i].Seitenname);
                    writer.WriteElementString("Seitenbeschreibung", dataWrite[i].Seitenbeschreibung);
                    writer.WriteElementString("Änderungsdatum", dataWrite[i].Änderungsdatum);

                    //Eplan Daten
                    writer.WriteStartElement("EPLAN_DATA");
                    writer.WriteElementString("Status_Erstellt", ("1"));
                    writer.WriteElementString("Erstellungsdatum", dataWrite[i].Erstellungsdatum);
                    writer.WriteElementString("Makroname", dataWrite[i].Makroname);
                    writer.WriteElementString("Makrobeschreibung", dataWrite[i].Makrobeschreibung);
                    writer.WriteElementString("Bemerkungen", dataWrite[i].Bemerkungen);
                    writer.WriteEndElement(); //End Eplan_Data            
                    writer.WriteEndElement();
                    i++;
                }
                catch (Exception)
                {
                    MessageBox.Show("Es ist ein Fehler beim XML erstellen aufgetreten!");
                    i++;
                }

            } while (i < (dataWrite.Length));

            // Endtag des Stammelements
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
            MessageBox.Show(String.Format(@"Datei {0} erzeugt.", outputFile));
            System.Diagnostics.Process.Start(outputFile);
        }




    }

}