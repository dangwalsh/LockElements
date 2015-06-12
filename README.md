#Interfaces
###IUpdater
1. Registers Failure Update with Revit
2. Calls ExtensibleStorageUtils on deletion update

###IExternalApplication
1. Initializers IUpdater

###IExternalCommand
1. Revit entry point

###ExtensibleStorageUtils
1. Read from _DataStorage_
2. Write to _DataStorage_
3. Check _DataStorage_ for protected elementIds
4. Display results

###Controller
1. Implements user selection
