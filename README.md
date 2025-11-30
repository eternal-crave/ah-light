# AH-Light

A horror game project implementing dependency injection, state machines, object pooling, and other common game development patterns.

## Project Overview

First-person horror game with enemy AI, corridor-based level system, and game state management. Uses VContainer for dependency injection and implements various design patterns for code organization.

### Getting Started

1. Open project in Unity 6000.2.6f2
2. And play :D

## Architecture & Design Patterns

### Dependency Injection (VContainer)
- Scene-based lifetime scopes (`BootstrapLifetimeScope`, `LevelLifetimeScope`)
- Constructor injection via `[Inject]` attributes
- Interface-based services (`IInputService`, `IDeathCounterService`, `IEnemyPool`)

### State Machine Pattern
- Game state machine manages Bootstrap → Loading → Gameplay → Restart flow
- Enemy AI uses state-based behavior (Patrol, Chase, Stunned states)
- Type-safe state transitions with generic constraints

### Object Pooling
- Unity's `ObjectPool<T>` for enemy management
- Pool-aware enemy lifecycle with reset/cleanup methods

### Factory Pattern
- `EnemyFactory` handles enemy instantiation with dependency injection
- Uses VContainer's `IObjectResolver` for proper DI during creation

### Service Pattern
- `InputService` wraps Unity Input System actions
- `DeathCounterService` tracks player deaths
- Services implement interfaces for testability

### Event-Driven Architecture
- MessageHub for decoupled communication
- R3 reactive extensions for safe subscription management
- Automatic unsubscribe on MonoBehaviour disable/destroy

## Key Features

### First-Person Controller
- Character Controller-based movement
- Walk/sprint speed configuration
- Gravity and jump(disabled by default) mechanics
- Mouse look with pitch clamping
- Ground detection via Physics.CheckSphere
- Input abstraction through `IInputService` and Unity's new input system

### Enemy AI System
- NavMesh pathfinding
- State-based AI (Patrol, Chase, Stunned)
- Configurable detection and kill ranges
- Torch interaction system
- Pool-aware lifecycle management

### Extension Methods
- String utilities (`IsNullOrEmpty`, `IsJsonEmpty`)
- Math utilities (`NormalizeInto`, `GetLogarithmicDecibel`)
- Range checking methods

## Technical Stack

### Packages
- VContainer - Dependency Injection
- R3 - Reactive programming
- Unity Input System 
- Unity AI Navigation 
- URP
- DOTween - Animation
- Easy.MessageHub - Event messaging
- NaughtyAttributes

### Code Organization
```
Assets/_Project/Scripts/Runtime/
├── Enemy/              # Enemy AI, pooling, state management
├── Gameplay/           # Level controllers, spawn systems
├── Helpers/            # Extension methods, utilities
├── InjectionBase/      # VContainer lifetime scopes
├── LevelInitializers/  # Entry points
├── Player/             # First-person controller
├── Services/           # Input, death counter services
├── StateMachine/       # Game state machine
└── UI/                 # UI components
```
- Namespace-based separation
- Region-based code organization (SERIALIZED_FIELDS, MONO, PUBLIC_METHODS, etc.)
- Interface-based design where applicable
- Consistent naming conventions

## Technologies Used

- C# design patterns: Dependency Injection, Factory, State Machine, Object Pooling
- Unity systems: Input System, NavMesh, Character Controller, URP
- Architecture: Interface-based design, service pattern, event-driven communication
