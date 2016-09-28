using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DependentSelection.Test
{
    [TestClass]
    public class DependentSelectionFactoryRegistrationAttributeTest
    {
        [TestMethod]
        public void CanCreateDependentSelectionFactoryRegistrationAttribute()
        {
            var dependentSelectionFactoryRegistrationAttribute = new DependentSelectionFactoryRegistrationAttribute();

            Assert.AreEqual(typeof(IDependentSelectionFactory), dependentSelectionFactoryRegistrationAttribute.ServiceType);
        }
    }
}
