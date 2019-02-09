using System.Collections.Generic;

namespace Rubidium
{
    /// <summary>
    /// Interface for types that can contain variables (e.g., Expression, Statement).
    /// </summary>
    interface ICanContainVariables
    {
        /// <summary>
        /// Enumerable of variables contained (references) in the object.
        /// </summary>
        IEnumerable<string> Variables { get; }
        /// <summary>
        /// Boolean value indicating if the object contains any variable.
        /// </summary>
        bool ContainsVariables { get; }
    }
}
