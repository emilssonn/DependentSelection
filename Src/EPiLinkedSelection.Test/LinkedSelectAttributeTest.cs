using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace EPiLinkedSelection.Test
{
    [TestClass]
    public class LinkedSelectAttributeTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CanNotCreateWithInValidClientEditingClass()
        {
            /// Throws <see cref="System.Reflection.TargetInvocationException"/>, the inner exception contains the real exception.
            try
            {
                var linkedSelectAttributeMock = new Mock<LinkedSelectAttribute>(typeof(Mock<ILinkedSelectionFactory>), "");
                var test = linkedSelectAttributeMock.Object;
            }
            catch (Exception e)
            {
                throw e.InnerException;
            }
            
        }
    }
}
