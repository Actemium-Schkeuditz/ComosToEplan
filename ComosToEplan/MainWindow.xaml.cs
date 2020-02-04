using System.Windows;
using System.Configuration;
using System.Collections.Specialized;
using ComosToEplan.Properties;
using System;
using System.Linq;
using System.Xml.Linq;
using System.IO;

namespace ComosToEplan
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
      
        /// <summary>
        /// Initialisierung Oberfläche
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            //PA Config Buttons
            btnAddRowPAConfig.Visibility = Visibility.Hidden;
            btnAddRowPASave.Visibility = Visibility.Hidden;
            btnEraseRowPAConfig.Visibility = Visibility.Hidden;
            //Geräte Config Buttons
            btnAddRowGeräteConfig.Visibility = Visibility.Hidden;
            btnAddRowGeräteSave.Visibility = Visibility.Hidden;
            btnEraseRowGeräteConfig.Visibility = Visibility.Hidden;
            pBprogress.Visibility = Visibility.Hidden;

            //Pumpen Config Buttons
            btnConfigPumpenAddRow.Visibility = Visibility.Hidden;
            btnConfigPumpenSave.Visibility = Visibility.Hidden;
            btnEraseRowPumpenConfig.Visibility = Visibility.Hidden;

            //Rio Config Buttons
            btnAddRowRioConfig.Visibility = Visibility.Hidden;
            btnAddRowRioSave.Visibility = Visibility.Hidden;
            btnEraseRowRioConfig.Visibility = Visibility.Hidden;

            this.Title = "ComosToEplan";
            // Einstellungen aus Konfig laden
            loadSettingsfromConfig();
        }

        /// <summary>
        /// Festlegung Zielverzeichnis
        /// </summary>
        string outputDirectory = "";// (@"C:\Users\Public\Documents\ComosEplanVergleich\");

        private void loadSettingsfromConfig()
        {
            // <userSettings> und <applicationSettings> auswerten
            Settings setting = new Settings();
            outputDirectory = setting.outputDirectory;
            Console.WriteLine(outputDirectory);

        }

        private void progressbar(int fortschritt)
        {
            pBprogress.Value = fortschritt;

        }


        /// <summary>
        /// Auswahl der Datendatei für Import
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectComosFile_Click(object sender, RoutedEventArgs e)
        {
            Filetypen selectFileTyp = new Filetypen();
            selectFileTyp.filter = "Excel";
            tbImportExcel.Text = selectFileFromDiskFilter(selectFileTyp, @"c:\Daten\");
            this.Title = "Comos Datei ist gewählt";
        }


        /// <summary>
        /// Auswahl der Datendatei für Eplan / Comos Vergleich
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectFileEplanContent_Click(object sender, RoutedEventArgs e)
        {
            Filetypen selectFileTyp = new Filetypen();
            selectFileTyp.filter = "XML";   // einstellen Datei Typ
            tbopenEplanContent.Text = selectFileFromDiskFilter(selectFileTyp, @"C:\Users\Public\Documents\XML");
            this.Title = "XML Datei ist gewählt";
        }

        /// <summary>
        /// Auswahl der Datendatei für Eplan / Comos Vergleich
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnSelectComosContent_Click(object sender, RoutedEventArgs e)
        {
            Filetypen selectFileTyp = new Filetypen();
            selectFileTyp.filter = "XML"; // einstellen Datei Typ
            tbopenComosContent.Text = selectFileFromDiskFilter(selectFileTyp, @"C:\Users\Public\Documents\XML");
            this.Title = "XML Datei ist gewählt";
        }


        /// <summary>
        /// Vergleicher Datei Auswahl
        /// </summary>
        private void btnSelectComosContent_Old_Click(object sender, RoutedEventArgs e)
        {
            Filetypen selectFileTyp = new Filetypen();
            selectFileTyp.filter = "XML"; // einstellen Datei Typ
            tbopenComosContent_Old.Text = selectFileFromDiskFilter(selectFileTyp, outputDirectory);//@"C:\Users\Public\Documents\XML");
            this.Title = "Alte Comos XML Datei ist gewählt";
        }

        /// <summary>
        /// Auswahl Comos Datei für Vergleich
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnSelectComosContent_New_Click(object sender, RoutedEventArgs e)
        {
            Filetypen selectFileTyp = new Filetypen();
            selectFileTyp.filter = "XML"; // einstellen Datei Typ
            tbopenComosContent_New.Text = selectFileFromDiskFilter(selectFileTyp, outputDirectory);
            this.Title = "Neue Comos XML Datei ist gewählt";
        }

        /// <summary>
        /// Auswahl Datendatei für Exoprt nach Eplan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnselectComosToEplan_Click(object sender, RoutedEventArgs e)
        {
            Filetypen selectFileTyp = new Filetypen();
            selectFileTyp.filter = "XML"; // einstellen Datei Typ
           tbopenComosToEplan.Text = selectFileFromDiskFilter(selectFileTyp, outputDirectory);// @"C:\Users\Public\Documents\XML");
            this.Title = "Comos Daten für Export ist gewählt";


            loadPLConfig();
        }

        /// <summary>
        /// neu erstellen der Prozesslinien Auswahl
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReadPLConfigNew_Click(object sender, RoutedEventArgs e)
        {
            // Datei holen
            string loadFile = tbopenComosToEplan.Text;
            // Prüfen ob Dateien existiert
            if (string.IsNullOrEmpty(loadFile) || !File.Exists(loadFile))
                MessageBox.Show("Es wurde keine Datei gewählt");
            else
            {
                // Filtern nach allem was aktuell ist               
                var filterGeräteAktiv =
                from geräteAktiv in xmlComosLoad(tbopenComosToEplan.Text).MSR_Gerät//dataComos.MSR_Gerät 
                where geräteAktiv.Aktiv == 1 && geräteAktiv.Ausbaustufe == 1
                select geräteAktiv;

                // alle PL suchen und ausgeben die in der Config enthalten sind
                var listPL =
                    from tempPL in filterGeräteAktiv    //dataOutput.MSR_Gerät
                orderby tempPL.PL
                    group tempPL by tempPL.PL into plList
                // select plList.Key;
                select new
                    {
                        Linien = plList.Key,
                        AnzahlGeräte = plList.Count(),
                        aktiv = false,
                    };               
                // Datei PL erstellen
                // Datei öffnen
                XElement root = XElement.Load(loadFilePLConfig);
                // Punkt in XML suchen wo die Änderung rein soll
                XElement name = root.Element("Config");

                //löschen der alten Daten
                name.RemoveAll();

                //Elemente festlegen
                foreach (var temp in listPL)
                {
                    name.Add(new XElement("PL",
                    new XElement("Linie", temp.Linien),
                    new XElement("AnzahlGeräte", temp.AnzahlGeräte),
                   new XElement("Aktiv", false)
                              )
                          );
                    //Datei schreiben
                    root.Save(loadFilePLConfig);
                }

                // neue Daten laden
                loadPLConfig();
            }
        }


        /// <summary>
        /// Anwendung beenden
        /// </summary>
        private void btn_close_Click(object sender, RoutedEventArgs e)
        {

            MessageBoxResult result = MessageBox.Show("Soll wirklich das Programm beendet werden?", "Dialog Title", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void dGconfigPLEdit_click(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {
            writeConfigPL();
        }

        private void btnReadRioConfig_Click(object sender, RoutedEventArgs e)
        {

            // Prüfen ob schon Daten geladen sind, wenn ja dann erst speichern
            if (dGconfigRioKaesten.SelectedIndex >= 0)
            {
                writeRioConfig();   
            }
            // Aufruf Hilfsfunktion zu laden der PA Config Daten
            loadRioConfig();

            // MapperXMLToTreeview loadXMLData = new MapperXMLToTreeview(loadFileRioConfig);
            // loadXMLData.LoadXml(tvConfigRio);
        }

        private void btnAddRowRioConfig_Click(object sender, RoutedEventArgs e)
        {
            // erst Daten sichern        
            writeRioConfig();
            // Datei öffnen
            XElement root = XElement.Load(loadFileRioConfig);
            // Punkt in XML suchen wo die Änderung rein soll
            XElement name = root.Element("RemoteIO");//root.Descendants("Name");
                                                          //Elemente festlegen
            name.Add(new XElement("RIO",
                          new XElement("Typ", "ET200isp"),
                          new XElement("BMK","=PL??+?m.UF??"),
                          new XElement("Networktyp", "RS485-IS"),
                          new XElement("Master", "AS?"),
                          new XElement("Adresse", "0"),
                          new XElement("Subnet", "0"),
                          new XElement("Ventilinsel", "true"),
                          new XElement("ersterSteckplatzVentileInsel", "0")
                      )

                  );
            //Datei schreiben
            root.Save(loadFileRioConfig);

            // Aufruf Hilfsfunktion zum neu laden der Daten
            loadRioConfig();
        }

        private void dGconfigRioKaesten_CellEditEnding(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {
            writeRioConfig();
        }

        private void btnAddRowRioSave_Click(object sender, RoutedEventArgs e)
        {
            writeRioConfig();
        }

        private void btnEraseRowRioConfig_Click(object sender, RoutedEventArgs e)
        {
            eraseRioConfigRow();
        }

        private void dGconfigPumpenEdit_click(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {

        }

        private void btnReadConfigPumpen_Click(object sender, RoutedEventArgs e)
        {
            // Prüfen ob schon Daten geladen sind, wenn ja dann erst speichern
            if (tvConfigPumpen.HasItems)
            {
                //speichern der Config
                //writeMSRGeräteConfigToXml(loadFilePumpenConfig , dGconfigPumpen);
            }
            // Aufruf Hilfsfunktion zu laden der PA Config Daten
            loadPumpenConfig(loadFilePumpenConfig);
        }

        private void btnPumpenConfigAddRow_click(object sender, RoutedEventArgs e)
        {

        }

        private void btnPumpenConfigEraseRow_click(object sender, RoutedEventArgs e)
        {

        }

        private void btnPumpenConfigSave_click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAuswahlConfigFürFehlersuche_Click(object sender, RoutedEventArgs e)
        {

            Filetypen selectFileTyp = new Filetypen();
            selectFileTyp.filter = "XML";
            tbFehlersuche.Text = selectFileFromDiskFilter(selectFileTyp, @"c:\User\Public\Documents");
            this.Title = "Comos Daten-Datei ist gewählt";
        }

        private void btnFehlerSuchen_Click(object sender, RoutedEventArgs e)
        {
            

                // Datei holen
            string loadFile = tbFehlersuche.Text;
            // Prüfen ob Dateien existiert
            if (string.IsNullOrEmpty(loadFile) || !File.Exists(loadFile))
                MessageBox.Show("Es wurde keine Datei gewählt");
            else
            {
                // Datei öffnen               
                //    var filterGeräteAktiv =
                //        from geräteAktiv in xmlComosLoad(tbopenComosToEplan.Text).MSR_Gerät
                // where geräteAktiv.Signale[0].HW_Anbindung.
                // select geräteAktiv;




            }

        }

        private void tabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

       
    }
}
