using System;
using System.Collections.Generic;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace LockElements
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class CommandLock : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Controller.IsUnlock = false;

            try
            {
#if DEBUG
                Console.WriteLine("info:\tLaunched Lock Command");
#endif
                IList<ElementId> elementIds = Controller.GetUserSelection(uidoc);

                ExtensibleStorageUtils.AddOrUpdateElements(doc, elementIds);
                ExtensibleStorageUtils.ShowResultsElement(doc);

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
#if DEBUG
                Console.WriteLine("err:\t" + message);
#endif
                return Result.Failed;
            }
        }
    }
}
