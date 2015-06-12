using System;
using Autodesk.Revit.DB;


namespace LockElements
{
    public class Updater : IUpdater
    {
        private const string Name = "Lock Elements";

        private readonly AddInId _addInId;

        public AddInId AddInId
        {
            get { return _addInId; }
        }

        private readonly UpdaterId _updaterId;

        public UpdaterId UpdaterId
        {
            get { return _updaterId; }
        }

        private readonly FailureDefinitionId _failureDefinitionId;

        public FailureDefinitionId MyProperty
        {
            get { return _failureDefinitionId; }
        }
        
        
        public Updater(AddInId addInId)
        {
            _addInId = addInId;
            _updaterId = new UpdaterId(_addInId, new Guid("32C24BA3-6BB4-4289-84B5-DD57285F25CE"));
            _failureDefinitionId = new FailureDefinitionId(new Guid("257426D2-4D62-4DFB-81E0-CB18A3AFEDEA"));
            FailureDefinition failureDefinition = 
                FailureDefinition.CreateFailureDefinition(_failureDefinitionId,
                FailureSeverity.Error, "This element has been locked. Please contact a project leader if you need to delete it.");
        }

        public void Execute(UpdaterData data)
        {
            Document doc = data.GetDocument();
            foreach (ElementId deletedElementId in data.GetDeletedElementIds())
            {
                if(!ExtensibleStorageUtils.IsLocked(doc, deletedElementId)) continue;
                FailureMessage failureMessage = new FailureMessage(_failureDefinitionId);
                failureMessage.SetFailingElement(deletedElementId);
                doc.PostFailure(failureMessage);
            }
        }

        public string GetAdditionalInformation()
        {
            return "Prevent deletion of selected elements.";
        }

        public ChangePriority GetChangePriority()
        {
            return ChangePriority.FloorsRoofsStructuralWalls;
        }

        public UpdaterId GetUpdaterId()
        {
            return UpdaterId;
        }

        public string GetUpdaterName()
        {
            return Name;
        }
    }
}
