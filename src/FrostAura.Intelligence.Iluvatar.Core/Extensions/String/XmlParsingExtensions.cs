using FrostAura.Intelligence.Iluvatar.Core.Models.Planning;
using System.Xml.Serialization;

namespace FrostAura.Intelligence.Iluvatar.Core.Extensions.String
{
    /// <summary>
    /// XML string extensions.
    /// </summary>
    public static class XmlParsingExtensions
    {
        /// <summary>
        /// Parse an XML string as an object type.
        /// </summary>
        /// <typeparam name="T">The object type of the root element.</typeparam>
        /// <param name="xmlStr">String containing the XML payload.</param>
        /// <param name="goal">The goal that the planner is solving for.</param>
        /// <returns>The parsed XML payload.</returns>
        public static Root PlannerFromXmlStr(this string xmlStr, string goal)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(Root));
                var wrappedXmlStr = $"<root>{xmlStr}</root>";
                using var stringReader = new StringReader(wrappedXmlStr);
                var planner = (Root)serializer.Deserialize(stringReader);

                planner.Plan.Goal = goal;
                planner.Plan.Content = xmlStr;

                return planner;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
