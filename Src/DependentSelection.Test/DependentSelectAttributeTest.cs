using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DependentSelection.Test
{
    [TestClass]
    public class DependentSelectAttributeTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CanNotCreateWithInValidClientEditingClass()
        {
            /// Throws <see cref="System.Reflection.TargetInvocationException"/>, the inner exception contains the real exception.
            try
            {
                var dependentSelectAttributeMock = new Mock<DependentSelectAttribute>(typeof(Mock<IDependentSelectionFactory>), "");
                var test = dependentSelectAttributeMock.Object;
            }
            catch (Exception e)
            {
                throw e.InnerException;
            }
            
        }
    }
}
