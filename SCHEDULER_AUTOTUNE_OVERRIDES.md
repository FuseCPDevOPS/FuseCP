# Scheduler Auto-Tune and Overrides

This document describes how scheduler execution mode and task parallelism are resolved at runtime.

## Execution Placement Precedence

1. Explicit schedule execution mode parameter (`SCHEDULER_EXECUTION_MODE`) when not `AUTO`
2. Central task placement advisor mapping
3. Runtime fallback to `AUTO`

Implementation:
- `SchedulerTaskPlacementAdvisor`
- `SchedulerController.ResolveDispatchExecutionMode(...)`

## Parallelism Precedence

For heavy scheduler tasks that support bounded internal fan-out:
- `CalculatePackagesDiskspaceTask` (`MAX_PARALLEL_PACKAGES`)
- `CalculatePackagesBandwidthTask` (`MAX_PARALLEL_PACKAGES`)
- `CalculateExchangeDiskspaceTask` (`MAX_PARALLEL_ORGANIZATIONS`)

Precedence order:

1. Explicit task-specific parameter already present on schedule (for example `MAX_PARALLEL_PACKAGES`)
2. Schedule-level manual override:
   - `SCHEDULER_PARALLELISM_MODE = MANUAL`
   - `SCHEDULER_PARALLELISM_MAX = <positive integer>`
3. Schedule-level auto mode:
   - `SCHEDULER_PARALLELISM_MODE = AUTO`
   - applies recommendation from `SchedulerTaskParallelismAdvisor`
   - only active when `SchedulerAutoTuneEnabled` is true
4. Task built-in defaults if no override was applied

## UI Fields

The schedule edit page exposes:

- Parallelism Mode: `Auto` or `Manual override`
- Manual Max Parallelism: numeric value used only when mode is `Manual`

Admin behavior:
- Administrators can set both values.
- Non-admin users keep existing saved values; they cannot force elevated scheduler behavior.

## Runtime Trace Parameters

When scheduler applies parallelism, it stamps these background task parameters:

- `SCHEDULER_PARALLELISM_EFFECTIVE`
- `SCHEDULER_PARALLELISM_SOURCE`

This makes post-run diagnostics easier in task logs.
