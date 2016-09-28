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
	public class DependentSelectOneAttributeTest : BaseDependentSelectAttributeTest
    {
        [TestMethod]
		public void CanCreateWithValidValues()
		{
            var dependentOn = new[] { "Test1" };
            var dependentSelectOneAttribute = new DependentSelectOneAttribute(typeof(Mock<IDependentSelectionFactory>))
            {
                DependentOn = dependentOn,
                ReadOnlyOnEmpty = true
            };

            Assert.IsTrue(dependentSelectOneAttribute.ReadOnlyOnEmpty);
            Assert.AreEqual(dependentOn, dependentSelectOneAttribute.DependentOn);
            Assert.AreEqual(typeof(Mock<IDependentSelectionFactory>), dependentSelectOneAttribute.DependentSelectionFactoryType);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CanNotCreateWithInValidType()
        {
            var dependentSelectOneAttribute = new DependentSelectOneAttribute(typeof(IDependentSelectionFactory));
        }

        [TestMethod]
        public void CanCreateWithEmptyValues()
        {
            var dependentSelectOneAttribute = new DependentSelectOneAttribute(typeof(Mock<IDependentSelectionFactory>));

            Assert.IsFalse(dependentSelectOneAttribute.ReadOnlyOnEmpty);
            Assert.IsNull(dependentSelectOneAttribute.DependentOn);
            Assert.AreEqual(typeof(Mock<IDependentSelectionFactory>), dependentSelectOneAttribute.DependentSelectionFactoryType);
        }

        [TestMethod]
        public void CanRunBasicOnMetadataCreated()
        {
            var type = typeof(Mock<IDependentSelectionFactory>);
            var dependentSelectOneAttribute = new DependentSelectOneAttribute(type);

            var contentDataMetadata = GetContentDataMetadata();
            contentDataMetadata.OwnerContent = new Mock<IContent>().Object;

            dependentSelectOneAttribute.OnMetadataCreated(contentDataMetadata);

            Assert.AreEqual("dependent-selection/DependentSelectionEditor", contentDataMetadata.ClientEditingClass);
            Assert.AreEqual(dependentSelectOneAttribute.ReadOnlyOnEmpty, contentDataMetadata.EditorConfiguration[Constants.ReadOnlyOnEmpty]);
            Assert.AreEqual(string.Format(CultureInfo.InvariantCulture, ResolvePathReturns, type.FullName), contentDataMetadata.EditorConfiguration[Constants.StoreUrl]);
            Assert.AreEqual(1, (contentDataMetadata.EditorConfiguration[Constants.Selections] as IList<ISelectItem>).Count);
            Assert.AreEqual(0, (contentDataMetadata.EditorConfiguration[Constants.DependentOn] as IDictionary<string, object>).Count);
            Assert.IsFalse(contentDataMetadata.IsReadOnly);
        }

        [TestMethod]
        public void CanNotRunOnMetadataCreatedWithoutOwnerContent()
        {
            var dependentSelectOneAttribute = new DependentSelectOneAttribute(typeof(Mock<IDependentSelectionFactory>));

            var contentDataMetadata = GetContentDataMetadata();

            dependentSelectOneAttribute.OnMetadataCreated(contentDataMetadata);

            Assert.IsNull(contentDataMetadata.ClientEditingClass);
        }

        [TestMethod]
        public void CanNotRunOnMetadataCreatedWithoutOwnerContentAsIContent()
        {
            var type = typeof(Mock<IDependentSelectionFactory>);
            var dependentSelectOneAttribute = new DependentSelectOneAttribute(type);
            var dependentSelectionFactoryMock = new Mock<IDependentSelectionFactory>();

            _serviceLocatorMock.Setup(x => x.GetInstance(type)).Returns(dependentSelectionFactoryMock.Object);

            var contentDataMetadata = GetContentDataMetadata();
            contentDataMetadata.OwnerContent = new Mock<IContentData>().Object;

            dependentSelectOneAttribute.OnMetadataCreated(contentDataMetadata);

            Assert.IsNull(contentDataMetadata.ClientEditingClass);
        }

        [TestMethod]
        public void CanRunOnMetadataCreated()
        {
            var dependentSelectOneAttribute = new DependentSelectOneAttribute(typeof(Mock<IDependentSelectionFactory>))
            {
                DependentOn = new[] { "property1" },
            };

            var contentDataMetadata = GetContentDataMetadata();

            var property1 = new PropertyString("property1Value");
            var propertyDataCollection = new PropertyDataCollection();
            propertyDataCollection.Add("property1" , property1);
            var contentMock = new Mock<IContent>();
            contentMock.Setup(x => x.Property).Returns(propertyDataCollection);

            contentDataMetadata.OwnerContent = contentMock.Object;

            dependentSelectOneAttribute.OnMetadataCreated(contentDataMetadata);

            Assert.AreEqual(dependentSelectOneAttribute.ReadOnlyOnEmpty, contentDataMetadata.EditorConfiguration[Constants.ReadOnlyOnEmpty]);
            Assert.AreEqual(1, (contentDataMetadata.EditorConfiguration[Constants.Selections] as IList<ISelectItem>).Count);
            Assert.AreEqual(1, (contentDataMetadata.EditorConfiguration[Constants.DependentOn] as IDictionary<string, object>).Count);
        }

        [TestMethod]
        public void AnnotatedPropertyIsExcludedFromDependentOn()
        {
            var dependentSelectOneAttribute = new DependentSelectOneAttribute(typeof(Mock<IDependentSelectionFactory>));

            var contentDataMetadata = GetContentDataMetadata();

            var contentMock = new Mock<IContent>();

            contentDataMetadata.OwnerContent = contentMock.Object;

            dependentSelectOneAttribute.OnMetadataCreated(contentDataMetadata);

            Assert.AreEqual(dependentSelectOneAttribute.ReadOnlyOnEmpty, contentDataMetadata.EditorConfiguration[Constants.ReadOnlyOnEmpty]);
            Assert.AreEqual(1, (contentDataMetadata.EditorConfiguration[Constants.Selections] as IList<ISelectItem>).Count);
            Assert.AreEqual(0, (contentDataMetadata.EditorConfiguration[Constants.DependentOn] as IDictionary<string, object>).Count);
        }

        [TestMethod]
        public void NotReadOnlyWhenAvailableSelections()
        {
            var dependentSelectOneAttribute = new DependentSelectOneAttribute(typeof(Mock<IDependentSelectionFactory>))
            {
                ReadOnlyOnEmpty = true,
            };

            var contentDataMetadata = GetContentDataMetadata();

            var contentMock = new Mock<IContent>();

            contentDataMetadata.OwnerContent = contentMock.Object;

            dependentSelectOneAttribute.OnMetadataCreated(contentDataMetadata);

            Assert.IsTrue(dependentSelectOneAttribute.ReadOnlyOnEmpty);
            Assert.AreEqual(1, (contentDataMetadata.EditorConfiguration[Constants.Selections] as IList<ISelectItem>).Count);
            Assert.IsFalse(contentDataMetadata.IsReadOnly);
        }

        [TestMethod]
        public void ReadOnlyWhenNoSelections()
        {
            var type = typeof(Mock<IDependentSelectionFactory>);
            var dependentSelectOneAttribute = new DependentSelectOneAttribute(type)
            {
                ReadOnlyOnEmpty = true,
            };

            var dependentSelectionFactoryMock = new Mock<IDependentSelectionFactory>();
            dependentSelectionFactoryMock.Setup(x => x.GetSelections(It.IsAny<IContent>())).Returns(new List<ISelectItem>());
            _serviceLocatorMock.Setup(x => x.GetInstance(type)).Returns(dependentSelectionFactoryMock.Object);

            var contentDataMetadata = GetContentDataMetadata();

            var contentMock = new Mock<IContent>();

            contentDataMetadata.OwnerContent = contentMock.Object;

            dependentSelectOneAttribute.OnMetadataCreated(contentDataMetadata);

            Assert.IsTrue(dependentSelectOneAttribute.ReadOnlyOnEmpty);
            Assert.AreEqual(0, (contentDataMetadata.EditorConfiguration[Constants.Selections] as IList<ISelectItem>).Count);
            Assert.IsTrue(contentDataMetadata.IsReadOnly);
        }
    }
}
