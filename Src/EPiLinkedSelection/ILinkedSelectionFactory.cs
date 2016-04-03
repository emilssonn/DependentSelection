using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.Shell.ObjectEditing;

namespace EPiLinkedSelection
{
    /// <summary>
    /// Creates a list of <see cref="ISelectItem" /> for a specific property.
    /// </summary>
    public interface ILinkedSelectionFactory
    {
        /// <summary>
        /// Creates a list of <see cref="ISelectItem" /> for a specific property.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>
        /// A list of <see cref="ISelectItem" /> for a specific property.
        /// </returns>
        IEnumerable<ISelectItem> GetSelections(Dictionary<string, object> values);

        /// <summary>
        /// Creates a list of <see cref="ISelectItem" /> for a specific property.
        /// </summary>
        /// <param name="contentData">The content data.</param>
        /// <returns>
        /// A list of <see cref="ISelectItem" /> for a specific property.
        /// </returns>
        IEnumerable<ISelectItem> GetSelectionsByContentData(IContentData contentData);
    }
}
