#VísualizationResponse

VisualizationResponse is a JSON object response product from Visualization API's GetVisualization endpoint. The response contains the data and metadata needed to draw a visualization. The response is structured in such a way that the data is always in the correct order for the visualization type that is selected. Below is an example of a response object.

```

{
    "tableReference": {
    "name": "example_table_123ab.px",
    "hierarchy": [
        "exampleDb",
        "exampl"
        ]
    },
    "data": [
        304.0,
        325.0,
        307.0,
        null,
        254.0,
        222.0,
        230.0,
        197.0,
        null,
        188.0,
        157.0,
        163.0,
    ],
    "dataNotes": {},
    "missingDataInfo": {"3": 3, "8": 1},
    "metadata": [*see metadata for more details*],
    "selectableVariableCodes": [
        "Area"
    ],
    "rowVariableCodes": [
        "Quarter"
    ],
    "columnVariableCodes": [],
    "visualizationSettings": {
        "visualizationType": "LineChart",
        "timeVariableIntervals": "Year",
        "timeSeriesStartingPoint": "1987-01-01T00:00:00Z",
        "cutValueAxis": false,
        "showDataPoints":false
        }
    }
```

#### tableReference
Reference to the table where the data is from. Contains the name of the table and the hierarchy.
```name``` is the name of the table/px file.
```hierarchy``` is a list of the names of the folders that contain the table, starting from the database.

#### data
Data for the visualization, can be floating point numbers and/or nulls.
Contains data for all the possible views of this visualization.
Data is structured in such a way that the data that belongs in one view is always
a continuous block and in the correct order. (See the selectable variable codes section
for more informaton about the views)

#### dataNotes
Multilanguage notes related to one data point, this feature is not yet complete

#### missingDataInfo

A dictionary that contains the missing data points and the reason for missing value.
Only the data points that have a note will have an entry in the dictionary.
Example entry:
```
{
    "5": 1,
    "29": 3
}
```

The integers represented here are:

1 = Missing,
2 = CannotRepresent,
3 = Confidential,
4 = NotAcquired,
5 = NotAsked, //or UnderConstruction / Error
6 = Empty, //Actual usage unclear
7 = Nill,  //Actual usage unclear

#### metaData

The metadata list defines the data values. The first data point is defined by the first
values of each variable (values field), second data point is defined by the second value
from the last variable on the list and first from every other variable and so on.
Meaning that the first variable in the metadata list is the most significant in some sense
(causes the biggest change in the index) and the last one is
the least significant (causes the smallest change).

| var0-val0 || var0-val1 ||
|---|---|---|---|
| var1-val0 | var1-val1 | var1-val0 | var1-val1 |
| 1.1 | 2.2 | 3.1 | 4.2 |

The table illustrates how four data points would be defined by two
variables that each have two values. (var0 would be first on the list, var1 second)


Example variable json:
```
{
    "code": "Tiedot",
    "name": {
        "fi": "Tiedot",
        "sv": "Uppgifter",
        "en": "Information"
    },
    "note": null,
    "type": "C",
    "values": [
        {
            "code": "keskivuokra",
            "name": {
                "fi": "Neli�vuokra (eur/m2)",
                "sv": "Kvadratmeterspris (eur/m2)",
                "en": "Rents per square meter (eur/m2)"
            },
            "note": null,
            "isSum": false,
            "contentComponent": {
                "unit": {
                    "fi": "eur / m2",
                    "sv": "eur / m2",
                    "en": "eur / m2"
                },
                "source": {
                    "fi": "Tilastokeskus, asuntojen vuokrat",
                    "sv": "Statistikcentralen, bostadshyror",
                    "en": "Statistics Finland, rents of dwellings"
                },
                "numberOfDecimals": 2,
                "lastUpdated": "2023-01-19T06:00:00Z"
            }
        }
    ]
}
```

The contentComponent field is only defined when the variable is a content variabale,
for all other variable types that field would be null. The metaData list will always contain
one and only one content variable. The variable type is defined by the type field.

#### selectableVariableCodes

The selectable variables define the views that the response contains.
Each combination of selections defines an unique view. This list contains
the codes of the variables that are selectable, and the selection options 
are the values of these variables. The selectable variables are always in the front
of the multivaluevariables in the metadata list, this causes the data of each view
to be a continuous block in the data list.

| var0-val0 |||| var0-val1 ||||
|---|---|---|---|---|---|---|---|
| var1-val0 || var1-val1 || var1-val0 || var1-val1 ||
| var2-val0 | var2-val1 | var2-val0 | var2-val1 | var2-val0 | var2-val1 | var2-val0 | var2-val1 |
| 1.1 | 2.2 | 3.1 | 4.2 | 5.1 | 6.3 | 7.2 | 3.9 |

If we extend the previous example table with one more variable but now the var0 variable
is selectable. Therefore there are two views of four values each, consisting of the values under
var0-val0 and var0-val1.

#### row- and columnVariableCodes

These variable codes define the orientation of the variables on a two dimensional plane
for the visualizations.

#### visualizationSettings

Contains the selected visualization type and additional input for drawing the visualization, content depends on the selected type.
Examples: ```defaultSelectableVariableCodes```, ```timeVariableIntervals```, ```cutValueAxis``` and so on.

The visualization type is defined by ```visualizationType``` possible values are:
- VerticalBarChart
- GroupVerticalBarChart
- StackedVerticalBarChart
- PercentVerticalBarChart
- HorizontalBarChart
- GroupHorizontalBarChart
- StackedHorizontalBarChart
- PercentHorizontalBarChart
- PyramidChart
- PieChart
- LineChart
- ScatterPlot
- Table
- KeyFigure