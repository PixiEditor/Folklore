using Folklore.Logical;

namespace Folklore.Syntax;

public interface IScopeReferences
{
    Dictionary<string, Reference> ReferencesInScope { get; set; }
}