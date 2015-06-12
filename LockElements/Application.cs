﻿using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace LockElements
{
    public class Application : IExternalApplication
    {

        public Result OnShutdown(UIControlledApplication application)
        {
            Utils.ConsoleManager.Hide();

            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            Utils.ConsoleManager.Show();
            Console.WriteLine("info:\tLaunched Application");

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
            var classFilter = new ElementClassFilter(typeof(T));
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), classFilter, Element.GetChangeTypeElementDeletion());
            Console.WriteLine("info:\tCreated Trigger for " + typeof(T).Name);

        }
    }
}
