using System;
using System.Collections.Generic;
using System.Globalization;
using EPiServer.Core;
using EPiServer.Shell.ObjectEditing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DependentSelection.Test
{
    [TestClass]
    public class DependentSelectManyAttributeTest : BaseDependentSelectAttributeTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CanNotCreateWithInValidType()
        {
            var dependentSelectManyAttribute = new DependentSelectManyAttribute(typeof(IDependentSelectionFactory));
        }

        [TestMethod]
        public void CanRunBasicOnMetadataCreated()
        {
            var type = typeof(Mock<IDependentSelectionFactory>);
            var dependentSelectManyAttribute = new DependentSelectManyAttribute(type);

            var contentDataMetadata = GetContentDataMetadata();
            contentDataMetadata.OwnerContent = new Mock<IContent>().Object;

            dependentSelectManyAttribute.OnMetadataCreated(contentDataMetadata);

            Assert.AreEqual("dependent-selection/DependentCheckBoxListEditor", contentDataMetadata.ClientEditingClass);
            Assert.AreEqual(dependentSelectManyAttribute.ReadOnlyOnEmpty, contentDataMetadata.EditorConfiguration[Constants.ReadOnlyOnEmpty]);
            Assert.AreEqual(string.Format(CultureInfo.InvariantCulture, ResolvePathReturns, type.FullName), contentDataMetadata.EditorConfiguration[Constants.StoreUrl]);
            Assert.AreEqual(1, (contentDataMetadata.EditorConfiguration[Constants.Selections] as IList<ISelectItem>).Count);
            Assert.AreEqual(0, (contentDataMetadata.EditorConfiguration[Constants.DependentOn] as IDictionary<string, object>).Count);
            Assert.IsFalse(contentDataMetadata.IsReadOnly);
        }
    }
}
