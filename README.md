# Bleeders

A narrative RPG/adventure with an orthographic 2.5D look, built on the FlickerVoid engine ([FlickerVoidNGN](https://github.com/tec-sx/FlickerVoidNGN)).

## Build

Requires the .NET 10 SDK. The engine is a git submodule at `Engine/`:

```
git clone --recurse-submodules https://github.com/tec-sx/Bleeders
cd Bleeders
dotnet build Bleeders.slnx
dotnet test --solution Bleeders.slnx
dotnet run --project Source/Launcher
```

In an existing clone, `git submodule update --init` fetches the engine. To build against an engine checkout of your own instead, set the `FLICKERVOID_ENGINE` environment variable to its folder.

Open `Bleeders.fvproject` in the FlickerVoid editor to edit the game's content.
