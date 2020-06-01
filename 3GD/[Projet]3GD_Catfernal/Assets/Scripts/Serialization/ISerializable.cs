using System.Xml;

namespace Game.Serialization
{
    /// <summary>
    /// Public interface for serializable elements.
    /// </summary>
    public interface ISerializable
    {
        #region Methods
        XmlNode Serialize(XmlDocument document);
        void Deserialize(XmlNode rootNode);
        #endregion
    }
}