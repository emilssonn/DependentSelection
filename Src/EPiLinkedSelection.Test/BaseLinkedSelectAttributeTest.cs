using System.Collections.Generic;
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
    public abstract class BaseLinkedSelectAttributeTest
    {
        protected Mock<IServiceLocator> _serviceLocatorMock;
        protected const string ResolvePathReturns = "cms/stores/contentdata/{0}";

        [TestInitialize()]
        public void LinkedSelectOneAttributeTestInitialize()
        {
            _serviceLocatorMock = new Mock<IServiceLocator>();

            var moduleTableMock = new Mock<ModuleTable>();
            moduleTableMock.Setup(x => x.ResolvePath("cms", "stores/contentdata/{0}")).Returns(ResolvePathReturns);
            _serviceLocatorMock.Setup(x => x.GetInstance<ModuleTable>()).Returns(moduleTableMock.Object);

            var linkedSelectionFactoryMock = new Mock<ILinkedSelectionFactory>();
            linkedSelectionFactoryMock.Setup(x => x.GetSelections(It.IsAny<IContent>())).Returns(new List<ISelectItem>() { new SelectItem() { Value = "value", Text = "text" } });
            _serviceLocatorMock.Setup(x => x.GetInstance(typeof(Mock<ILinkedSelectionFactory>))).Returns(linkedSelectionFactoryMock.Object);

            _serviceLocatorMock.Setup(x => x.GetInstance<IServiceLocator>()).Returns(_serviceLocatorMock.Object);

            ServiceLocator.SetLocator(_serviceLocatorMock.Object);
        }

        protected ContentDataMetadata GetContentDataMetadata()
        {
            var extendedDataAnnotationsModelMetadataProviderMock = new Mock<ExtendedDataAnnotationsModelMetadataProvider>();

            return new ContentDataMetadata(null, null, typeof(Mock<ILinkedSelectionFactory>), "annotatedProperty", null, extendedDataAnnotationsModelMetadataProviderMock.Object, null, null);
        }
    }
}
