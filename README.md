# Asteroids Demo

Asteroids Demo is a showcase Unity project that combines a classic 2D arcade shooter loop with production-oriented mobile game infrastructure. It is designed as a demonstration project for architecture, runtime systems, and service integration, not just as a minimal gameplay prototype.

The repository is useful as a reference for how core gameplay, content delivery, monetization, persistence, and service integrations can live inside one coherent Unity project.

## Project Focus

- Arcade gameplay wrapped in a mobile-ready application flow.
- A service-oriented Unity architecture with clear runtime responsibilities.
- Data-driven configuration and remotely tunable game parameters.
- Practical integration of common mobile game systems in a compact demo project.

## Highlights

### Core gameplay

- Fast 2D space combat built around ship control, movement, and reactive weapon use.
- A mix of enemy and hazard behaviors, including asteroid pressure and aggressive UFO encounters.
- A run-based gameplay loop with scoring, defeat handling, and session continuation.

### Player experience

- Multi-scene flow covering startup, sign-in, menu, loading, and gameplay states.
- UI Toolkit-based menu, shop, and HUD presentation.
- Responsive interface behavior suitable for different screen sizes and aspect ratios.

### Mobile and live-ops systems

- Authentication, local persistence, and cloud-backed save flow.
- Automatic progress syncing with conflict resolution between save sources.
- In-game shop, in-app purchases, and ad-supported gameplay flows.
- Runtime balancing and parameter control through remote configuration.
- Session and gameplay analytics for player behavior tracking.

### Content pipeline

- Addressables-based asset loading for runtime content.
- Separation between local and remote content delivery.
- Prebuilt remote content included in `ServerData`.

## Architecture

### Scene-oriented composition

The project is structured around a clear scene flow rather than a single monolithic runtime. Bootstrap, authentication, menu, loading, and gameplay are treated as separate stages, which makes startup and state transitions easier to reason about.

### Dependency injection and modular installers

- Zenject is used as the composition root.
- Shared systems are registered globally, while scene-specific systems are bound where they are needed.
- This keeps service wiring explicit and avoids tightly coupled scene logic.

### Presenter-driven runtime

Runtime code is separated into models, presenters, services, and views:

- `Models` own state and gameplay rules.
- `Presenters` coordinate runtime behavior and connect logic to views.
- `Views` handle Unity-facing representation and user interaction.
- `Services` isolate infrastructure concerns such as loading, saves, pools, and integrations.

### Data-driven setup

- Runtime assets are organized through ScriptableObjects and Addressables.
- Balancing and numeric tuning can be overridden remotely.
- Local fallback data keeps the project runnable without live services.

### Async-first initialization

- Startup and loading flows are built around UniTask.
- Scene transitions and asset preparation happen asynchronously.
- Dedicated loading processors keep initialization logic structured and reusable.

### Runtime efficiency

- Frequently spawned objects are managed through object pools.
- This reduces allocation churn and keeps the gameplay loop closer to production-style runtime behavior.

## Tech Stack

### Unity stack

- Unity 6 (`6000.3.11f1`)
- URP
- UI Toolkit
- Input System
- Addressables
- Unity Ads
- Unity IAP
- Unity Services Authentication
- Unity Cloud Save

### External SDK

- `Zenject`
- `UniTask`
- `Firebase Analytics`
- `Firebase Remote Config`
- `External Dependency Manager for Unity`

## Project Structure

```text
Assets/_Project/
  Runtime/                # gameplay logic, services, presenters, views
  Scenes/                 # application and gameplay flow
  Prefabs/                # gameplay and UI prefabs
  Settings/               # content and configuration assets
  UI/                     # UI Toolkit layouts and styles

Assets/AddressableAssetsData/   # Addressables setup
ServerData/                     # remote content output
ProjectSettings/                # build/editor/runtime settings
```

## How to Run

1. Open the project in `Unity Editor 6000.3.11f1`.
2. Make sure Unity package dependencies are restored.
3. Open `Assets/_Project/Scenes/Bootstrap.unity`.
4. Enter Play Mode.

## Use Cases

- Portfolio showcase for Unity gameplay and mobile game engineering.
- Reference project for combining gameplay code with common mobile service integrations.
- Starter foundation for an arcade-style prototype with production-oriented scaffolding.
