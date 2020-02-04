
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;

namespace ComosToEplan
{

        /// <summary>
        /// Erzeugt Daten für Treeview 
        /// </summary>
      public class MapperXMLToTreeview

    {
        private string sourceXmlFile;
        private XDocument xmlData;

        /// <summary>
        /// Festlegen Quelle
        /// </summary>
        /// <param name="xmlFilePath">Quelldatei</param>
         public MapperXMLToTreeview(string xmlFilePath)
         {
            sourceXmlFile = xmlFilePath;
         }

        private void BuildNodes(TreeViewItem treeNode, XElement element)
        {

            string attributes = "";
            if (element.HasAttributes)
            {
                foreach (var att in element.Attributes())
                {
                    attributes += " " + att.Name + " = " + att.Value;
                }
            }

            TreeViewItem childTreeNode = new TreeViewItem
            {
                Header = element.Name.LocalName + attributes,
                IsExpanded = true
            };
            if (element.HasElements)
            {
                foreach (XElement childElement in element.Elements())
                {
                    BuildNodes(childTreeNode, childElement);
                }
            }
            else
            {
                TreeViewItem childTreeNodeText = new TreeViewItem
                {
                    Header = element.Value,
                    IsExpanded = true
                };
                childTreeNode.Items.Add(childTreeNodeText);
            }

            treeNode.Items.Add(childTreeNode);
        }

        /// <summary>
        /// Laden der XML-Datei
        /// </summary>
        /// <param name="treeview">Ziel Treeview</param>
        public void LoadXml(TreeView treeview)
        {
          try
            {
                if (sourceXmlFile != null)
                {
                   
                    xmlData = XDocument.Load(sourceXmlFile, LoadOptions.None);
                    if (xmlData == null)
                    {
                        throw new XmlException("Cannot load Xml document from file : " + sourceXmlFile);
                    }
                    else
                    {
                        TreeViewItem treeNode = new TreeViewItem
                        {
                            Header = sourceXmlFile,
                            IsExpanded = true
                        };
                        BuildNodes(treeNode, xmlData.Root);
                        treeview.Items.Add(treeNode);
                    }
                }
                else
                {
                    throw new IOException("Xml file is not set correctly.");
                }
            }
            catch (IOException ioex)
            {
                //log
                Console.WriteLine(ioex);
                MessageBox.Show(string.Format("Fehler beim Lesen der XML-Config {0} {1}", sourceXmlFile, ioex));
            }
            catch (XmlException xmlex)
            {
                //log
                Console.WriteLine(xmlex);
                MessageBox.Show(string.Format("Fehler beim Lesen der XML-Config {0} {1}", sourceXmlFile, xmlex));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //log
            }
        }
    }
}
