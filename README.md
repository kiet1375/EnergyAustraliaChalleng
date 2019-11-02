

MusicFestival

API get for Energy Australia coding challenge. The application sorts the data by record labels, band names, and sorts fetivals of each band if quantity is greater than 2.

Application receives JSON object array in the form of,

[ { "name": , "bands": [ { "name":, "recordLabel": } ] } ]

converting it to a form of

[ { "recordLabel": , "bands": [ { "name":, "festivals": [ "festivalNames": ] } ] } ]

