using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Gensler
{
    public class Controller
    {
        public static bool IsUnlock { get; set; }  

        public static IList<ElementId> GetUserSelection(UIDocument uidoc)
        {
            IList<Reference> references = null;
            Document doc = uidoc.Document;
            Selection selection = uidoc.Selection;

            references = selection.PickObjects(ObjectType.Element, "Choose elements you would like to lock. Click FINISH to complete.");

            if (null == references) return null; 

            IList<ElementId> elementIds = references.Select(r => r.ElementId).ToList();
            elementIds = FilterByElementClass(doc, elementIds);

            return elementIds;
        }

        private static IList<ElementId> FilterByElementClass(Document doc, IList<ElementId> elementIds)
        {
            List<ElementId> filteredIds = new List<ElementId>();
            SelectionFilter filter = new SelectionFilter();
            foreach (var elementId in elementIds)
            {
                Element element = doc.GetElement(elementId);
                if (filter.AllowElement(element))
                    filteredIds.Add(element.Id);
            }
            return filteredIds;
        }
    }
}
