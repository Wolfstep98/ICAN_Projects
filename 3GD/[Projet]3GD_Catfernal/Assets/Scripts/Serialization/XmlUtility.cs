using System;
using System.Xml;

namespace Game.Serialization
{
    [Serializable]
    public static class XmlUtility
    {
        #region Fields
        public const string Extension = "xml";
        public const string XmlSavePath = "DataSave/";
        public const string XmlEnemySavePath = "DataSave/Enemies/";
        #endregion

        #region Contructors
        static XmlUtility()
        {

        }
        #endregion

        #region Fields

        #endregion

        #region Methods
        public static XmlDocument CreateDocument()
        {
            XmlDocument document = new XmlDocument();
            XmlDeclaration declaration = document.CreateXmlDeclaration("1.0", "UTF-8", "no");
            document.AppendChild(declaration);

            return document;
        }
        #endregion
    }
}