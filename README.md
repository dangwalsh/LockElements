#LockElements
This Revit addin will allow a user to store Ids of elements to be protected from accidental deletion.  When a deletion update is made by the application the addin will check the id against a list stored in extensible storage.
####Known Issues
This addin stores element ids in the form of strings which will not be updated by Revit dynamically.  This can present a problem as ElementIds are not stable.
####IUpdater
1. Registers Failure Update with Revit
2. Calls ExtensibleStorageUtils on deletion update

####IExternalApplication
1. Initializers IUpdater

####IExternalCommand
1. Revit entry point

####ExtensibleStorageUtils
1. Read from _DataStorage_
2. Write to _DataStorage_
3. Check _DataStorage_ for protected elementIds
4. Display results

####Controller
1. Implements user selection
