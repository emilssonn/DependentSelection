using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EPiLinkedSelection.Test
{
    [TestClass]
    public class LinkedSelectionFactoryRegistrationAttributeTest
    {
        [TestMethod]
        public void CanCreateLinkedSelectionFactoryRegistrationAttribute()
        {
            var linkedSelectionFactoryRegistrationAttribute = new LinkedSelectionFactoryRegistrationAttribute();

            Assert.AreEqual(typeof(ILinkedSelectionFactory), linkedSelectionFactoryRegistrationAttribute.ServiceType);
        }
    }
}
