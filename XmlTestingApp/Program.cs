using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace XmlTestingApp
{
    class Program
    {
        private static string _inputFile;
        static void Main(string[] args)
        {
            _inputFile = @"..\..\..\XMLDoc\vs-testcases.xml";

            // Inline XML DTD Validation
            var isValid = XmlValidation.ValidateDtd(_inputFile);
            if (isValid != true)
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
            //Console.WriteLine("Root Node: {0}", xmlRoot?.Name);

            // Generate Case List
            XmlNodeList cases = xmlRoot?.ChildNodes;

            // Iterate the TestCases
            if (cases != null)
            {
                foreach (XmlNode @case in cases)
                {
                    var useMaterialParser = false;
                    var useItemParser = false;

                    // Determine Correct Parser
                    for (int i = 0; i < @case.ChildNodes.Count; i++)
                    {
                        switch (@case.ChildNodes[i].Name)
                        {
                            case "material":
                                useMaterialParser = true;
                                break;
                            case "item":
                                useItemParser = true;
                                break;
                        }
                    }

                    // Output Result
                    if (useItemParser)
                    {
                        Console.WriteLine($"Case: {@case.Name}");
                        Console.WriteLine(ItemParser(@case));
                    }
                    else if (useMaterialParser)
                    {
                        Console.WriteLine($"Case: {@case.Name}");
                        Console.WriteLine(MaterialParser(@case));
                    }
                    else
                    {
                        Console.WriteLine($"Case: {@case.Name}");
                        Console.WriteLine($"{@case.Name}: No Suitable Parser");
                    }
                }
            }
        }

        public static string ItemParser(XmlNode xmlNode)
        {
            // Generate Lists of Elements based on keyword
            var givenElements = xmlNode.Cast<XmlElement>().Where(xmlElement => xmlElement.Name.ToLower() == "given").ToList();
            var whenElements = xmlNode.Cast<XmlElement>().Where(xmlElement => xmlElement.Name.ToLower() == "when").ToList();
            var itemElements = xmlNode.Cast<XmlElement>().Where(xmlElement => xmlElement.Name.ToLower() == "item").ToList();
            var addressElements = xmlNode.Cast<XmlElement>().Where(xmlElement => xmlElement.Name.ToLower() == "address").ToList();
            var thenElements = xmlNode.Cast<XmlElement>().Where(xmlElement => xmlElement.Name.ToLower() == "then").ToList();

            // Build Output String using ElementsStringBuilder
            var result = $"IF {ElementsStringBuilder(whenElements)}" +
                         $" {ElementsStringBuilder(itemElements)}" +
                         $" TO {addressElements[0].InnerXml}" +
                         $", AND {ElementsStringBuilder(givenElements)}" +
                         $", THEN {ElementsStringBuilder(thenElements)}" +
                         $" {(addressElements.Count > 1 ? addressElements[1].InnerXml : "")}";
            return result;
        }

        public static string MaterialParser(XmlNode xmlNode)
        {
            // Generate Lists of Elements based on keyword
            var givenElements = xmlNode.Cast<XmlElement>().Where(xmlElement => xmlElement.Name.ToLower() == "given").ToList();
            var whenElements = xmlNode.Cast<XmlElement>().Where(xmlElement => xmlElement.Name.ToLower() == "when").ToList();
            var materialElements = xmlNode.Cast<XmlElement>().Where(xmlElement => xmlElement.Name.ToLower() == "material").ToList();
            var thenElements = xmlNode.Cast<XmlElement>().Where(xmlElement => xmlElement.Name.ToLower() == "then").ToList();

            // Build Output String using ElementsStringBuilder
            var result = $"IF {ElementsStringBuilder(whenElements)}" +
                         $" {ElementsStringBuilder(materialElements)}" +
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
