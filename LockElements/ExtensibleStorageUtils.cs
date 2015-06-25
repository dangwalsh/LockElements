using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;

namespace LockElements
{
    class ExtensibleStorageUtils
    {
        private static string s_applicationVersion = "ApplicationVersion";
        private static string s_lastUsed = "LastUsed";
        private static String s_elementsToLock = "ElementsToLock";

        private static Schema GetOrCreateLockSchema()
        {
            Guid guid = new Guid("12491EDC-9B7C-4150-B8B6-0245BB791C3D");

            Schema schema = Schema.Lookup(guid);

            if (schema != null)
                return schema;

            SchemaBuilder sb = new SchemaBuilder(guid);
            sb.SetSchemaName("ElementLockInfo");
            sb.AddSimpleField(s_applicationVersion, typeof(string));
            sb.AddSimpleField(s_lastUsed, typeof(string));
            sb.AddArrayField(s_elementsToLock, typeof(string));
            schema = sb.Finish();

            return schema;
        }

        private static DataStorage FindDataStorageElement(Document doc, Schema schema)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(DataStorage));
            collector.WherePasses(new ExtensibleStorageFilter(schema.GUID));

            return collector.FirstElement() as DataStorage;
        }

        private static Entity GenerateResultsEntity(Document doc, Schema schema, DataStorage dse, List<string> elementList)
        {
            IList<string> ids = GetElementElementIdList(dse, schema);

            foreach (var element in elementList)
            {
                if (!ids.Contains(element))
                    ids.Add(element);
            }

            Entity entity = new Entity(schema);
            entity.Set<string>(s_applicationVersion, "0.0.0.1");
            entity.Set<string>(s_lastUsed, DateTime.Now.ToLongTimeString());
            entity.Set<IList<string>>(s_elementsToLock, ids);

            return entity;
        }

        public static List<string> CreateElementIdList(Document doc, IEnumerable<ElementId> elementIds)
        {
            List<string> elementList = new List<string>();
            foreach (var elementId in elementIds)
            {
                if (doc.IsWorkshared)
                    AttemptToCheckoutInAdvance(doc, elementId);

                elementList.Add(elementId.ToString());
            }

            return elementList;
        }

        public static void AddOrUpdateElements(Document doc, IEnumerable<ElementId> elementIds)
        {
            List<string> elementList = CreateElementIdList(doc, elementIds);
            Schema schema = GetOrCreateLockSchema();
            DataStorage dse = FindDataStorageElement(doc, schema);

            using (Transaction t = new Transaction(doc, "Add or update separate results element"))
            {
                t.Start();
                if (dse == null)
                {
                    dse = DataStorage.Create(doc);
                    dse.Name = "Tracking for elements to be locked.";
                }
                Entity entity = GenerateResultsEntity(doc, schema, dse, elementList);
                dse.SetEntity(entity);
                t.Commit();

                IList<string> ids = entity.Get<IList<string>>(s_elementsToLock);
                String content = String.Format("info:\tDataStorage updated {0}\n\tIds ({1}):\n\t{2}", 
                                                entity.Get<String>(s_lastUsed),
                                                ids.Count, 
                                                String.Join("\n\t", ids));
#if DEBUG
                Console.WriteLine(content);
#endif
            }
        }

        public static void ShowResultsElement(Document doc)
        {
            Schema schema = GetOrCreateLockSchema();
            DataStorage dse = FindDataStorageElement(doc, schema);
            String label = "Command results:";
            if (dse != null)
            {
                TaskDialog ts = new TaskDialog("Results");
                ts.MainInstruction = label;

                Entity entity = dse.GetEntity(schema);
                IList<string> ids = entity.Get<IList<string>>(s_elementsToLock);

                String content = String.Format("Application version {0}\nUpdated {1}\n\nElementIds ({2}):\n\n{3}",
                                               entity.Get<String>(s_applicationVersion),
                                               entity.Get<String>(s_lastUsed),
                                               ids.Count,
                                               String.Join("  ", ids));
                ts.MainContent = content;
                ts.Show();
            }
            else
            {
                TaskDialog.Show("Results element", label + ": Not found");
            }
        }

        public static IList<string> GetElementElementIdList(DataStorage dse, Schema schema)
        {
            IList<string> ids = null;
            Entity existEntity = dse.GetEntity(schema);

            if (existEntity.IsValid())
                ids = existEntity.Get<IList<string>>(s_elementsToLock);
            else
                ids = new List<string>();

            return ids;
        }

        public static void AttemptToCheckoutInAdvance(Document doc, ElementId elementId)
        {
            // Checkout attempt
            ICollection<ElementId> checkedOutIds = WorksharingUtils.CheckoutElements(doc, new ElementId[] { elementId });

            // Confirm checkout
            bool checkedOutSuccessfully = checkedOutIds.Contains(elementId);
            if (!checkedOutSuccessfully)
            {
                throw new Exception("Cannot edit the Element - " +
                                "it was not checked out successfully and may be checked out to another.");
            }

            // If element is updated in central or deleted in central, it is not editable
            ModelUpdatesStatus updatesStatus = WorksharingUtils.GetModelUpdatesStatus(doc, elementId);
            if (updatesStatus == ModelUpdatesStatus.DeletedInCentral || updatesStatus == ModelUpdatesStatus.UpdatedInCentral)
            {
                throw new Exception("Cannot edit the Element - " +
                                "it is not up to date with central, but it is checked out.");
            }
        }

        public static bool IsLocked(Document doc, ElementId elementId)
        {
            Schema schema = GetOrCreateLockSchema();
            DataStorage dse = FindDataStorageElement(doc, schema);
            if (null == dse) return false;

            IList<string> uids = GetElementElementIdList(dse, schema);

            return uids.Contains(elementId.ToString());
        }
    }
}
