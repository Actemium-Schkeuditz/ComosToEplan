using System.IO;
using System.Windows;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Xml;
using System.Linq;
using System.Xml.XPath;

namespace ComosToEplan
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        
        string loadFilePAConfig = @"..\..\XML\ConfigPAKaesten.xml";
       string writeFilePAConfig = @"..\..\XML\ConfigPAKaesten.xml";
        string loadFileGeräteConfig = @"..\..\XML\ConfigGeräte.xml";
        string loadFileRioConfig = @"..\..\XML\ConfigRioDaten.xml";
        string loadFilePumpenConfig = @"..\..\XML\ConfigMembranPumpen.xml";

        /// <summary>
        /// PA-Kästen Konfiguartion
        /// </summary>
        MSRKaesten dataPAConfig = new MSRKaesten();
        
        MSRGeraete dataGeräteConfig = new MSRGeraete();

        MSRGeraete dataPumpenConfig = new MSRGeraete();

        /// <summary>
        /// Konfiguration der ET200isp
        /// </summary>
        MSRKaesten dataRioConfig = new MSRKaesten();

        // string loadFile = @"C:\Users\clangrock\Documents\Visual Studio 2015\Projects\Wpf_ExcelTest\Wpf_ExcelTest\XML\ConfigPAKaesten.xml";
        /// <summary>
        /// Laden der PA Configuration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReadConfigPA_Click(object sender, RoutedEventArgs e)
        {

            // Prüfen ob schon Daten geladen sind, wenn ja dann erst speichern
            if (dGconfigPaKaesten.SelectedIndex >= 0)
            {
                writePAConfig();
            }
                // Aufruf Hilfsfunktion zu laden der PA Config Daten
                loadPaConfig();
            
        }

        /// <summary>
        /// Speichern der PA config
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPAConfigSave_click(object sender, RoutedEventArgs e)
        {
            writePAConfig();
        }

        private void dGconfigPaKaestenEdit_click(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {
            //Daten schreiben
            writePAConfig();
        }

        /// <summary>
        /// Neuen Datensatz anlegen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPAConfigAddRow_click(object sender, RoutedEventArgs e)
        {

            // erst Daten sichern        
            writePAConfig();
            // Datei öffnen
            XElement root = XElement.Load(loadFilePAConfig);
            // Punkt in XML suchen wo die Änderung rein soll
            XElement name = root.Element("PABestueckung");//root.Descendants("Name");
            //Elemente festlegen
             name.Add( new XElement("BMKPA",                                              
                           new XElement("BMK", "=PL??+?m.???"),
                           new XElement("anzahlFeldbarrieren", "0"),
                           new XElement("anzahlPAPt100", "0"),
                           new XElement("anzahlSensoranschaltung", "0")
                       )

                   );
            //Datei schreiben
            root.Save(writeFilePAConfig);

            // Aufruf Hilfsfunktion zum neu laden der Daten
            loadPaConfig();
        }


        private void btnPAConfigEraseRow_click(object sender, RoutedEventArgs e)
        {
            erasePaConfigRow();
        }

       
        private void btnGeräteConfigRead_Click(object sender, RoutedEventArgs e)
        {

            // Prüfen ob schon Daten geladen sind, wenn ja dann erst speichern
            if (dGconfigGeräte.SelectedIndex >= 0)
            {
                writeMSRGeräteConfigToXml(loadFileGeräteConfig, dGconfigGeräte.DataContext);
            }
            // Aufruf Hilfsfunktion zu laden der PA Config Daten
            loadGeräteConfig();
      
        }

        private void btnGeräteConfigAddRow_click(object sender, RoutedEventArgs e)
        {

            // erst Daten sichern        
            writeMSRGeräteConfigToXml(loadFileGeräteConfig, dGconfigGeräte.DataContext);
            // Datei öffnen
            XElement root = XElement.Load(loadFileGeräteConfig);
            // Punkt in XML suchen wo die Änderung rein soll
            XElement name = root.Element("Feldgerraete");//root.Descendants("Name");
                                                          //Elemente festlegen
            name.Add(new XElement("Gerät",
                          new XElement("Hersteller", "xyz"),
                          new XElement("Gerätetyp", ""),
                          new XElement("Bestellcode", ""),
                          new XElement("Beschreibung", ""),
                          new XElement("Anschluß_Plus", "+"),
                          new XElement("Anschluß_Minus", "-")
                      )
                  );
            //Datei schreiben
            root.Save(loadFileGeräteConfig);

            // Aufruf Hilfsfunktion zum neu laden der Daten
            loadGeräteConfig();
        }

        private void btnGeräteConfigEraseRow_click(object sender, RoutedEventArgs e)
        {
            eraseGeräteConfigRow();
        }

        private void btnGeräteConfigSave_click(object sender, RoutedEventArgs e)
        {
            //Daten schreiben
            writeMSRGeräteConfigToXml(loadFileGeräteConfig, dGconfigGeräte.DataContext);
        }

        private void dGconfigGeräteEdit_click(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {
            //Daten schreiben
            writeMSRGeräteConfigToXml(loadFileGeräteConfig, dGconfigGeräte.DataContext);
        }


        /// <summary>
        /// Funktionen für dei Verwalteung der PA-Kästen
        /// </summary>

        private void loadPaConfig()
        {
            try
            {
                // Prüfen ob es die Datei gibt
                if (string.IsNullOrEmpty(loadFilePAConfig) || !File.Exists(loadFilePAConfig))
                    MessageBox.Show("Es wurde keine Datei gewählt");
                else
                {
                    // einlesen der Config
                    this.Title = "PA Konfiguartion wird eingelesen";
                    //Datei laden             

                    XmlSerializer serializer = new XmlSerializer(typeof(MSRKaesten));
                    FileStream fs = new FileStream(loadFilePAConfig, FileMode.Open);
                    dataPAConfig = (MSRKaesten)serializer.Deserialize(fs);

                    //Config Datei schliessen                
                    fs.Close();

                    this.Title = "Übergabe an DataGrid";
                    // Weitergabe an DataGrid
                    dGconfigPaKaesten.DataContext = dataPAConfig;
                    dGconfigPaKaesten.ItemsSource = dataPAConfig.PABestueckung;

                    btnAddRowPAConfig.Visibility = Visibility.Visible;
                    btnAddRowPASave.Visibility = Visibility.Visible;
                    btnEraseRowPAConfig.Visibility = Visibility.Visible;
                }
            }
            catch
                {
                    MessageBox.Show(string.Format("Die Datei {0} konnte nicht geöffnet werden!", loadFilePAConfig));
                }
        }

        private void erasePaConfigRow()
        {
            try
            {
            int index = dGconfigPaKaesten.SelectedIndex;

            if (index >= 0)
            {

                // erst Daten sichern        
                writePAConfig();
                // Datei öffnen
                XmlDocument doc = new XmlDocument();
                doc.Load(loadFilePAConfig);
                //Navigazor benutzen
                XPathNavigator navigator = doc.CreateNavigator();
                // in die richtige Ebene gehen
                navigator.MoveToFirstChild();               
                navigator.MoveToFirstChild();               
                navigator.MoveToFirstChild();
               //Durchzählen bis zum richtigen Element
                for (int i = 0; i < index; i++)
                {
                    navigator.MoveToNext();
                }
                //Datensatz löschen
                navigator.DeleteSelf();
                //Datei schreiben
                doc.Save(writeFilePAConfig);

                // Aufruf Hilfsfunktion zum neu laden der Daten
                loadPaConfig();
            }
            else
                MessageBox.Show("Es wurde kein Datensatz ausgewählt!");
            }
            catch
            {
                MessageBox.Show("Fehler beim löschen eines Datensatzes!");
            }
        }

        /// <summary>
        /// Schreiben der PA-Config der Kästen
        /// </summary>
        private void writePAConfig()
        {
            // löschen der Datei damit alles neu geschrieben wird
            // sonst sind Datenreste drin
            try
            {
                if (File.Exists(writeFilePAConfig))
                {
                    File.Delete(writeFilePAConfig);
                }
                // Ausgabe Datei einrichten
                XmlSerializer serializer = new XmlSerializer(typeof(MSRKaesten));
                // Konfig schreiben
                FileStream fs = new FileStream(writeFilePAConfig, FileMode.OpenOrCreate);

                serializer.Serialize(fs, dGconfigPaKaesten.DataContext);
                //Config Datei schliessen        
                fs.Flush();
                fs.Close();
            }
            catch
            {
                MessageBox.Show(string.Format("Fehler beim Schreiben der PA-Config {0}", writeFilePAConfig));
            }
        }

        private void writeRioConfig()
        {
            // löschen der Datei damit alles neu geschrieben wird
            // sonst sind Datenreste drin
            try
            {
                if (File.Exists(loadFileRioConfig))
                {
                    File.Delete(loadFileRioConfig);
                }
                // Ausgabe Datei einrichten
                XmlSerializer serializer = new XmlSerializer(typeof(MSRKaesten));
                // Konfig schreiben
                FileStream fs = new FileStream(loadFileRioConfig, FileMode.OpenOrCreate);

                serializer.Serialize(fs, dGconfigRioKaesten.DataContext);
                //Config Datei schliessen        
                fs.Flush();
                fs.Close();
            }
            catch
            {
                MessageBox.Show(string.Format("Fehler beim Schreiben der RIO-Config {0}", loadFileRioConfig));
            }
        }

        private void eraseRioConfigRow()
        {
            try
            {
                int index = dGconfigRioKaesten.SelectedIndex;

                if (index >= 0)
                {

                    // erst Daten sichern        
                    writeRioConfig();
                   
                    // Datei öffnen
                    XmlDocument doc = new XmlDocument();
                    doc.Load(loadFileRioConfig);
                    //Navigazor benutzen
                    XPathNavigator navigator = doc.CreateNavigator();
                    // in die richtige Ebene gehen
                    navigator.MoveToFirstChild();
                    navigator.MoveToFirstChild();
                    navigator.MoveToFirstChild();
                    //Durchzählen bis zum richtigen Element
                    for (int i = 0; i < index; i++)
                    {
                        navigator.MoveToNext();
                    }
                    //Datensatz löschen
                    navigator.DeleteSelf();
                    //Datei schreiben
                    doc.Save(loadFileRioConfig);

                    // Aufruf Hilfsfunktion zum neu laden der Daten
                    loadRioConfig();
                }
                else
                    MessageBox.Show("Es wurde kein Datensatz ausgewählt!");
            }
            catch
            {
                MessageBox.Show("Fehler beim löschen eines Datensatzes!");
            }
        }

        /// <summary>
        /// Funktionen für die Geräte Verwaltung
        /// </summary>
        /// <returns>Rückgabe ob laden erfolgreich</returns>

        private bool loadGeräteConfig()
        {
            string filename = loadFileGeräteConfig;
            // Prüfen ob es die Datei gibt
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
            {
                MessageBox.Show("Es wurde keine Datei gewählt");
                return false;
            }
            else
            {
                try
                {
                    //MSRGeraete dataConfig = new MSRGeraete();
                    // einlesen der Config
                    this.Title = "Geräte Konfiguartion wird eingelesen";
                    //Datei laden             

                    dataGeräteConfig = readXmlMSRGeraeteConfig(filename);

                    if (dataGeräteConfig.Feldgerraete.Count() >= 1)
                    {
                        this.Title = "Übergabe an DataGrid";
                        // Weitergabe an DataGrid
                        dGconfigGeräte.DataContext = dataGeräteConfig;
                        dGconfigGeräte.ItemsSource = dataGeräteConfig.Feldgerraete;

                        //Geräte Config Buttons
                        btnAddRowGeräteConfig.Visibility = Visibility.Visible;
                        btnAddRowGeräteSave.Visibility = Visibility.Visible;
                        btnEraseRowGeräteConfig.Visibility = Visibility.Visible;

                        return true;
                    }
                    else
                        return false;
                }
                catch
                {
                    MessageBox.Show(string.Format("Die Datei {0} konnte nicht geöffnet werden!", filename));
                    return false;
                }
            }

        }

        private bool loadPumpenConfig(string filename)
        {
          // string filename = loadFilePumpenConfig;
            // Prüfen ob es die Datei gibt
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
            {
                MessageBox.Show("Es wurde keine Datei gewählt");
                return false;
            }
            else
            {
                try
                {               
                    // einlesen der Config
                    this.Title = "Geräte Konfiguartion wird eingelesen";
                    //Datei laden             

                    MapperXMLToTreeview loadXMLData = new MapperXMLToTreeview(filename);
                    loadXMLData.LoadXml(tvConfigPumpen);

                    // Daten für Filter laden
                    dataPumpenConfig  = readXmlMSRGeraeteConfig(filename);

                    if (tvConfigPumpen.HasItems)
                    {
                        this.Title = "Übergabe an DataGrid";
                        //Pumpen Config Buttons
                        btnConfigPumpenAddRow.Visibility = Visibility.Visible;
                        btnConfigPumpenSave.Visibility = Visibility.Visible;
                        btnEraseRowPumpenConfig.Visibility = Visibility.Visible;

                        return true;
                    }
                    else
                        return false;
                }
                catch
                {
                    MessageBox.Show(string.Format("Die Datei {0} konnte nicht geöffnet werden!", filename));
                    return false;
                }
            }

        }

        private MSRGeraete readXmlMSRGeraeteConfig(string filename)
        {
            // Prüfen ob es die Datei gibt
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
            {
                MessageBox.Show("Es wurde keine Datei gewählt");
                return null;
            }
            else
            {
                try
                {
                    MSRGeraete dataConfig = new MSRGeraete();
                    // einlesen der Config
                    this.Title = "Geräte Konfiguartion wird eingelesen";
                    //Datei laden             

                    XmlSerializer serializer = new XmlSerializer(typeof(MSRGeraete));
                    FileStream fs = new FileStream(filename, FileMode.Open);
                   dataConfig = (MSRGeraete)serializer.Deserialize(fs);
                    
                    //Config Datei schliessen                
                    fs.Close();
                    return dataConfig;                   
                }
                catch
                {
                    MessageBox.Show(string.Format("Die Datei {0} konnte nicht geöffnet werden!", filename));
                    return null;
                }
            }
        }

        private bool writeMSRGeräteConfigToXml(string filename, object geraeteData)
        {
            // löschen der Datei damit alles neu geschrieben wird
            // sonst sind Datenreste drin
            try
            {
                if (File.Exists(loadFileGeräteConfig))
                {
                    File.Delete(loadFileGeräteConfig);
                }
                // Ausgabe Datei einrichten
                XmlSerializer serializer = new XmlSerializer(typeof(MSRGeraete));
                // Konfig schreiben
                FileStream fs = new FileStream(filename, FileMode.OpenOrCreate);

                serializer.Serialize(fs, geraeteData);// dGconfigGeräte.DataContext);
                //Config Datei schliessen        
                fs.Flush();
                fs.Close();
                return true;
            }
            catch
            {
                MessageBox.Show(string.Format("Fehler beim Schreiben der PA-Config {0}", filename));
                return false;
            }
        }

        private void eraseGeräteConfigRow()
        {
            try
            {
                int index = dGconfigGeräte.SelectedIndex;

                if (index >= 0)
                {

                    // erst Daten sichern        
                    writeMSRGeräteConfigToXml(loadFileGeräteConfig, dGconfigGeräte.DataContext);
                    // Datei öffnen
                    XmlDocument doc = new XmlDocument();
                    doc.Load(loadFileGeräteConfig);
                    //Navigazor benutzen
                    XPathNavigator navigator = doc.CreateNavigator();
                    // in die richtige Ebene gehen
                    navigator.MoveToFirstChild();
                    navigator.MoveToFirstChild();
                    navigator.MoveToFirstChild();
                    //Durchzählen bis zum richtigen Element
                    for (int i = 0; i < index; i++)
                    {
                        navigator.MoveToNext();
                    }
                    //Datensatz löschen
                    navigator.DeleteSelf();
                    //Datei schreiben
                    doc.Save(loadFileGeräteConfig);

                    // Aufruf Hilfsfunktion zum neu laden der Daten
                    loadGeräteConfig();
                }
                else
                    MessageBox.Show("Es wurde kein Datensatz ausgewählt!");
            }
            catch
            {
                MessageBox.Show("Fehler beim löschen eines Datensatzes!");
            }
        }


        private void loadRioConfig()
        {
            // Prüfen ob es die Datei gibt
            if (string.IsNullOrEmpty(loadFileRioConfig) || !File.Exists(loadFileRioConfig))
                MessageBox.Show("Es wurde keine Datei gewählt");
            else
            {
                try
                {
                    // einlesen der Config
                    this.Title = "Geräte Konfiguartion wird eingelesen";
                    //Datei laden             

                    XmlSerializer serializer = new XmlSerializer(typeof(MSRKaesten));
                    FileStream fs = new FileStream(loadFileRioConfig, FileMode.Open);
                    dataRioConfig = (MSRKaesten)serializer.Deserialize(fs);

                    //Config Datei schliessen                
                    fs.Close();

                      this.Title = "Übergabe an DataGrid";     
                    dGconfigRioKaesten.DataContext = dataRioConfig;
                    dGconfigRioKaesten.ItemsSource = dataRioConfig.RemoteIO;            
                    //RIO Config Buttons
                    btnAddRowRioConfig.Visibility = Visibility.Visible;
                     btnAddRowRioSave.Visibility = Visibility.Visible;
                     btnEraseRowRioConfig.Visibility = Visibility.Visible;
                }
                catch
                {
                    MessageBox.Show(string.Format("Die Datei {0} konnte nicht geöffnet werden!", loadFileRioConfig));
                }
            }
        }


        private void writeConfigPL()
        {
            // löschen der Datei damit alles neu geschrieben wird
            // sonst sind Datenreste drin
            try
            {
                if (File.Exists(loadFilePLConfig))
                {
                    File.Delete(loadFilePLConfig);
                }
                // Ausgabe Datei einrichten
                XmlSerializer serializer = new XmlSerializer(typeof(ConfigPL));
                // Konfig schreiben
                FileStream fs = new FileStream(loadFilePLConfig, FileMode.OpenOrCreate);

                serializer.Serialize(fs, dGconfigPL.DataContext);
                //Config Datei schliessen        
                fs.Flush();
                fs.Close();
            }
            catch
            {
                MessageBox.Show(string.Format("Fehler beim Schreiben der PL-Config {0}", loadFilePLConfig));
            }
        }

        private void loadPLConfig()
        {
            try
            {
                // Prüfen ob es die Datei gibt
                if (string.IsNullOrEmpty(loadFilePLConfig) || !File.Exists(loadFilePLConfig))
                    MessageBox.Show("Es wurde keine Datei gewählt");
                else
                {
                    // einlesen der Config
                    this.Title = "PL Konfiguartion wird eingelesen";
                    //Datei laden             

                    XmlSerializer serializer = new XmlSerializer(typeof(ConfigPL));
                    FileStream fs = new FileStream(loadFilePLConfig, FileMode.Open);
                    dataPlListe = (ConfigPL)serializer.Deserialize(fs);

                    //Config Datei schliessen                
                    fs.Close();

                    this.Title = "Übergabe an DataGrid";
                    // Weitergabe an DataGrid
                    dGconfigPL.DataContext = dataPlListe;
                    dGconfigPL.ItemsSource = dataPlListe.Config;

                    // btnAddRowPAConfig.Visibility = Visibility.Visible;
                    // btnAddRowPASave.Visibility = Visibility.Visible;
                    // btnEraseRowPAConfig.Visibility = Visibility.Visible;
                }
            }
            catch
            {
                MessageBox.Show(string.Format("Die Datei {0} konnte nicht geöffnet werden!", loadFilePLConfig));
            }
        }



    }
}