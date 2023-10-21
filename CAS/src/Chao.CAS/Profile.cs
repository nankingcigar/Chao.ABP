using System.Xml.Serialization;

namespace Chao.CAS;

[XmlType(AnonymousType = true, Namespace = "http://www.yale.edu/tp/cas")]
[XmlRoot(Namespace = "http://www.yale.edu/tp/cas", IsNullable = false, ElementName = "serviceResponse")]
public class Profile
{
    [XmlElement("authenticationSuccess")]
    public UserInfo UserInfo { get; set; }
}