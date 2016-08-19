using System;
using System.Collections.Generic;
using System.Globalization;
using EPiServer.Cms.Shell.UI.ObjectEditing;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Modules;
using EPiServer.Shell.ObjectEditing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace EPiLinkedSelection.Test
{
    [TestClass]
	public class LinkedSelectOneAttributeTest
	{
        private Mock<IServiceLocator> _serviceLocatorMock;

        [TestInitialize()]
        public void LinkedSelectOneAttributeTestInitialize()
        {
            _serviceLocatorMock = new Mock<IServiceLocator>();

            var moduleTableMock = new Mock<ModuleTable>();
            moduleTableMock.Setup(x => x.ResolvePath("cms", "stores/contentdata/{0}")).Returns("cms/stores/contentdata/{0}");
            _serviceLocatorMock.Setup(x => x.GetInstance<ModuleTable>()).Returns(moduleTableMock.Object);

            var linkedSelectionFactoryMock = new Mock<ILinkedSelectionFactory>();
            linkedSelectionFactoryMock.Setup(x => x.GetSelections(It.IsAny<IContent>())).Returns(new List<ISelectItem>() { new SelectItem() { Value = "value", Text = "text" } });
            _serviceLocatorMock.Setup(x => x.GetInstance(typeof(Mock<ILinkedSelectionFactory>))).Returns(linkedSelectionFactoryMock.Object);

            _serviceLocatorMock.Setup(x => x.GetInstance<IServiceLocator>()).Returns(_serviceLocatorMock.Object);

            ServiceLocator.SetLocator(_serviceLocatorMock.Object);
        }

        [TestMethod]
		public void CanCreateWithValidValues()
		{
            var linkedTo = new[] { "Test1" };
            var linkedSelectOneAttribute = new LinkedSelectOneAttribute(typeof(Mock<ILinkedSelectionFactory>))
            {
                LinkedTo = linkedTo,
                ReadOnlyOnEmpty = true
            };

            Assert.IsTrue(linkedSelectOneAttribute.ReadOnlyOnEmpty);
            Assert.AreEqual(linkedTo, linkedSelectOneAttribute.LinkedTo);
            Assert.AreEqual(typeof(Mock<ILinkedSelectionFactory>), linkedSelectOneAttribute.LinkedSelectionFactoryType);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CanNotCreateWithInValidType()
        {
            var linkedSelectOneAttribute = new LinkedSelectOneAttribute(typeof(ILinkedSelectionFactory));
        }

        [TestMethod]
        public void CanCreateWithEmptyValues()
        {
            var linkedSelectOneAttribute = new LinkedSelectOneAttribute(typeof(Mock<ILinkedSelectionFactory>));

            Assert.IsFalse(linkedSelectOneAttribute.ReadOnlyOnEmpty);
            Assert.IsNull(linkedSelectOneAttribute.LinkedTo);
            Assert.AreEqual(typeof(Mock<ILinkedSelectionFactory>), linkedSelectOneAttribute.LinkedSelectionFactoryType);
        }

        [TestMethod]
        public void CanRunBasicOnMetadataCreated()
        {
            var type = typeof(Mock<ILinkedSelectionFactory>);
            var linkedSelectOneAttribute = new LinkedSelectOneAttribute(type);

            var contentDataMetadata = GetContentDataMetadata();
            contentDataMetadata.OwnerContent = new Mock<IContent>().Object;

            linkedSelectOneAttribute.OnMetadataCreated(contentDataMetadata);

            Assert.AreEqual("epi-linked-selection/LinkedSelectionEditor", contentDataMetadata.ClientEditingClass);
            Assert.AreEqual(linkedSelectOneAttribute.ReadOnlyOnEmpty, contentDataMetadata.EditorConfiguration[Constants.ReadOnlyOnEmpty]);
            Assert.AreEqual(string.Format(CultureInfo.InvariantCulture, "cms/stores/contentdata/{0}", type.FullName), contentDataMetadata.EditorConfiguration[Constants.StoreUrl]);
            Assert.AreEqual(1, (contentDataMetadata.EditorConfiguration[Constants.Selections] as IList<ISelectItem>).Count);
            Assert.AreEqual(0, (contentDataMetadata.EditorConfiguration[Constants.LinkedTo] as IDictionary<string, object>).Count);
            Assert.IsFalse(contentDataMetadata.IsReadOnly);
        }

        [TestMethod]
        public void CanNotRunOnMetadataCreatedWithoutOwnerContent()
        {
            var linkedSelectOneAttribute = new LinkedSelectOneAttribute(typeof(Mock<ILinkedSelectionFactory>));

            var contentDataMetadata = GetContentDataMetadata();

            linkedSelectOneAttribute.OnMetadataCreated(contentDataMetadata);

            Assert.IsNull(contentDataMetadata.ClientEditingClass);
        }

        [TestMethod]
        public void CanNotRunOnMetadataCreatedWithoutOwnerContentAsIContent()
        {
            var type = typeof(Mock<ILinkedSelectionFactory>);
            var linkedSelectOneAttribute = new LinkedSelectOneAttribute(type);
            var linkedSelectionFactoryMock = new Mock<ILinkedSelectionFactory>();

            _serviceLocatorMock.Setup(x => x.GetInstance(type)).Returns(linkedSelectionFactoryMock.Object);

            var contentDataMetadata = GetContentDataMetadata();
            contentDataMetadata.OwnerContent = new Mock<IContentData>().Object;

            linkedSelectOneAttribute.OnMetadataCreated(contentDataMetadata);

            Assert.IsNull(contentDataMetadata.ClientEditingClass);
        }

        [TestMethod]
        public void CanRunOnMetadataCreated()
        {
            var linkedSelectOneAttribute = new LinkedSelectOneAttribute(typeof(Mock<ILinkedSelectionFactory>))
            {
                LinkedTo = new[] { "property1" },
            };

            var contentDataMetadata = GetContentDataMetadata();

            var property1 = new PropertyString("property1Value");
            var propertyDataCollection = new PropertyDataCollection();
            propertyDataCollection.Add("property1" , property1);
            var contentMock = new Mock<IContent>();
            contentMock.Setup(x => x.Property).Returns(propertyDataCollection);

            contentDataMetadata.OwnerContent = contentMock.Object;

            linkedSelectOneAttribute.OnMetadataCreated(contentDataMetadata);

            Assert.AreEqual(linkedSelectOneAttribute.ReadOnlyOnEmpty, contentDataMetadata.EditorConfiguration[Constants.ReadOnlyOnEmpty]);
            Assert.AreEqual(1, (contentDataMetadata.EditorConfiguration[Constants.Selections] as IList<ISelectItem>).Count);
            Assert.AreEqual(1, (contentDataMetadata.EditorConfiguration[Constants.LinkedTo] as IDictionary<string, object>).Count);
        }

        [TestMethod]
        public void AnnotatedPropertyIsExcludedFromLinkTo()
        {
            var linkedSelectOneAttribute = new LinkedSelectOneAttribute(typeof(Mock<ILinkedSelectionFactory>));

            var contentDataMetadata = GetContentDataMetadata();

            var contentMock = new Mock<IContent>();

            contentDataMetadata.OwnerContent = contentMock.Object;

            linkedSelectOneAttribute.OnMetadataCreated(contentDataMetadata);

            Assert.AreEqual(linkedSelectOneAttribute.ReadOnlyOnEmpty, contentDataMetadata.EditorConfiguration[Constants.ReadOnlyOnEmpty]);
            Assert.AreEqual(1, (contentDataMetadata.EditorConfiguration[Constants.Selections] as IList<ISelectItem>).Count);
            Assert.AreEqual(0, (contentDataMetadata.EditorConfiguration[Constants.LinkedTo] as IDictionary<string, object>).Count);
        }

        [TestMethod]
        public void NotReadOnlyWhenAvailableSelections()
        {
            var linkedSelectOneAttribute = new LinkedSelectOneAttribute(typeof(Mock<ILinkedSelectionFactory>))
            {
                ReadOnlyOnEmpty = true,
            };

            var contentDataMetadata = GetContentDataMetadata();

            var contentMock = new Mock<IContent>();

            contentDataMetadata.OwnerContent = contentMock.Object;

            linkedSelectOneAttribute.OnMetadataCreated(contentDataMetadata);

            Assert.IsTrue(linkedSelectOneAttribute.ReadOnlyOnEmpty);
            Assert.AreEqual(1, (contentDataMetadata.EditorConfiguration[Constants.Selections] as IList<ISelectItem>).Count);
            Assert.IsFalse(contentDataMetadata.IsReadOnly);
        }

        [TestMethod]
        public void ReadOnlyWhenNoSelections()
        {
            var type = typeof(Mock<ILinkedSelectionFactory>);
            var linkedSelectOneAttribute = new LinkedSelectOneAttribute(type)
            {
                ReadOnlyOnEmpty = true,
            };

            var linkedSelectionFactoryMock = new Mock<ILinkedSelectionFactory>();
            linkedSelectionFactoryMock.Setup(x => x.GetSelections(It.IsAny<IContent>())).Returns(new List<ISelectItem>());
            _serviceLocatorMock.Setup(x => x.GetInstance(type)).Returns(linkedSelectionFactoryMock.Object);

            var contentDataMetadata = GetContentDataMetadata();

            var contentMock = new Mock<IContent>();

            contentDataMetadata.OwnerContent = contentMock.Object;

            linkedSelectOneAttribute.OnMetadataCreated(contentDataMetadata);

            Assert.IsTrue(linkedSelectOneAttribute.ReadOnlyOnEmpty);
            Assert.AreEqual(0, (contentDataMetadata.EditorConfiguration[Constants.Selections] as IList<ISelectItem>).Count);
            Assert.IsTrue(contentDataMetadata.IsReadOnly);
        }

        private ContentDataMetadata GetContentDataMetadata()
        {
            var extendedDataAnnotationsModelMetadataProviderMock = new Mock<ExtendedDataAnnotationsModelMetadataProvider>();

            return new ContentDataMetadata(null, null, typeof(Mock<ILinkedSelectionFactory>), "annotatedProperty", null, extendedDataAnnotationsModelMetadataProviderMock.Object, null, null);
        }
    }
}
