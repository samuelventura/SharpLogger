# SharpLogger App Spec

* Opens SharpLogger files on double click
* Filter, browse, delete, and export logs
* Concurrent usage with log owner app
* Persist all with post filtered view
* Log files default to EntryAssy.SharpLogger
* Log files configurable to EntryAssy.ID.SharpLogger
* Human readable indexed log files
* NET Core compatible

## LogViewer Control Spec

* Won't lag under heavy load in low spec CPU
* Real time last N and post filtered view 
* Auto freeze on scroll
* Copy all/selection capability
* Select/deselect item on click
* Clear selection on click on no-item
* Add item to selection by click+ctrl
* Add item to selection by shift+up/down
* Copy as text, csv, json, xml
* Ctrl+c copy
* Custom item color 
* Custom row formatting
* Auto persistible configuration
* Visual feedback on mouse box selection
* Copy context menu

## Currently Implemented

 * Clear selection on clicking non item
 * Toggle selection on control + click
 * Range selection on shift + click
 * Range selection on control + mouse box
 * Select with mouse box
 * Full row selection

## Design Decisions

 * TRACE replaced by persist all + post filter view
 * DEBUG to be only check filter in viewer
 * Items may accumulate if timer not running
 * More file viewer like than table like
 * Appender accumulates before switching to UI
 * String log level to handle custom colors

