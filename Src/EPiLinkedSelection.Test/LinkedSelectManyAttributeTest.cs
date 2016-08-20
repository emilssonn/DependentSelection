using System;
using System.Collections.Generic;
using System.Globalization;
using EPiServer.Core;
using EPiServer.Shell.ObjectEditing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace EPiLinkedSelection.Test
{
    [TestClass]
    public class LinkedSelectManyAttributeTest : BaseLinkedSelectAttributeTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CanNotCreateWithInValidType()
        {
            var linkedSelectOneAttribute = new LinkedSelectManyAttribute(typeof(ILinkedSelectionFactory));
        }

        [TestMethod]
        public void CanRunBasicOnMetadataCreated()
        {
            var type = typeof(Mock<ILinkedSelectionFactory>);
            var linkedSelectManyAttribute = new LinkedSelectManyAttribute(type);

            var contentDataMetadata = GetContentDataMetadata();
            contentDataMetadata.OwnerContent = new Mock<IContent>().Object;

            linkedSelectManyAttribute.OnMetadataCreated(contentDataMetadata);

            Assert.AreEqual("epi-linked-selection/LinkedCheckBoxListEditor", contentDataMetadata.ClientEditingClass);
            Assert.AreEqual(linkedSelectManyAttribute.ReadOnlyOnEmpty, contentDataMetadata.EditorConfiguration[Constants.ReadOnlyOnEmpty]);
            Assert.AreEqual(string.Format(CultureInfo.InvariantCulture, ResolvePathReturns, type.FullName), contentDataMetadata.EditorConfiguration[Constants.StoreUrl]);
            Assert.AreEqual(1, (contentDataMetadata.EditorConfiguration[Constants.Selections] as IList<ISelectItem>).Count);
            Assert.AreEqual(0, (contentDataMetadata.EditorConfiguration[Constants.LinkedTo] as IDictionary<string, object>).Count);
            Assert.IsFalse(contentDataMetadata.IsReadOnly);
        }
    }
}
