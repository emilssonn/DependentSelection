using System.Collections.Generic;
using System.Linq;
using System.Net;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.Services.Rest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace EPiDependentSelection.Test
{
    [TestClass]
    public class DependentSelectionStoreTest
    {
        private Mock<IServiceLocator> _serviceLocatorMock;

        [TestInitialize()]
        public void DependentSelectOneAttributeTestInitialize()
        {
            _serviceLocatorMock = new Mock<IServiceLocator>();

            var contentLoaderMock = new Mock<IContentLoader>();
            contentLoaderMock.Setup(x => x.Get<IContentData>(It.IsAny<ContentReference>())).Returns(new Mock<IContentData>().Object);
            _serviceLocatorMock.Setup(x => x.GetInstance<IContentLoader>()).Returns(contentLoaderMock.Object);

            ServiceLocator.SetLocator(_serviceLocatorMock.Object);
        }

        [TestMethod]
        public void CanGetSelections()
        {
            var dependentSelectionFactoryMock = new Mock<IDependentSelectionFactory>();
            var dependentSelectionStore = new DependentSelectionStore(new List<IDependentSelectionFactory>() { dependentSelectionFactoryMock.Object });

            var result = dependentSelectionStore.Get(dependentSelectionFactoryMock.Object.GetType().FullName, "1") as RestResult;

#pragma warning disable CS0618 // Type or member is obsolete
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
#pragma warning restore CS0618 // Type or member is obsolete
            Assert.AreEqual(0, (result.Data as IEnumerable<ISelectItem>).Count());
        }

        [TestMethod]
        public void CanNotGetSelections()
        {
            var dependentSelectionFactoryMock = new Mock<IDependentSelectionFactory>();
            var dependentSelectionStore = new DependentSelectionStore(new List<IDependentSelectionFactory>() { dependentSelectionFactoryMock.Object });

            var result = dependentSelectionStore.Get("Invalid type name", "1") as RestStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.NotFound, result.StatusCode);
            Assert.AreEqual("No matching dependent selection factory was found", result.StatusDescription);
        }

    }
}
