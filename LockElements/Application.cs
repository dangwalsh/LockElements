using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace LockElements
{
    public class Application : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
#if DEBUG
            Utils.ConsoleManager.Hide();
#endif
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
#if DEBUG
            Utils.ConsoleManager.Show();
            Console.WriteLine("info:\tLaunched Application");
#endif
            Updater updater = new Updater(application.ActiveAddInId);
            UpdaterRegistry.RegisterUpdater(updater);

            CreateTrigger<Grid>(updater);
            CreateTrigger<Level>(updater);
            CreateTrigger<Wall>(updater);
            CreateTrigger<Floor>(updater);
            CreateTrigger<Ceiling>(updater);
            CreateTrigger<RoofBase>(updater);
            CreateTrigger<FamilyInstance>(updater);
            CreateTrigger<ReferencePlane>(updater);
            CreateTrigger<Opening>(updater);
            
            return Result.Succeeded;
        }

        private void CreateTrigger<T>(Updater updater)
        {
            ElementClassFilter classFilter = new ElementClassFilter(typeof(T));
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), classFilter, Element.GetChangeTypeElementDeletion());
#if DEBUG
            Console.WriteLine("info:\tCreated Trigger for " + typeof(T).Name);
#endif
        }
    }
}
