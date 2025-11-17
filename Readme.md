## Football Simulator

This prototype demonstrates how a small tournament engine can be modelled in C#.

### Domain Model

- **Teams & Strength** – Each `Team` owns a `TeamStrength` made up of a base rating (0‑100) and several `StrengthFactor`s (attack, defense, stamina, etc.). Match-day events feed in as `StrengthModifier`s that apply percentage adjustments to one or more factors before a match is simulated.
- **Hard-coded factors/modifiers** – For this exercise, factor and modifier names live in enums to keep things explicit. In a fuller product they could be data‑driven via admin screens.
- **Rating calculation strategy** – Ratings combine the base score and the (potentially adjusted) factor average using configurable weights from `DomainConstants`. These rules could be swapped for alternative strategies by introducing interfaces, but were kept simple here to reduce assignment complexity.

#### Match & Group Simulation

- **Match engine** – `KnuthAlgorithmMatchSimulation` uses the provided ratings plus configurable `MatchSimulationSettings` to generate Poisson-based scorelines.
- **Events per match** – `DefaultGroupSimulator` asks an `IMatchModifierEventProvider` for every fixture, so each match can experience different contextual events before ratings are computed.
- **Group standings** – A round-robin scheduler produces fixtures, results feed into `StandingAccumulator`, and the final table breaks ties by points, goal difference, goals scored, goals against, and head-to-head mini tables.
- **Pluggable algorithms** – Both the match simulator (`IMatchSimulator`) and the group scheduler (`IGroupScheduler`) are abstractions. The baseline implementations live in `FootballSimulator.Domain.Algorithms`, but can be swapped out for richer logic without touching the domain model.

### Extensibility Notes

- Because the emphasis is the domain model, the orchestration layer stays intentionally thin and no IoC container is introduced; dependencies are wired through constructors (“poor man’s DI”) to keep the flow explicit.

### Running the Sample

Two entry points exist:

1. **Console Runner** – `dotnet run --project FootballSimulator.ConsoleRunner` seeds a pseudo-random simulation using `DateTime.UtcNow.Ticks` and prints the standings/match list in a compact table.
2. **RandomGroupSimulationRunner** – Used by the console app and tests; it builds four sample teams, injects the algorithms, and runs a single group once.

### Tests

`dotnet test FootballSimulator.sln`

### Additional Ideas

- Persistence for teams/modifiers (e.g., JSON, database, or API).
- Tournament orchestrator that carries state across rounds and generates events based on previous scores (injuries, morale, fatigue, etc.).
