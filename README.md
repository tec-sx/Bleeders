# Bleeders

A narrative RPG/adventure with an orthographic 2.5D look, built on the FlickerVoid engine ([FlickerVoidNGN](https://github.com/tec-sx/FlickerVoidNGN)).

## Build

Requires the .NET 10 SDK and one clone of the engine, [FlickerVoidNGN](https://github.com/tec-sx/FlickerVoidNGN), anywhere on disk, which every project shares. Set `FLICKERVOID_ENGINE` to its folder (on Windows, `setx FLICKERVOID_ENGINE C:\path\to\FlickerVoidNGN`, then open a new terminal or IDE); builds find the engine there. Clone this repository wherever you keep projects.

```
dotnet build Bleeders.slnx
dotnet test --solution Bleeders.slnx
dotnet run --project Source/Launcher
```

Open `Bleeders.fvproject` in the FlickerVoid editor to edit the game's content: Browse to it in the project picker once, and the picker lists it from then on wherever it is.
