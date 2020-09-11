# RichFields

An example of creating custom HTML Helpers.  Some sample fields are defined for editing:
* Dates
* Ints
* Money (Decimal)
* Month and Year combinations (DateTime)
* Percents
* Plain text

The example fields create a wrapper div, an Input for user interaction,
a "scrubbed" version of the user's input for posting back to the server, and a
hidden input to compare with the user's edited values.

Whenvever the user's input mismatches with the original value loaded from the
server, the field is highlighted to indicate a save is needed.
