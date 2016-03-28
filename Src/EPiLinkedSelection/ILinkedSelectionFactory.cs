using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.Shell.ObjectEditing;

namespace EPiLinkedSelection
{
    /// <summary>
    /// Creates a list of selection items for a specific property.
    /// </summary>
    public interface ILinkedSelectionFactory
    {
        /// <summary>
        /// Creates a list of selection items for a specific property.
        /// </summary>
        /// <param name="contentData">The content data.</param>
        /// <returns>
        /// A list of selection items for a specific property.
        /// </returns>
        IEnumerable<ISelectItem> GetSelections(IContentData contentData);
    }
}
