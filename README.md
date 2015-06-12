#LockElements
This Revit addin will allow a user to store Ids of elements to be protected from accidental deletion.  When a deletion update is made by the application the addin will check the id against a list stored in extensible storage.

####Known Issues
This addin stores ElementIds in the form of strings which will not be updated by Revit dynamically.  This can present a problem as ElementIds are not stable.  Use with caution!

####IUpdater
+ Registers Failure Update with Revit
+ Calls ExtensibleStorageUtils on deletion update

####IExternalApplication
+ Initializes IUpdater

####IExternalCommand
+ Revit entry point

####ExtensibleStorageUtils
+ Read from _DataStorage_
+ Write to _DataStorage_
+ Check _DataStorage_ for protected ElementIds
+ Display results

####Controller
+ Implements user selection
