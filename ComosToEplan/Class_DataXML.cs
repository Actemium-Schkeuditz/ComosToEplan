using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace ComosToEplan
{
    /// <summary>
    /// Datenstrucktur für Import der Comos Daten aus Excel
    /// </summary>

    public struct ComosDataImport
    {
        /// <remarks/>
        public int RevEPC;
        /// <remarks/>
        public string RevEPCKommentar;
        /// <remarks/>
        public bool aktiv;
        /// <remarks/>
        public int RevPMS;
        /// <remarks/>
        public string RevPMSKommentar;
        /// <remarks/>
        public int RevACT;
        /// <remarks/>
        public string RevACTKommentar;
        /// <remarks/>
        public DateTime Erzeugt;
        /// <remarks/>
        public string Funktion;
#pragma warning disable CS1591 // Fehledes XML-Kommentar für öffentlich sichtbaren Typ oder Element
        public string Tag_NR;

        public string TAG_NAME;
        public string TAG_NAME_ACT;
        public string PL;
        public string Beschreibung;
        public string Ebene;
        public string Koordinate;
        public string Hersteller;
        public string Gerät;
        public string gerätBeschreibung;
        public string Anbindung;
        public string Signalrichtung;
        public string Wertebereich;
        public string Signalphysik;
        public int Signalindex;
        public string SafePosition;
        public string SW_Typical;
        public DateTime Signal_Erzeugt;
        public string Signalerweiterung;
        public bool signalaktiv;
        public string Signal;
        public string Verbalbeschreibung;
        public string Signalgrundtyp;
        public string messbereichMin;
        public string messbereichMax;
        public string messbereichEinheit;
        public string Hysterese;
        public string Reglerwirkung;
        public string StellgrenzeMin;
        public string StellgrenzeMax;
        public string AlarmUnten;
        public string WarnungUnten;
        public string TolereanzUnten;
        public string AlarmOben;
        public string WarnungOben;
        public string ToleranzOben;
        public string BedeutungFalse;
        public string BedeutungTrue;
        public string revers;

        public string Busstrang;
        public int SK3;
        public int Trunk;
        public int Feldbarriere;
        public string AS;
        public string IO_Station;
        public string RACK;
        public string Steckplatz;
        public string Kanal;
        public string HW_Adresse;
        public string PB_Adresse;
        public string Kartentyp;
        public string SlaveTyp;
        public int Ausbaustufe;
#pragma warning restore CS1591 // Fehledes XML-Kommentar für öffentlich sichtbaren Typ oder Element
    }

    /// <summary>
    /// Datenstruktur für Import Eplan Inhaltsverzeichnis Daten
    /// </summary>

    public struct EplanDataContent : IEnumerable
    {/// <summary>
    /// Anlagenkenzeichen
    /// </summary>
        public string Anlage;
        /// <summary>
        /// Einbauort
        /// </summary>
        public string Einbauort;
        /// <summary>
        /// 
        /// </summary>
        public string Struktur;
            /// <summary>
            /// Seitennam
            /// </summary>
        public string Seitenname;
        /// <summary>
        /// 
        /// </summary>
        public string Seitenbeschreibung;
        /// <summary>
        /// 
        /// </summary>
        public string Änderungsdatum;
/// <summary>
/// erstellt am
/// </summary>
        public string Erstellungsdatum;
        /// <summary>
        /// Makroname
        /// </summary>
        public string Makroname;
        /// <summary>
        /// Makrobeschreibung
        /// </summary>
        public string Makrobeschreibung;
        /// <summary>
        /// Bemerkungen aus Eplan
        /// </summary>
        public string Bemerkungen;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Anlage).GetEnumerator();
        }
    }

    /// <summary>
    /// Class für Vergleicher Eplan Inhaltsverzeichnis mit Comos
    /// </summary>
    public class ClassMSREPC : IEnumerable
    {
        /// <summary>
        /// Konstruktor ClassMSREPC
        /// </summary>
        public ClassMSREPC()
        {
        }
        /// <summary>
        /// TAG
        /// </summary>
        public string TAG { get; set; }
        /// <summary>
        /// Dateum der Seitenerstellung
        /// </summary>
        public string Erstellungsdatum { get; set; }
        /// <summary>
        /// Makroname
        /// </summary>
        public string Makroname { get; set; }
        /// <summary>
        /// Bemerkunmgen
        /// </summary>
        public string Bemerkungen { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="erstellungsdatum"></param>
        /// <param name="makroname"></param>
        /// <param name="bemerkungen"></param>
        public ClassMSREPC(string tag, string erstellungsdatum, string makroname, string bemerkungen)
        {
            this.TAG = tag;
            this.Erstellungsdatum = erstellungsdatum;
            this.Makroname = makroname;
            this.Bemerkungen = bemerkungen;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            // return ((IEnumerable)TAG).GetEnumerator(); //einfache Variante zurückgabe des TAG    
            for (int i = 0; i < TAG.Length; i++)        // Variante die eine Verwendung in foreach ermöglicht, mit GetList
            {
                yield return TAG[i];
                yield return Erstellungsdatum[i];
                yield return Makroname[i];
                yield return Bemerkungen[i];
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable GetList()
        {
            for (int i = 0; i < TAG.Length; i++)        // Variante die eine Verwendung in foreach ermöglicht, mit GetList
            {
                yield return TAG[i];
                yield return Erstellungsdatum[i];
                yield return Makroname[i];
                yield return Bemerkungen[i];

            }
        }

    }

    /// <summary>
    /// nicht in Verwendung
    /// </summary>
    public class ClassEplanStatus
    {
        /// <summary>
        /// in Eplan 
        /// </summary>
        public ClassEplanStatus()
        { }
        /// <summary>
        /// TAG wie in COMOS
        /// </summary>
        public string TAG { get; set; }
        /// <summary>
        /// Eplan Status
        /// </summary>
        /// <param name="tag"></param>
        public ClassEplanStatus(string tag)
        {
            this.TAG = tag;
        }
    }

    /// <summary>
    /// nicht in Verwendung
    /// </summary>
    public class ClassEplanPages
    {
        /// <summary>
        /// 
        /// </summary>
        public ClassEplanPages()
        {

        }

        /// <summary>
        /// Tag-Name
        /// </summary>
        public string TAG { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        public ClassEplanPages(string tag)
        {
            this.TAG = tag;
        }

    }
    /// <summary>
    /// Vergleicher für TAG-Name
    /// </summary>
    public class TagComparer : IEqualityComparer<ClassMSREPC>
    {/// <summary>
     /// Vergleicher TAG
     /// </summary>
     /// <param name="x"></param>
     /// <param name="y"></param>
     /// <returns>bool Ergebnis des Vergleichs</returns>
        public bool Equals(ClassMSREPC x, ClassMSREPC y)
        {
            return (x.TAG == y.TAG);
        }
        /// <summary>
        /// Rückgabe Hashcode
        /// </summary>
        /// <param name="book"></param>
        /// <returns>int</returns>
        public int GetHashCode(ClassMSREPC book)
        {
            return book.TAG.GetHashCode();
        }
    }

    /// <summary>
    /// Class zur Nutzung mit FileopenDialog
    /// </summary>
    public class Filetypen
    {
        /// <summary>
        /// Auswahl der Filetypen
        /// </summary>
        public Filetypen()
        {
        }
        private string _filter;
        private string _title;
        /// <summary>
        /// FileOpenDialog Namme der Titleleiste
        /// </summary>
        public string title
        {
            get
            {
                return _title;
            }
        }
        /// <summary>
        /// Dateifilter
        /// </summary>
        public string filter
        {
            get
            {
                return _filter;
            }

            set
            {
                if (value == "Excel")
                {
                    _filter = "Excel Files| *.xls; *.xlsx; *.xlsm";
                    _title = "Select a Excel File";
                }
                else if (value == "XML")
                {
                    _filter = "XML Files| *.xml";
                    _title = "Select a XML File";
                }
                else
                {
                    _filter = "All Files| *.* ";
                    _title = "Select a any File";
                }
            }

        }
    }

    /// <summary>
    /// Class Comos Daten
    /// </summary>
    public class ComosDataC
    {
        /// <summary>
        /// Rückgabe der Daten
        /// </summary>
        /// <param name="erzeugt"></param>
        /// <param name="geräte_bezeichnung"></param>
        /// <param name="bezeichnung"></param>
        /// <param name="aktiv"></param>
        /// <param name="ausbaustufe"></param>
        /// <param name="funktion"></param>
        /// <param name="tagNr"></param>
        /// <param name="tagName"></param>
        /// <param name="tagNameAct"></param>
        /// <param name="pl"></param>
        /// <param name="gerät"></param>
        /// <param name="hersteller"></param>
        /// <param name="revEPC"></param>
        public ComosDataC(string erzeugt, string geräte_bezeichnung, string bezeichnung, bool aktiv, string ausbaustufe, string funktion, string tagNr, string tagName, string tagNameAct, string pl, string gerät, string hersteller,
            string revEPC)
        {
            this.Erzeugt = erzeugt;
            this.geräteBezeichnung = geräte_bezeichnung;
            this.Beschreibung = bezeichnung;
            this.Aktiv = aktiv;
            this.Ausbaustufe = stringToInt(ausbaustufe);
            this.Funktion = funktion;
            this.Tag_NR = tagNr;
            this.TAG_NAME = tagName;
            this.TAG_NAME_ACT = tagNameAct;
            this.PL = PL;
            this.Gerät = gerät;
            this.Hersteller = hersteller;
            //Revision
            this.RevEPC = stringToInt(revEPC);



        }

        private int stringToInt(string input)
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


        private int _RevEPC;
        private string _RevEPCKommentar;
        private bool _aktiv;
        private int _RevPMS;
        private string _revPMSKommentar;
        private int _revACT;
        private string _revACTKommentar;
        private string _erzeugt;
        private string _funktion;
        private string _tag_NR;
        private string _TAG_NAME;
        private string _TAG_NAME_ACT;
        private string _PL;
        private string _gerät;
        private string _hersteller;

        private string _beschreibung;
        private string _ebene;
        private string _koordinate;


        private string _geräteBezeichnung;
        private string _anbindung;
        private string _SW_Typical;
        private string _signalErzeugt;
        private string _signal;
        private string _signalgrundtyp;
        private string _busstrang;
        private int _SK3;
        private int _trunk;
        private int _feldbarriere;
        private string _AS;
        private string _iO_Station;
        private string _rACK;
        private string _steckplatz;
        private string _kanal;
        private string _hW_Adresse;
        private string _pB_Adresse;
        private string _kartentyp;
        private string _slaveTyp;
        private int _ausbaustufe;

        /// <summary>
        /// Gerät angelegt
        /// </summary>
        public string Erzeugt
        {
            get
            {
                return _erzeugt;
            }

            set
            {
                _erzeugt = value;
            }
        }
        /// <summary>
        /// Gerätebezeichnung
        /// </summary>
        public string geräteBezeichnung
        {
            get
            {
                return _geräteBezeichnung;
            }

            set
            {
                _geräteBezeichnung = value;
            }
        }
        /// <summary>
        /// Gerätefunktion
        /// </summary>
        public string Funktion
        {
            get
            {
                return _funktion;
            }

            set
            {
                _funktion = value;
            }
        }
        /// <summary>
        /// Tag_Nr aus Comos
        /// </summary>
        public string Tag_NR
        {
            get
            {
                return _tag_NR;
            }

            set
            {
                _tag_NR = value;
            }
        }
        /// <summary>
        /// Tag-Name Comos
        /// </summary>
        public string TAG_NAME
        {
            get
            {
                return _TAG_NAME;
            }

            set
            {
                _TAG_NAME = value;
            }
        }
        /// <summary>
        /// Tag-Name von ACT
        /// </summary>
        public string TAG_NAME_ACT
        {
            get
            {
                return _TAG_NAME_ACT;
            }

            set
            {
                _TAG_NAME_ACT = value;
            }
        }
        /// <summary>
        /// Prozesslinie
        /// </summary>
        public string PL
        {
            get
            {
                return _PL;
            }

            set
            {
                _PL = value;
            }
        }

        /// <summary>
        /// Gerätetyp
        /// </summary>
        public string Gerät
        {
            get
            {
                return _gerät;
            }

            set
            {
                _gerät = value;
            }
        }
        /// <summary>
        /// Geräte Hersteller
        /// </summary>
        public string Hersteller
        {
            get
            {
                return _hersteller;
            }

            set
            {
                _hersteller = value;
            }
        }

        /// <summary>
        /// Revisionsnummer EPC
        /// </summary>
        public int RevEPC
        {
            get
            {
                return _RevEPC;
            }

            set
            {
                _RevEPC = value;
            }
        }
        /// <summary>
        /// Revisionskomentar EPC
        /// </summary>
        public string RevEPCKommentar
        {
            get
            {
                return _RevEPCKommentar;
            }

            set
            {
                _RevEPCKommentar = value;
            }
        }
        /// <summary>
        /// Gerät in Verwendung
        /// </summary>
        public bool Aktiv
        {
            get
            {
                return _aktiv;
            }

            set
            {
                _aktiv = value;
            }
        }
        /// <summary>
        /// Revisionsnummer PMS
        /// </summary>
        public int RevPMS
        {
            get
            {
                return _RevPMS;
            }

            set
            {
                _RevPMS = value;
            }
        }
        /// <summary>
        /// Revisionskomentar PMS
        /// </summary>
        public string RevPMSKommentar
        {
            get
            {
                return _revPMSKommentar;
            }

            set
            {
                _revPMSKommentar = value;
            }
        }
        /// <summary>
        /// Revisionsnummer ACT
        /// </summary>
        public int RevACT
        {
            get
            {
                return _revACT;
            }

            set
            {
                _revACT = value;
            }
        }
        /// <summary>
        /// Revisionskomentar ACT
        /// </summary>
        public string RevACTKommentar
        {
            get
            {
                return _revACTKommentar;
            }

            set
            {
                _revACTKommentar = value;
            }
        }
        /// <summary>
        /// Bauvorhaben
        /// </summary>
        public int Ausbaustufe
        {
            get
            {
                return _ausbaustufe;
            }

            set
            {
                _ausbaustufe = value;
            }
        }
        /// <summary>
        /// Kartentyp des RIO
        /// </summary>
        public string Kartentyp
        {
            get
            {
                return _kartentyp;
            }

            set
            {
                _kartentyp = value;
            }
        }
        /// <summary>
        /// Profibusadresse aus SPS-Config
        /// </summary>
        public string PB_Adresse
        {
            get
            {
                return _pB_Adresse;
            }

            set
            {
                _pB_Adresse = value;
            }
        }
        /// <summary>
        /// Adresse aus SPS HW-Config
        /// </summary>
        public string HW_Adresse
        {
            get
            {
                return _hW_Adresse;
            }

            set
            {
                _hW_Adresse = value;
            }
        }
        /// <summary>
        /// Kanal aus SPS-Config
        /// </summary>
        public string Kanal
        {
            get
            {
                return _kanal;
            }

            set
            {
                _kanal = value;
            }
        }
        /// <summary>
        /// Rack der RIO
        /// </summary>
        public string RACK
        {
            get
            {
                return _rACK;
            }

            set
            {
                _rACK = value;
            }
        }

        /// <summary>
        /// Bezeichner der RIO
        /// </summary>
        public string IO_Station
        {
            get
            {
                return _iO_Station;
            }

            set
            {
                _iO_Station = value;
            }
        }
        /// <summary>
        /// Steckplatz
        /// </summary>
        public string Steckplatz
        {
            get
            {
                return _steckplatz;
            }

            set
            {
                _steckplatz = value;
            }
        }
        /// <summary>
        /// Slavetyp
        /// </summary>
        public string SlaveTyp
        {
            get
            {
                return _slaveTyp;
            }

            set
            {
                _slaveTyp = value;
            }
        }
        /// <summary>
        /// Beschreibung der Messstelle
        /// </summary>
        public string Beschreibung
        {
            get
            {
                return _beschreibung;
            }

            set
            {
                _beschreibung = value;
            }
        }
        /// <summary>
        /// Ebene des Gerätes
        /// </summary>
        public string Ebene
        {
            get
            {
                return _ebene;
            }

            set
            {
                _ebene = value;
            }
        }
        /// <summary>
        /// Koordinate des Gerätes
        /// </summary>
        public string Koordinate
        {
            get
            {
                return _koordinate;
            }

            set
            {
                _koordinate = value;
            }
        }

        /// <summary>
        /// Anbindung elektrisch
        /// </summary>
        public string Anbindung
        {
            get
            {
                return _anbindung;
            }

            set
            {
                _anbindung = value;
            }
        }
        /// <summary>
        /// Software Typical
        /// </summary>
        public string SW_Typical
        {
            get
            {
                return _SW_Typical;
            }

            set
            {
                _SW_Typical = value;
            }
        }
        /// <summary>
        /// Signal angelegt
        /// </summary>
        public string Signal_Erzeugt
        {
            get
            {
                return _signalErzeugt;
            }

            set
            {
                _signalErzeugt = value;
            }
        }

        /// <summary>
        /// Signalname
        /// </summary>
        public string Signal
        {
            get
            {
                return _signal;
            }

            set
            {
                _signal = value;
            }
        }
        /// <summary>
        /// Signalgrundtyp
        /// </summary>
        public string Signalgrundtyp
        {
            get
            {
                return _signalgrundtyp;
            }

            set
            {
                _signalgrundtyp = value;
            }
        }
        /// <summary>
        /// Busstarng
        /// </summary>
        public string Busstrang
        {
            get
            {
                return _busstrang;
            }

            set
            {
                _busstrang = value;
            }
        }
        /// <summary>
        /// SK3-Koppler ja/nein
        /// </summary>
        public int SK3
        {
            get
            {
                return _SK3;
            }

            set
            {
                _SK3 = value;
            }
        }
        /// <summary>
        /// Trunk des PA-Buses
        /// </summary>
        public int Trunk
        {
            get
            {
                return _trunk;
            }

            set
            {
                _trunk = value;
            }
        }
        /// <summary>
        /// Feldbarriere am PA-Trunk
        /// </summary>
        public int Feldbarriere
        {
            get
            {
                return _feldbarriere;
            }

            set
            {
                _feldbarriere = value;
            }
        }
        /// <summary>
        /// Automatisierungssystem in dem die Messung verarbeitet wird
        /// </summary>
        public string AS
        {
            get
            {
                return _AS;
            }

            set
            {
                _AS = value;
            }
        }
    }


    /// <summary>
    /// Vergleicher Comos Daten nur Testfunktion
    /// </summary>

    public class ComosComparer : IEqualityComparer<ComosDataC>
    {
        /// <summary>
        /// Vergleicher TAG_NAME
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>bool</returns>
        public bool Equals(ComosDataC x, ComosDataC y)
        {
            return ((x.TAG_NAME == y.TAG_NAME) & (x.Aktiv == y.Aktiv));
        }
        /// <summary>
        /// Rückgabe Hashcode
        /// </summary>
        /// <param name="tag"></param>
        /// <returns>int</returns>
        public int GetHashCode(ComosDataC tag)
        {
            return tag.TAG_NAME.GetHashCode();
        }
    }


    /// <summary>
    /// Class Eigene Klasse zum Suchen und Kopieren
    /// abgeleitet von MSR
    /// </summary>
    public class MSRhilfs : MSR, IEnumerable
    {
        /// <summary>
        /// Rückgabewert des Vergleichers
        /// </summary>
        public MSRhilfs[] MSR_Gerät_TAG
        {
            get
            {
                return this.MSR_Gerät_TAG;
            }
            set
            {
                this.MSR_Gerät_TAG = value;
            }
        }

        /// <summary>
        /// macht class MSRhilfs durchsuchbar
        /// </summary>
        /// <returns></returns>

        public IEnumerator GetEnumerator()
        {
            //return MSR_Gerät.GetEnumerator();
            for (int i = 0; i < MSR_Gerät.Length; i++)        // Variante die eine Verwendung in foreach ermöglicht, mit GetList
                yield return MSR_Gerät[i];
        }



    }

    /// <summary>
    /// Vergleicher für Comos Daten
    /// nicht fertig
    /// </summary>
    public class MSRComparer : IEqualityComparer<MSRhilfs>//,<MSRMSR_Gerät>
    {
        /// <summary>
        /// Rückgabe Vergelichsergebnis
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>bool</returns>

        public bool Equals(MSRhilfs x, MSRhilfs y)//, MSRMSR_Gerät z)
        {
            return ((x.MSR_Gerät[0].Signale == y.MSR_Gerät[0].Signale)); //& (x.Aktiv == y.Aktiv));
        }
        /// <summary>
        /// Rückgabe Hashcode
        /// </summary>
        /// <param name="tag"></param>
        /// <returns>int</returns>
        public int GetHashCode(MSRhilfs tag)
        {
            return tag.MSR_Gerät.GetHashCode();
        }
    }

    /// <summary>
    /// 
    /// </summary>

    public partial class PumpenConfig : IEnumerable
    {
        /// <summary>
        /// 
        /// </summary>
        public PumpenConfig()
        {
            this.BMK = _bmk;
            this.Bezeichnung = _bezeichnung;
            this.Sensor = _sensor;
            this.PL = _pL;
            this.Ebene = _ebene;
        }
        private string _bmk;
        private string _bezeichnung;
        private string _sensor;
        private int _pL;
        private int _ebene;
        /// <summary>
        /// 
        /// </summary>
        public string BMK
        {
            get
            {
                return _bmk;
            }

            set
            {
                _bmk = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Bezeichnung
        {
            get
            {
                return _bezeichnung;
            }

            set
            {
                _bezeichnung = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Sensor
        {
            get
            {
                return _sensor;
            }

            set
            {
                _sensor = value;
            }
        }
       
/// <summary>
/// Ebene
/// </summary>
        public int Ebene
        {
            get
            {
                return _ebene;
            }

            set
            {
                _ebene = value;
            }
        }
        /// <summary>
        /// PL
        /// </summary>
        public int PL
        {
            get
            {
                return _pL;
            }

            set
            {
                _pL = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {

            // return ((IEnumerable)TAG).GetEnumerator(); //einfache Variante zurückgabe des TAG    
            for (int i = 0; i < BMK.Length; i++)        // Variante die eine Verwendung in foreach ermöglicht, mit GetList
            {
                yield return BMK[i];
                yield return Bezeichnung[i];
                yield return Sensor[i];
               // yield return PL;
               // yield return Ebene;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>

    public class MSRMSRGerät_Pumpe :  IEnumerable
    {
        /// <summary>
        /// Kostruktor
        /// </summary>
        public MSRMSRGerät_Pumpe()
        {
            this.BMK = _bmk;
            this.PumpeBezeichnung = _pumpeBezeichnung;
            this.MSR_Gerät = mSR_GerätField;
            this.PL = _pL;
            this.Ebene = _ebene;
        }
        private MSRMSR_Gerät mSR_GerätField;

        /// <summary>
        /// class MSR_Gerät Gerätedaten
        /// </summary>
        public MSRMSR_Gerät MSR_Gerät
           {
         get
            {
               return this.mSR_GerätField;
            }
         set
           {
              this.mSR_GerätField = value;

            }
        }

        private string _bmk;
        private string _pumpeBezeichnung;
        private int _pL;
        private int _ebene;
        /// <summary>
        /// BMK
        /// </summary>
        public string BMK
            {
                get
                {
                    return _bmk;
                }

                set
                {
                    _bmk = value;
                }
            }

        /// <summary>
        /// Bezeichnung aus RundI
        /// </summary>
        public string PumpeBezeichnung
        {
            get
            {
                return _pumpeBezeichnung;
            }

            set
            {
                _pumpeBezeichnung = value;
            }
        }

        /// <summary>
        /// Ebene
        /// </summary>
        public int Ebene
        {
            get
            {
                return _ebene;
            }

            set
            {
                _ebene = value;
            }
        }
        /// <summary>
        /// PL
        /// </summary>
        public int PL
        {
            get
            {
                return _pL;
            }

            set
            {
                _pL = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
            {
                return ((IEnumerable)BMK).GetEnumerator();
            }
        }

    
}




//public bool Equals(ComosData x, ComosData y)

//      {

//          throw new NotImplementedException();

//      }


//public int GetHashCode(ComosData obj)

//     {

//         throw new NotImplementedException();

//       }

//   }

//}



