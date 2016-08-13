using System;
using EPiServer;
using EPiServer.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace EPiLinkedSelection.Test
{
    [TestClass]
    public class LinkedSelectionStoreTest
    {
        private Mock<IServiceLocator> _serviceLocatorMock;

        [TestInitialize()]
        public void LinkedSelectOneAttributeTestInitialize()
        {
            _serviceLocatorMock = new Mock<IServiceLocator>();

            var contentLoaderMock = new Mock<IContentLoader>();


            _serviceLocatorMock.Setup(x => x.GetInstance<IContentLoader>()).Returns(contentLoaderMock.Object);

            ServiceLocator.SetLocator(_serviceLocatorMock.Object);
        }
    }
}
