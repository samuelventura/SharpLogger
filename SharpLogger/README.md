# SharpLogger App Spec

* Opens SharpLogger files on double click
* Filter, browse, delete, and export logs
* Concurrent usage with log owner app

## LogViewer Control Spec

* Auto freeze on scroll
* Copy all/selection capability
* Select/deselect item on click
* Clear selection on click on no-item
* Add item to selection by click+ctrl
* Add item to selection by shift+up/down
* Copy as text, csv, json, xml
* Ctrl+c copy

## Currently Implemented

 * Clear selection on clicking non item
 * Toggle selection on control + click
 * Range selection on shift + click
 * Range selection on control + mouse box
 * Select with mouse box
 * Full row selection

## Design Decisions

 * Remove TRACE in favor or text search
 * DEBUG to be only check filter in viewer
 * Items may accumulate if timer not running
