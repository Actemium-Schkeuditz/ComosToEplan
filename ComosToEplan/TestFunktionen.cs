
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ComosToEplan
{

    /// <summary>
    /// Funktionen die nur für Tests geschrieben wurden
    /// </summary>
    public partial class MainWindow

    {

          

        // nur für Tests
        private void btnXMLread_click(object sender, RoutedEventArgs e)
        {
            // auslesen aller werte
            // XDocument xdocument = XDocument.Load(@"C:\Users\Public\Documents\XML_output_3.xml");
            // IEnumerable<XElement> MSR = xdocument.Elements();
            // foreach (var MSR_Gerät in MSR)
            // {
            //     Console.WriteLine();
            // }

            // auslesen einzelner Elemente
            //XElement xelement = XElement.Load(@"C:\Users\Public\Documents\XML_output_3.xml");
            //  IEnumerable<XElement> MSR = xelement.Elements();
            //   IEnumerable<ClassMSREPC> EPCRevB = LoadFromXML(@"C:\Users\Public\Documents\ComosEplanVerrgleich\Eplan_Inhaltsverzeichnis_2017428.xml");
            // IEnumerable<XElement> MSRItem = EPCRevB.Elements(); 
            //    foreach (var item in EPCRevB)
            //   {
            //   MSRItem = (EPCRevB.Elements("EPLAN_DATA").ElementAt(1));
            //   }

            Console.WriteLine("List of all Employee Names :");


            //  foreach (var TAG in MSRItem)
            //  {
            //      Console.WriteLine(TAG.Element("TAG-Nr").Value);
            //  }


        }


        // nur für Tests
        private void btnXmlTest_click(object sender, RoutedEventArgs e)
        {
            // laden der Dokumente
            XElement aDoc = XElement.Load(@"C:\Users\Public\Documents\XML_output_3.xml");
            XElement bDoc = XElement.Load(@"C:\Users\Public\Documents\XML_output_3_1.xml");
            //  XElement bDoc = XElement.Load(@"C:\Users\Public\Documents\XML_output_Eplan_2017425.xml");
            // IEnumerable<XElement> msrListA = aDoc.Elements();
            // IEnumerable<XElement> msrListB = bDoc.Elements();

            //Finding common elements(Elements that are in both A and B)

            // COMMON ELEMENTS QUERY (one-liner)

            // alles ausgeben
            //   var commonfromA = aDoc.Descendants("MSR_Gerät").Cast<XNode>().Intersect(bDoc.Descendants("MSR_Gerät").Cast<XNode>(), new XNodeEqualityComparer());
            // nur die MSR_Geräte ausgeben wenn gleiche Struckturen genutzt werden
            //     var commonfromA = aDoc.Elements("MSR_Gerät")
            //       .Where (aDoc)


            //XElement aGerät = msrListA.Descendants("Geräte_Bezeichnung")
            //       .Where(pers => (string)pers == "E21004")
            //       .First();
            //var elemts = aGerät.Ancestors("MSR_Gerät");




            // CROSS JOIN (RETURN elements in Both XML documents)
            // IMPORTANT: Use cross-joins only when you need cross-join results
            // Always prefer regular joins as they are faster. If you need non-equi joins
            // There other options than using cross-join with where condition.
            // You might get into performance issues if you use cross-joins for non-equi join cases
            var allGeräteAB = from aGerät in aDoc.Descendants("MSR_Gerät")
                              from bGerät in bDoc.Descendants("MSR_Gerät") 
                             
                              where aGerät.Attribute("TAG").Value == bGerät.Attribute("TAG").Value 
                              select new { GerätA = aGerät, GerätB = bGerät };

            foreach (var temp in allGeräteAB)
            {

                Console.WriteLine("Gerät" + temp);
            }




        }


        // nur für Tests
        private void btn_XML_Search_Click(object sender, RoutedEventArgs e)
        {
            XElement root = XElement.Load(@"C:\Users\Public\Documents\ComosEplanVerrgleich\Eplan_Inhaltsverzeichnis_2017428.xml");


            //  XElement person = root.Element("EPLAN_DATA")
            //          .Where(pers => (string)pers != "E21004")
            //          .First();
            //  var elemts = person.Ancestors("MSR_Gerät");



            // die Methode 'Ancestors'
            // nur mit COMOS DATA
            //XElement person = root.Descendants("Geräte_Bezeichnung")
            //        .Where(pers => (string)pers == "E21004")
            //        .First();
            //var elemts = person.Ancestors("MSR_Gerät");

            // Console.WriteLine(" ---------- Methode 'Ancestors' -----------");
            // foreach (var item in elemts.Elements())
            // if (item.HasAttributes)
            //{
            //foreach (var temp in item.Attributes())
            //Console.WriteLine((string)temp);
            // }
            //       else
            // Console.WriteLine((string)item);
            // Console.WriteLine("\n\n");

            //var elemts2 = elemts.Descendants("Signale");
            // Console.WriteLine(" ---------- Methode 'Ancestors Signale' -----------");
            //foreach (var item2 in elemts2.Elements())
            // if (item2.HasAttributes)
            // {
            //foreach (var temp in item2.Attributes())
            //Console.WriteLine((string)temp);
            //}
            //  else
            //Console.WriteLine((string)item2);
            //Console.WriteLine("\n\n");

            Console.ReadLine();
        }

        // nur für test 
        private void btnEplanDatatoOutput_click(object sender, RoutedEventArgs e)
        {
            XElement xelement = XElement.Load(@"C:\Users\Public\Documents\ComosEplanVerrgleich\Eplan_Inhaltsverzeichnis_201753.xml");
            IEnumerable<XElement> outputData = xelement.Elements("MSR_Gerät");
            IEnumerable<XElement> eplandata = outputData.Elements("EPLAN_DATA");
            IEnumerable<XElement> makroData = eplandata.Elements("Makroname");
            Console.WriteLine("List of all Tag :");
            foreach (var TAG in makroData)
            {

                // Console.WriteLine(TAG.Elements("Status_Erstellt").ToString());
                Console.WriteLine(TAG.ToString());
            }

        }

        /// <summary>
        /// nur Test
        /// Vergleich der COMOS Daten
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCompareComos_click(object sender, RoutedEventArgs e)
        {
            // Merge
            // Datendatei EPC
            string comosDataAlt = tbopenComosContent.Text;
            string comosDataNeu = tbopenEplanContent.Text;

            this.Title = "Start neue Comos gegen alte COMOS Daten Vergleich";
            IEnumerable<ComosDataC> EPCRevB = LoadComos(comosDataAlt);
            IEnumerable<ComosDataC> eplanContent = LoadComos(comosDataNeu);
            ComosComparer oComparer = new ComosComparer();
            var comparerListInEplan = eplanContent.Intersect(EPCRevB, oComparer).ToList();

            // Vergleicher für fehlende TAG-Nummern
            var comparerListEplanOpen = eplanContent.Except(EPCRevB, oComparer).ToList();
        }

        /// <summary>
        /// Vergleicher für Comos gegen Eplan Daten
        /// </summary>
        /// <param name="filname"></param>
        /// <returns></returns>
        IEnumerable<ComosDataC> LoadComos(string filname)
        {
            XElement xml = XElement.Load((filname));
            var query = (from book in xml.Descendants("MSR_Gerät")
                         orderby (string)book.Attribute("TAG") ascending
                         select new ComosDataC(
                             (string)book.Element("Erzeugt") != null ? (string)book.Element("Erzeugt") : string.Empty,
                             (string)book.Element("Geräte_Bezeichnung") != null ? (string)book.Element("Geräte_Bezeichnung") : string.Empty,
                             (string)book.Element("Bezeichnung") != null ? (string)book.Element("Bezeichnung") : string.Empty,
                             (bool)book.Element("Aktiv"),
                             (string)(book.Element("Ausbaustufe")) != null ? (string)book.Element("Ausbaustufe") : string.Empty,
                             (string)(book.Element("Funktion")) != null ? (string)book.Element("Funktion") : string.Empty,
                             (string)(book.Element("Tag_NR")) != null ? (string)book.Element("Tag_NR") : string.Empty,
                             (string)(book.Element("TAG_NAME")) != null ? (string)book.Element("TAG_NAME") : string.Empty, //TAG_NAME
                             (string)(book.Element("TAG_NAME_ACT")) != null ? (string)book.Element("TAG_NAME_ACT") : string.Empty, //TAG_NAME_ACT
                             (string)(book.Element("PL")) != null ? (string)book.Element("PL") : string.Empty, //PL
                             (string)(book.Element("Gerät")) != null ? (string)book.Element("Gerät") : string.Empty, //Gerät
                             (string)(book.Element("Hersteller")) != null ? (string)book.Element("Hersteller") : string.Empty, //Hersteller
                             (string)(book.Descendants("Revision").ElementAt(0)) != null ? (string)book.Descendants("Revision").ElementAt(0) : string.Empty //RevEPC

                                //   (string)book.Attribute("TAG") != null ? (string)book.Attribute("TAG") : string.Empty


                                )
           );
            return query;
        }    
    }

}