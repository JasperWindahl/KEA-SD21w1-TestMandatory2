using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable RedundantDelegateCreation


namespace XmlTestingApp
{
    public static class XmlValidation
    {
        private static List<string> _validationEvents = new List<string>();

        public static bool ValidateDtd(string inputFile)
        {
            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Parse,
                ValidationType = ValidationType.DTD
            };
            settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);

            var reader = XmlReader.Create(inputFile, settings);

            while (reader.Read()) { }

            return _validationEvents.Count == 0;
        }

        private static void ValidationCallBack(object sender, ValidationEventArgs e)
        {
            _validationEvents.Add($"Validation Error: {e.Message}");
        }
    }
}
