using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using ToeKnife.BspEditor.Documents;
using ToeKnife.BspEditor.Grid;
using ToeKnife.BspEditor.Modification;
using ToeKnife.BspEditor.Modification.Operations;
using ToeKnife.BspEditor.Primitives.MapData;
using ToeKnife.Common.Shell.Commands;
using ToeKnife.Common.Shell.Context;
using ToeKnife.Common.Shell.Hotkeys;
using ToeKnife.Common.Translations;

namespace ToeKnife.BspEditor.Tools.Grid
{
    [Export(typeof(ICommand))]
    [CommandID("BspEditor:Grid:CycleGrid")]
    [DefaultHotkey("Shift+R")]
    [AutoTranslate]
    public class SwitchGrid : ICommand
    {
        [ImportMany] private IGridFactory[] _grids;

        public string Name => "Switch grids";
        public string Details => "Cycle through grid types";

        public bool IsInContext(IContext context)
        {
            return context.TryGet("ActiveDocument", out MapDocument _);
        }

        public async Task Invoke(IContext context, CommandParameters parameters)
        {
            if (context.TryGet("ActiveDocument", out MapDocument doc))
            {
                if (!_grids.Any()) return;

                var current = doc.Map.Data.GetOne<GridData>()?.Grid;
                var idx = current == null ? -1 : Array.FindIndex(_grids, x => x.IsInstance(current));
                idx = (idx + 1) % _grids.Length;
                
                var grid = await _grids[idx].Create(doc.Environment);

                var gd = new GridData(grid);
                var operation = new TrivialOperation(x => doc.Map.Data.Replace(gd), x => x.Update(gd));

                await MapDocumentOperation.Perform(doc, operation);
            }
        }
    }
}