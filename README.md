#Interfaces
###IUpdater
1. Registers Failure Update with Revit
2. Linked to element deletion
3. Contains logic to check _DataStorage_ for protected elementIds

###IExternalApplication
1. Initializers IUpdater
2. Subscribes to _DocumentOpened_ events

###IExternalCommand
1. Revit entry point
2. Implements user selection
3. Checks for matches then adds elementIds to _DataStorage_

###ExtensibleStorageUtils
1. Read from _DataStorage_
2. Write to _DataStorage_
3. Get 
