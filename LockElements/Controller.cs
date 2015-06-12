using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace LockElements
{
    public class Controller
    {
        public static void GetUserSelection(UIDocument uidoc)
        {
            IList<Reference> references = null;
            Document doc = uidoc.Document;
            Selection selection = uidoc.Selection;

            references = selection.PickObjects(ObjectType.Element, "Choose elements you would like to lock. Click FINISH to complete.");

            if (null == references) return; 

            IList<ElementId> elementIds = references.Select(r => r.ElementId).ToList();

            ExtensibleStorageUtils.AddOrUpdateElements(doc, elementIds);
            ExtensibleStorageUtils.ShowResultsElement(doc);
        }
    }
}
