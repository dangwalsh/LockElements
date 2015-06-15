using System;
using Autodesk.Revit.UI;


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
                Console.WriteLine("info:\tLaunched Command");
#endif
                Controller.GetUserSelection(uidoc);

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
