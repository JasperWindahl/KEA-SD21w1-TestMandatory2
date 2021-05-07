using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
// ReSharper disable UnusedParameter.Local

namespace XmlTestingApp
{
    class Program
    {
        private static string _inputFile;
        static void Main(string[] args)
        {
            _inputFile = @"..\..\..\XMLDoc\testcases.xml";

            // Inline XML DTD Validation
            if (XmlValidation.ValidateDtd(_inputFile) != true)
            {
                Console.WriteLine("The DTD for this XML File is Invalid, exiting!");
                return;
            }

            // Load XML File from Disk
            var xmlFile = File.ReadAllText(_inputFile);

            // Load XML Document
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlFile);

            // Find Root Node
            var xmlRoot = xmlDoc.DocumentElement;

            // Baseline ErrorCheck: Check if Root Node is Valid and if Root Node is Testcases
            if (xmlRoot == null || xmlRoot.Name.ToLower() != "testcases")
            {
                Console.WriteLine("XML File not valid for this application, exiting!");
                return;
            }

            // Generate Case List
            XmlNodeList cases = xmlRoot.ChildNodes;

            // Iterate the TestCases
            foreach (XmlNode @case in cases)
            {
                var caseName = @case.Name;
                Console.WriteLine($"Case: {caseName}");

                switch (caseName)
                {
                    case "shipitem":
                        Console.WriteLine(ShipItemParser(@case));
                        break;
                    case "ordermaterials":
                        Console.WriteLine(OrderMaterialsParser(@case));
                        break;
                    case "queryitems":
                        Console.WriteLine(QueryItemsParser(@case));
                        break;
                    default:
                        Console.WriteLine("No Suitable Parser");
                        break;
                }
            }
        }

        public static string ShipItemParser(XmlNode xmlNode)
        {
            // Generate Lists of Elements based on keyword
            var givenElements = xmlNode.Cast<XmlElement>().Where(xmlElement => xmlElement.Name.ToLower() == "given").ToList();
            var whenElements = xmlNode.Cast<XmlElement>().Where(xmlElement => xmlElement.Name.ToLower() == "when").ToList();
            var itemElements = xmlNode.Cast<XmlElement>().Where(xmlElement => xmlElement.Name.ToLower() == "item").ToList();
            var addressElements = xmlNode.Cast<XmlElement>().Where(xmlElement => xmlElement.Name.ToLower() == "address").ToList();
            var thenElements = xmlNode.Cast<XmlElement>().Where(xmlElement => xmlElement.Name.ToLower() == "then").ToList();

            // Build Output String using ElementsStringBuilder
            var result = $"IF {ElementsStringBuilder(whenElements)} " +
                         $"{ElementsStringBuilder(itemElements)} " +
                         $"TO {addressElements[0].InnerXml}" +
                         $", AND {ElementsStringBuilder(givenElements)}" +
                         $", THEN {ElementsStringBuilder(thenElements)} " +
                         $"{(addressElements.Count > 1 ? addressElements[1].InnerXml : "")}";
            return result;
        }

        public static string OrderMaterialsParser(XmlNode xmlNode)
        {
            // Generate Lists of Elements based on keyword
            var givenElements = xmlNode.Cast<XmlElement>().Where(xmlElement => xmlElement.Name.ToLower() == "given").ToList();
            var whenElements = xmlNode.Cast<XmlElement>().Where(xmlElement => xmlElement.Name.ToLower() == "when").ToList();
            var materialElements = xmlNode.Cast<XmlElement>().Where(xmlElement => xmlElement.Name.ToLower() == "material").ToList();
            var thenElements = xmlNode.Cast<XmlElement>().Where(xmlElement => xmlElement.Name.ToLower() == "then").ToList();

            // Build Output String using ElementsStringBuilder
            var result = $"IF {ElementsStringBuilder(whenElements)} " +
                         $"{ElementsStringBuilder(materialElements)}" +
                         $", AND {ElementsStringBuilder(givenElements)}" +
                         $", THEN {ElementsStringBuilder(thenElements)}";
            return result;
        }

        public static string QueryItemsParser(XmlNode xmlNode)
        {
            var givenElements = xmlNode.Cast<XmlElement>().Where(xmlElement => xmlElement.Name.ToLower() == "given").ToList();
            var whenElements = xmlNode.Cast<XmlElement>().Where(xmlElement => xmlElement.Name.ToLower() == "when").ToList();
            var itemElements = xmlNode.Cast<XmlElement>().Where(xmlElement => xmlElement.Name.ToLower() == "item").ToList();
            var thenElements = xmlNode.Cast<XmlElement>().Where(xmlElement => xmlElement.Name.ToLower() == "then").ToList();

            var result = $"IF {ElementsStringBuilder(whenElements)} " +
                         $"{ElementsStringBuilder(itemElements)} " +
                         $", AND {ElementsStringBuilder(givenElements)}" +
                         $", THEN {ElementsStringBuilder(thenElements)}";
            return result;
        }

        public static string ElementsStringBuilder(List<XmlElement> elements)
        {
            var result = "";
            // Iterates list, and combines elemets to generate string
            for (var i = 0; i < elements.Count; i++)
            {
                if (i >= 1) result += " AND ";
                result += elements[i].InnerXml;
            }
            return result;
        }
    }
}
