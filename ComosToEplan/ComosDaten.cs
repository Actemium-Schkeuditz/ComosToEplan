﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
// upload to gitHUB
//------------------------------------------------------------------------------

// 
// Dieser Quellcode wurde automatisch generiert von xsd, Version=4.6.1055.0.
// 
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ComosToEplan
{
    // 
    // Dieser Quellcode wurde automatisch generiert von xsd, Version=4.6.1055.0.
    // 


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class MSR
    {

        private MSRMSR_Gerät[] mSR_GerätField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("MSR_Gerät")]
        public MSRMSR_Gerät[] MSR_Gerät
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
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class MSRMSR_Gerät
    {

        private string erzeugtField;

        private string geräte_BezeichnungField;

        private string bezeichnungField;

        private byte aktivField;

        private byte ausbaustufeField;

        private string funktionField;

        private string tag_NRField;

        private string tAG_NAMEField;

        private string tAG_NAME_ACTField;

        private string plField;

        private string ebeneField;

        private string koordinateField;

        private string gerätField;

        private string herstellerField;

        private string geräte_BeschreibungField;

        private MSRMSR_GerätEPLAN_DATA ePLAN_DATAField;

        private MSRMSR_GerätSignal[] signaleField;

        private string tAGField;

        /// <remarks/>
        public string Erzeugt
        {
            get
            {
                return this.erzeugtField;
            }
            set
            {
                this.erzeugtField = value;
            }
        }

        /// <remarks/>
        public string Geräte_Bezeichnung
        {
            get
            {
                return this.geräte_BezeichnungField;
            }
            set
            {
                this.geräte_BezeichnungField = value;
            }
        }

        /// <remarks/>
        public string Bezeichnung
        {
            get
            {
                return this.bezeichnungField;
            }
            set
            {
                this.bezeichnungField = value;
            }
        }

        /// <remarks/>
        public byte Aktiv
        {
            get
            {
                return this.aktivField;
            }
            set
            {
                this.aktivField = value;
            }
        }

        /// <remarks/>
        public byte Ausbaustufe
        {
            get
            {
                return this.ausbaustufeField;
            }
            set
            {
                this.ausbaustufeField = value;
            }
        }

        /// <remarks/>
        public string Funktion
        {
            get
            {
                return this.funktionField;
            }
            set
            {
                this.funktionField = value;
            }
        }

        /// <remarks/>
        public string Tag_NR
        {
            get
            {
                return this.tag_NRField;
            }
            set
            {
                this.tag_NRField = value;
            }
        }

        /// <remarks/>
        public string TAG_NAME
        {
            get
            {
                return this.tAG_NAMEField;
            }
            set
            {
                this.tAG_NAMEField = value;
            }
        }

        /// <remarks/>
        public string TAG_NAME_ACT
        {
            get
            {
                return this.tAG_NAME_ACTField;
            }
            set
            {
                this.tAG_NAME_ACTField = value;
            }
        }

        /// <remarks/>
        public string PL
        {
            get
            {
                return this.plField;
            }
            set
            {
                this.plField = value;
            }
        }

        /// <remarks/>
        public string Ebene
        {
            get
            {
                return this.ebeneField;
            }
            set
            {
                this.ebeneField = value;
            }
        }

        /// <remarks/>
        public string Koordinate
        {
            get
            {
                return this.koordinateField;
            }
            set
            {
                this.koordinateField = value;
            }
        }

        /// <remarks/>
        public string Gerät
        {
            get
            {
                return this.gerätField;
            }
            set
            {
                this.gerätField = value;
            }
        }

        /// <remarks/>
        public string Hersteller
        {
            get
            {
                return this.herstellerField;
            }
            set
            {
                this.herstellerField = value;
            }
        }

        /// <remarks/>
        public string Geräte_Beschreibung
        {
            get
            {
                return this.geräte_BeschreibungField;
            }
            set
            {
                this.geräte_BeschreibungField = value;
            }
        }

        /// <remarks/>
        public MSRMSR_GerätEPLAN_DATA EPLAN_DATA
        {
            get
            {
                return this.ePLAN_DATAField;
            }
            set
            {
                this.ePLAN_DATAField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Signal", IsNullable = false)]
        public MSRMSR_GerätSignal[] Signale
        {
            get
            {
                return this.signaleField;
            }
            set
            {
                this.signaleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TAG
        {
            get
            {
                return this.tAGField;
            }
            set
            {
                this.tAGField = value;
            }
        }  
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class MSRMSR_GerätEPLAN_DATA
    {

        private byte status_ErstelltField;

        private string erstellungsdatumField;

        private string makronameField;

        private string bemerkungenField;

        /// <remarks/>
        public byte Status_Erstellt
        {
            get
            {
                return this.status_ErstelltField;
            }
            set
            {
                this.status_ErstelltField = value;
            }
        }

        /// <remarks/>
        public string Erstellungsdatum
        {
            get
            {
                return this.erstellungsdatumField;
            }
            set
            {
                this.erstellungsdatumField = value;
            }
        }

        /// <remarks/>
        public string Makroname
        {
            get
            {
                return this.makronameField;
            }
            set
            {
                this.makronameField = value;
            }
        }

        /// <remarks/>
        public string Bemerkungen
        {
            get
            {
                return this.bemerkungenField;
            }
            set
            {
                this.bemerkungenField = value;
            }
        }  
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class MSRMSR_GerätSignal
    {

        private string signalField;

        private string erzeugtField;

        private byte _signalaktiv;

        private string asField;

        private MSRMSR_GerätSignalSignalKonfig signalKonfigField;

        private MSRMSR_GerätSignalRevision revisionField;

        private MSRMSR_GerätSignalHW_Anbindung hW_AnbindungField;

        private MSRMSR_GerätSignalSoftware softwareField;

        private string signal1Field;

        /// <remarks/>
        public string Signal
        {
            get
            {
                return this.signalField;
            }
            set
            {
                this.signalField = value;
            }
        }

        /// <remarks/>
        public string Erzeugt
        {
            get
            {
                return this.erzeugtField;
            }
            set
            {
                this.erzeugtField = value;
            }
        }
        /// <remarks/>
        public byte Signal_Aktiv
        {
            get
            {
                return this._signalaktiv;
            }

            set
            {
                this._signalaktiv = value;
            }
        }

/// <remarks/>
public string AS
        {
            get
            {
                return this.asField;
            }
            set
            {
                this.asField = value;
            }
        }

        /// <remarks/>
        public MSRMSR_GerätSignalSignalKonfig SignalKonfig
        {
            get
            {
                return this.signalKonfigField;
            }
            set
            {
                this.signalKonfigField = value;
            }
        }

        /// <remarks/>
        public MSRMSR_GerätSignalRevision Revision
        {
            get
            {
                return this.revisionField;
            }
            set
            {
                this.revisionField = value;
            }
        }

        /// <remarks/>
        public MSRMSR_GerätSignalHW_Anbindung HW_Anbindung
        {
            get
            {
                return this.hW_AnbindungField;
            }
            set
            {
                this.hW_AnbindungField = value;
            }
        }

        /// <remarks/>
        public MSRMSR_GerätSignalSoftware Software
        {
            get
            {
                return this.softwareField;
            }
            set
            {
                this.softwareField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("Signal")]
        public string Signal1
        {
            get
            {
                return this.signal1Field;
            }
            set
            {
                this.signal1Field = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class MSRMSR_GerätSignalSignalKonfig
    {

        private string anbindungField;

        private string signalrichtungField;

        private string wertebereichField;

        private string signalgrundtypField;

        private string signalphysikField;

        private byte signalindexField;

        private string safepositionField;

        private string signalerweiterungField;

        private string messbereichMinField;

        private string messbereichMaxField;

        private string einheitField;

        /// <remarks/>
        public string Anbindung
        {
            get
            {
                return this.anbindungField;
            }
            set
            {
                this.anbindungField = value;
            }
        }

        /// <remarks/>
        public string Signalrichtung
        {
            get
            {
                return this.signalrichtungField;
            }
            set
            {
                this.signalrichtungField = value;
            }
        }

        /// <remarks/>
        public string Wertebereich
        {
            get
            {
                return this.wertebereichField;
            }
            set
            {
                this.wertebereichField = value;
            }
        }

        /// <remarks/>
        public string Signalgrundtyp
        {
            get
            {
                return this.signalgrundtypField;
            }
            set
            {
                this.signalgrundtypField = value;
            }
        }

        /// <remarks/>
        public string Signalphysik
        {
            get
            {
                return this.signalphysikField;
            }
            set
            {
                this.signalphysikField = value;
            }
        }

        /// <remarks/>
        public byte Signalindex
        {
            get
            {
                return this.signalindexField;
            }
            set
            {
                this.signalindexField = value;
            }
        }

        /// <remarks/>
        public string Safeposition
        {
            get
            {
                return this.safepositionField;
            }
            set
            {
                this.safepositionField = value;
            }
        }

        /// <remarks/>
        public string Signalerweiterung
        {
            get
            {
                return this.signalerweiterungField;
            }
            set
            {
                this.signalerweiterungField = value;
            }
        }

        /// <remarks/>
        public string MessbereichMin
        {
            get
            {
                return this.messbereichMinField;
            }
            set
            {
                this.messbereichMinField = value;
            }
        }

        /// <remarks/>
        public string MessbereichMax
        {
            get
            {
                return this.messbereichMaxField;
            }
            set
            {
                this.messbereichMaxField = value;
            }
        }

        /// <remarks/>
        public string Einheit
        {
            get
            {
                return this.einheitField;
            }
            set
            {
                this.einheitField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class MSRMSR_GerätSignalRevision
    {

        private byte revEPCField;

        private string revEPC_KomentarField;

        private byte revPMSField;

        private string revPMS_KomentarField;

        private byte revACTField;

        private string revACT_KomentarField;

        /// <remarks/>
        public byte RevEPC
        {
            get
            {
                return this.revEPCField;
            }
            set
            {
                this.revEPCField = value;
            }
        }

        /// <remarks/>
        public string RevEPC_Komentar
        {
            get
            {
                return this.revEPC_KomentarField;
            }
            set
            {
                this.revEPC_KomentarField = value;
            }
        }

        /// <remarks/>
        public byte RevPMS
        {
            get
            {
                return this.revPMSField;
            }
            set
            {
                this.revPMSField = value;
            }
        }

        /// <remarks/>
        public string RevPMS_Komentar
        {
            get
            {
                return this.revPMS_KomentarField;
            }
            set
            {
                this.revPMS_KomentarField = value;
            }
        }

        /// <remarks/>
        public byte RevACT
        {
            get
            {
                return this.revACTField;
            }
            set
            {
                this.revACTField = value;
            }
        }

        /// <remarks/>
        public string RevACT_Komentar
        {
            get
            {
                return this.revACT_KomentarField;
            }
            set
            {
                this.revACT_KomentarField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class MSRMSR_GerätSignalHW_Anbindung
    {

        private string busstrangField;

        private MSRMSR_GerätSignalHW_AnbindungProfibus_PA profibus_PAField;

        private string kartentypField;

        private string slaveTypField;

        private string iO_StationField;

        private string rACKField;

        private string steckplatzField;

        private string kanalField;

        private string hW_AdresseField;

        private string pB_AdresseField;

        /// <remarks/>
        public string Busstrang
        {
            get
            {
                return this.busstrangField;
            }
            set
            {
                this.busstrangField = value;
            }
        }

        /// <remarks/>
        public MSRMSR_GerätSignalHW_AnbindungProfibus_PA Profibus_PA
        {
            get
            {
                return this.profibus_PAField;
            }
            set
            {
                this.profibus_PAField = value;
            }
        }

        /// <remarks/>
        public string Kartentyp
        {
            get
            {
                return this.kartentypField;
            }
            set
            {
                this.kartentypField = value;
            }
        }

        /// <remarks/>
        public string SlaveTyp
        {
            get
            {
                return this.slaveTypField;
            }
            set
            {
                this.slaveTypField = value;
            }
        }

        /// <remarks/>
        public string IO_Station
        {
            get
            {
                return this.iO_StationField;
            }
            set
            {
                this.iO_StationField = value;
            }
        }

        /// <remarks/>
        public string RACK
        {
            get
            {
                return this.rACKField;
            }
            set
            {
                this.rACKField = value;
            }
        }

        /// <remarks/>
        public string Steckplatz
        {
            get
            {
                return this.steckplatzField;
            }
            set
            {
                this.steckplatzField = value;
            }
        }

        /// <remarks/>
        public string Kanal
        {
            get
            {
                return this.kanalField;
            }
            set
            {
                this.kanalField = value;
            }
        }

        /// <remarks/>
        public string HW_Adresse
        {
            get
            {
                return this.hW_AdresseField;
            }
            set
            {
                this.hW_AdresseField = value;
            }
        }

        /// <remarks/>
        public string PB_Adresse
        {
            get
            {
                return this.pB_AdresseField;
            }
            set
            {
                this.pB_AdresseField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class MSRMSR_GerätSignalHW_AnbindungProfibus_PA
    {

        private byte sK3Field;

        private byte trunkField;

        private byte feldbarriereField;

        /// <remarks/>
        public byte SK3
        {
            get
            {
                return this.sK3Field;
            }
            set
            {
                this.sK3Field = value;
            }
        }

        /// <remarks/>
        public byte Trunk
        {
            get
            {
                return this.trunkField;
            }
            set
            {
                this.trunkField = value;
            }
        }

        /// <remarks/>
        public byte Feldbarriere
        {
            get
            {
                return this.feldbarriereField;
            }
            set
            {
                this.feldbarriereField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class MSRMSR_GerätSignalSoftware
    {
        private string _verbalbeschreibung;

        private string hystereseField;

        private string reglerwirkungField;

        private string stellgrenzeMinField;

        private string stellgrenzeMaxField;

        private string alarmUntenField;

        private string warnungUntenField;

        private string toleranzUntenField;

        private string tolerenzObenField;

        private string warnungObenField;

        private string alarmObenField;

        private string signalBedeutungFalseField;

        private string signalBedeutungTrueField;

        private string reverseField;

        /// <remarks>Verbalbeschreibung des Signals</remarks>
        public string Verbalbeschreibung
        {
            get
            {
                return this._verbalbeschreibung;
            }
            set
            {
                this._verbalbeschreibung = value;
            }
        }
        /// <remarks/>
        public string Hysterese
        {
            get
            {
                return this.hystereseField;
            }
            set
            {
                this.hystereseField = value;
            }
        }

        /// <remarks/>
        public string Reglerwirkung
        {
            get
            {
                return this.reglerwirkungField;
            }
            set
            {
                this.reglerwirkungField = value;
            }
        }

        /// <remarks/>
        public string StellgrenzeMin
        {
            get
            {
                return this.stellgrenzeMinField;
            }
            set
            {
                this.stellgrenzeMinField = value;
            }
        }

        /// <remarks/>
        public string StellgrenzeMax
        {
            get
            {
                return this.stellgrenzeMaxField;
            }
            set
            {
                this.stellgrenzeMaxField = value;
            }
        }

        /// <remarks/>
        public string AlarmUnten
        {
            get
            {
                return this.alarmUntenField;
            }
            set
            {
                this.alarmUntenField = value;
            }
        }

        /// <remarks/>
        public string WarnungUnten
        {
            get
            {
                return this.warnungUntenField;
            }
            set
            {
                this.warnungUntenField = value;
            }
        }

        /// <remarks/>
        public string ToleranzUnten
        {
            get
            {
                return this.toleranzUntenField;
            }
            set
            {
                this.toleranzUntenField = value;
            }
        }

        /// <remarks/>
        public string TolerenzOben
        {
            get
            {
                return this.tolerenzObenField;
            }
            set
            {
                this.tolerenzObenField = value;
            }
        }

        /// <remarks/>
        public string WarnungOben
        {
            get
            {
                return this.warnungObenField;
            }
            set
            {
                this.warnungObenField = value;
            }
        }

        /// <remarks/>
        public string AlarmOben
        {
            get
            {
                return this.alarmObenField;
            }
            set
            {
                this.alarmObenField = value;
            }
        }

        /// <remarks/>
        public string SignalBedeutungFalse
        {
            get
            {
                return this.signalBedeutungFalseField;
            }
            set
            {
                this.signalBedeutungFalseField = value;
            }
        }

        /// <remarks/>
        public string SignalBedeutungTrue
        {
            get
            {
                return this.signalBedeutungTrueField;
            }
            set
            {
                this.signalBedeutungTrueField = value;
            }
        }

        /// <remarks/>
        public string Reverse
        {
            get
            {
                return this.reverseField;
            }
            set
            {
                this.reverseField = value;
            }
        }
    }


}
