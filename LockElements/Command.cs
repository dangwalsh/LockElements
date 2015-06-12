using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
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

            // TODO: move all method declarations out of this class
            IList<Reference> references = null;

            try
            {
                ConsoleManager.Show();
                Console.WriteLine("info:\tLaunched Command");

                var selection = uidoc.Selection;
                references = selection.PickObjects(ObjectType.Element,
                    @"Choose elements you would like to lock. Click FINISH to complete.");

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                Console.WriteLine("err:\t" + message);

                return Result.Failed;
            }

            if (null == references) return Result.Succeeded;

            // TODO: logic for adding newly selected elements to DataStorage goes here
            // either iterate collection and call utility methods
            // or send colection to utility methods
        }
    }
}
