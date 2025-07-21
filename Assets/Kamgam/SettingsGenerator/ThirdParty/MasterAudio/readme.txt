This folder contains scripts that are NOT part of the SettingsGenerator assembly definition.
It was necessary to create this to support third party assets that do NOT support assembly definitions by default.
The way we support these is to define interfaces in the SettingsGenerator assembly that are the filled with a concrete implementation at runtime.