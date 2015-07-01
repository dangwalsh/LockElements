using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace Gensler
{
    class SelectionFilter : ISelectionFilter
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
