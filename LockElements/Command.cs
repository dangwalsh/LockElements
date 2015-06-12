using System;
using Autodesk.Revit.UI;
using Utils;

namespace LockElements
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Command :IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;

            try
            {
#if DEBUG
                ConsoleManager.Show();
#endif
                Console.WriteLine("info:\tLaunched Command");

                Controller.GetUserSelection(uidoc);

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                Console.WriteLine("err:\t" + message);

                return Result.Failed;
            }
        }
    }
}
