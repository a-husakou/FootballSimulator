* Domain representation
Team strength is represented by a base line plus a set of StrengthFactors (Attach, Defense, etc). In order to randomize simulation, there is a StrengthModifier concept, which represents a match context which can affect team strength either positively or negatively.
StengthModifier is factored in randomly during simulation.
The choice was to keep strength factor and strength modifier names hardcoded into enums, in the real app, those may be configurable via admin UI.

Mention that calculation rules could be made dynamic, and be used as a strategy. Such as instead of having a hardcoded logic of 
averaging factors when calculating a team rating, the calculation can be supplied via interface to choose from. That applies to all of the calculation.
To avoid extra complexity in the test assignemnt, the simple hardcoded calculation was chosen


** Match simulation
Match simulation is kept to a minimum, as the assignment does not emphasize its importance 

Some parts of the domain model are configurable via strategy pattern, FootballSimulator.Domain.Algorithms keeps implementation of domain abstractions.

IOC is not used for this app - poor man constructor injection