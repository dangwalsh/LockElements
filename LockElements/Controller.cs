using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace LockElements
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

            //ExtensibleStorageUtils.AddOrUpdateElements(doc, elementIds);
            //ExtensibleStorageUtils.ShowResultsElement(doc);
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

    public class SelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is Grid) return true;
            if (elem is Level) return true;
            if (elem is Wall) return true;
            if (elem is Floor) return true;
            if (elem is Ceiling) return true;
            if (elem is RoofBase) return true;
            if (elem is ReferencePlane) return true;
            if (elem is FamilyInstance) return true;
            if (elem is Opening) return true;

            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new System.NotImplementedException();
        }
    }

}
