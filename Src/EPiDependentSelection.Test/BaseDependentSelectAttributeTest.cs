using System.Collections.Generic;
using EPiServer.Cms.Shell.UI.ObjectEditing;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Modules;
using EPiServer.Shell.ObjectEditing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace EPiDependentSelection.Test
{
    [TestClass]
    public abstract class BaseDependentSelectAttributeTest
    {
        protected Mock<IServiceLocator> _serviceLocatorMock;
        protected const string ResolvePathReturns = "EPiDependentSelection/stores/dependentselection/{0}/";

        [TestInitialize()]
        public void DependentSelectOneAttributeTestInitialize()
        {
            _serviceLocatorMock = new Mock<IServiceLocator>();

            var moduleTableMock = new Mock<ModuleTable>();
            moduleTableMock.Setup(x => x.ResolvePath("EPiDependentSelection", "stores/dependentselection/{0}/")).Returns(ResolvePathReturns);
            _serviceLocatorMock.Setup(x => x.GetInstance<ModuleTable>()).Returns(moduleTableMock.Object);

            var dependentSelectionFactoryMock = new Mock<IDependentSelectionFactory>();
            dependentSelectionFactoryMock.Setup(x => x.GetSelections(It.IsAny<IContent>())).Returns(new List<ISelectItem>() { new SelectItem() { Value = "value", Text = "text" } });
            _serviceLocatorMock.Setup(x => x.GetInstance(typeof(Mock<IDependentSelectionFactory>))).Returns(dependentSelectionFactoryMock.Object);

            _serviceLocatorMock.Setup(x => x.GetInstance<IServiceLocator>()).Returns(_serviceLocatorMock.Object);

            ServiceLocator.SetLocator(_serviceLocatorMock.Object);
        }

        protected ContentDataMetadata GetContentDataMetadata()
        {
            var extendedDataAnnotationsModelMetadataProviderMock = new Mock<ExtendedDataAnnotationsModelMetadataProvider>();

            return new ContentDataMetadata(null, null, typeof(Mock<IDependentSelectionFactory>), "annotatedProperty", null, extendedDataAnnotationsModelMetadataProviderMock.Object, null, null);
        }
    }
}
