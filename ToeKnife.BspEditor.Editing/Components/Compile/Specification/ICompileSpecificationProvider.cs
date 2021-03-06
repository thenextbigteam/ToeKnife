using System.Collections.Generic;
using System.Threading.Tasks;

namespace ToeKnife.BspEditor.Editing.Components.Compile.Specification
{
    public interface ICompileSpecificationProvider
    {
        Task<IEnumerable<CompileSpecification>> GetSpecifications();
    }
}