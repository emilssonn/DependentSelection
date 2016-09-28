using System;

namespace DependentSelection
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="DependentSelection.DependentSelectAttribute" />
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DependentSelectOneAttribute : DependentSelectAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependentSelectOneAttribute" /> class.
        /// </summary>
        /// <param name="dependentSelectionFactoryType">Type of the dependent selection factory.</param>
        public DependentSelectOneAttribute(Type dependentSelectionFactoryType) 
            : base(dependentSelectionFactoryType, "dependent-selection/DependentSelectionEditor")
        {
        }
    }
}
