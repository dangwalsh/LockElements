using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;

namespace LockElements
{
    class ExtensibleStorageUtils
    {
        /********
         * 
         * Methods to assist with creating DataStorage and populating with data
         * 
         */

        /// <summary>
        /// Gets or creates the schema for storing specific results
        /// </summary>
        /// <returns></returns>
        private static Schema GetOrCreateSeparateResultsSchema()
        {
            Guid guid = new Guid("9FB88EA6-BAC0-44B8-87C1-BC59BF03E36B");

            Schema schema = Schema.Lookup(guid);

            if (schema != null)
                return schema;

            SchemaBuilder sb = new SchemaBuilder(guid);
            sb.SetSchemaName("WSAddInStoredResults");
            sb.AddSimpleField(s_applicationVersion, typeof(string));
            sb.AddSimpleField(s_lastUsed, typeof(string));
            sb.AddSimpleField(s_categoryId, typeof(ElementId));
            sb.AddArrayField(s_elementsToReview, typeof(ElementId));
            schema = sb.Finish();

            return schema;
        }

        private static Entity GenerateSeparateResultsEntity(Document doc, ElementId categoryId)
        {
            Schema schema = GetOrCreateSeparateResultsSchema();

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfCategoryId(categoryId);
            collector.WhereElementIsNotElementType();

            List<ElementId> elementIdsToTrack = new List<ElementId>();
            foreach (ElementId elementId in collector.ToElementIds())
            {
                if (ShouldTrackElement(doc, elementId))
                    elementIdsToTrack.Add(elementId);
            }

            Entity entity = new Entity(schema);
            entity.Set<string>(s_applicationVersion, "1.0.0.0");
            entity.Set<string>(s_lastUsed, DateTime.Now.ToLongTimeString());
            entity.Set<ElementId>(s_categoryId, categoryId);
            entity.Set<IList<ElementId>>(s_elementsToReview, elementIdsToTrack);

            return entity;
        }

        private static void AddOrUpdateSeparateResultsElement(Document doc, ElementId categoryId)
        {
            DataStorage dse = FindSeparateResultsElement(doc, categoryId);

            using (Transaction t = new Transaction(doc, "Add or update separate results element"))
            {
                t.Start();
                if (dse == null)
                {
                    dse = DataStorage.Create(doc);
                    dse.Name = "Tracking for category id " + categoryId.IntegerValue;
                }
                Entity entity = GenerateSeparateResultsEntity(doc, categoryId);
                dse.SetEntity(entity);
                t.Commit();
            }
        }

        private static DataStorage FindSeparateResultsElement(Document doc, ElementId categoryId)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(DataStorage));
            Schema schema = GetOrCreateSeparateResultsSchema();
            collector.WherePasses(new ExtensibleStorageFilter(schema.GUID));

            foreach (DataStorage dse in collector.Cast<DataStorage>())
            {
                Entity entity = dse.GetEntity(schema);
                if (entity != null)
                {
                    ElementId storedCategoryId = entity.Get<ElementId>(s_categoryId);

                    if (categoryId == storedCategoryId)
                    {
                        return dse;
                    }
                }
            }

            return null;
        }

        /****************
         * 
         * Methods to assist with removing accessing data from DataStorage
         * 
         */

        private static void ShowSeparateResultsElement(Document doc, ElementId categoryId)
        {
            DataStorage dse = FindSeparateResultsElement(doc, categoryId);
            String label = "Results element for " + GetCategoryString(doc, categoryId);
            if (dse != null)
            {
                TaskDialog ts = new TaskDialog("Results element");
                ts.MainInstruction = label;

                Schema schema = GetOrCreateSeparateResultsSchema();
                Entity entity = dse.GetEntity(schema);
                IList<ElementId> ids = entity.Get<IList<ElementId>>(s_elementsToReview);

                String content = String.Format("App version {0}\nUpdated {1}\nIds({2}):\n {3}",
                                               entity.Get<String>(s_applicationVersion),
                                               entity.Get<String>(s_lastUsed),
                                               ids.Count,
                                               String.Join(",\n", ids));

                ts.MainContent = content;

                ts.Show();
            }
            else
            {
                TaskDialog.Show("Results element", label + ": Not found");
            }
        }

        private static String GetCategoryString(Document doc, ElementId categoryId)
        {
            Category cat = doc.Settings.Categories.get_Item((BuiltInCategory)categoryId.IntegerValue);

            return cat.Name;
        }
    }
}
