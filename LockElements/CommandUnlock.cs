using System;
using System.Collections.Generic;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace Gensler
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class CommandUnlock : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Controller.IsUnlock = true;

            try
            {
#if DEBUG
                Console.WriteLine("info:\tLaunched Unlock Command");
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
