# Architecture: "Pokeball" (simplified Clean Architecture)

Core sits in the center. Every other folder is a ring around it. Dependencies point INWARD only.

## Hard rules
- `core` MUST NOT reference `api`, `events`, `jobs`, `persistence`, `ui`, or any other ring. Not even for types or DTOs.
- Rings call core use cases. Rings never call each other (api never calls persistence directly).
- Core defines interfaces (ports). Rings implement them.
- Never persist core entities directly. Map to a persistence model in `persistence`.
- Never pass api/event DTOs into core. Validate and map at the boundary (anti-corruption layer).
- A feature MUST NOT reference another feature's folders.

## Folder layout
<src-root>/
    <feature-plural>/              e.g. features/transactions
        api/<endpoint-name>/       endpoint, request/response DTOs, validation, mapping to use-case input
        core/<use-case-name>/      use case, domain entities, value objects, port interfaces
        events/<event-name>/       consumers (map, then call use case) and producers (implement core port)
        jobs/<job-name>/           scheduled and background jobs; schedule, then call use case
        persistence/<repository>/  implements core repository ports; DB/S3/other-service models + mappers
        ui/                        (mobile only) screens and components; calls core use cases 
    shared/<ring>/                 code used by 2+ features ONLY

## Where does it go?
| Thing                                   | Location                                 |
|-----------------------------------------|------------------------------------------|
| Business rule / invariant               | x/core                                   |
| Request validation, DTO                 | x/api/<endpoint>                         |
| Repository interface                    | x/core/<use-case>                        |
| Repository implementation, DB model     | x/persistence/<repo>                     |
| Scheduled or background job             | x/jobs/<job>                             |
| Client for an external API              | new ring: ASK before naming it           |
| Type needed by a second feature         | move to shared/<ring> in the same change |

## shared/ rules: we code for the present
- NEVER create code in `shared` because it "will be needed later".
- New code goes in the feature that needs it, even if reuse looks likely.
- Move code to `shared/<same ring>` only when a SECOND feature needs it now.
  Do the move as part of that change, and update the imports in the first feature.

## When unsure
If something fits no existing ring, stop and propose a name instead of inventing one.

## Worked example: add "GET /transactions/{id}"
1. transactions/core/get-transaction/: query, handler, ITransactionReader port
2. transactions/persistence/transaction-repository/: implements ITransactionReader, maps DB model to entity
3. transactions/api/get-transaction/: endpoint, validates id, calls handler, maps entity to response DTO

## Verify
Run `<arch test command>`. Architecture tests enforce the dependency rules above.
