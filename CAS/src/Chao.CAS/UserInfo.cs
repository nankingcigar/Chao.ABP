using System.Xml.Serialization;

namespace Chao.CAS;

[XmlType(AnonymousType = true, Namespace = "http://www.yale.edu/tp/cas")]
public class UserInfo
{
    [XmlElement("user")]
    public virtual string UserName { get; set; }
}