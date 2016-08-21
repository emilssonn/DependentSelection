using System;

namespace EPiDependentSelection
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="EPiDependentSelection.DependentSelectAttribute" />
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DependentSelectManyAttribute : DependentSelectAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependentSelectManyAttribute"/> class.
        /// </summary>
        /// <param name="dependentSelectionFactoryType">Type of the dependent selection factory.</param>
        public DependentSelectManyAttribute(Type dependentSelectionFactoryType) 
            : base(dependentSelectionFactoryType, "epi-dependent-selection/DependentCheckBoxListEditor")
        {
        }
    }
}
