# Comics .NET 10.0 Upgrade Tasks

## Overview

This document tracks the atomic upgrade of the `Comics.slnx` solution to `.NET 10.0`. All projects will be upgraded simultaneously (convert non-SDK projects, update `TargetFramework`, update packages), followed by automated testing and validation.

**Progress**: 3/3 tasks complete (100%) ![100%](https://progress-bar.xyz/100)

---

## Tasks

### [✓] TASK-001: Verify prerequisites *(Completed: 2026-02-22 18:34)*
**References**: Plan §Annexes / Actions recommandées avant exécution, Plan §Stratégie de migration

- [✓] (1) Verify the .NET 10 SDK is installed on the execution environment per Plan §Annexes / Actions recommandées avant exécution
- [✓] (2) Runtime/SDK version meets minimum requirements (**Verify**)
- [✓] (3) Check for `global.json` and validate or update it if present per Plan §Stratégie de migration
- [✓] (4) Verify required build tools and CLI (`dotnet`, MSBuild) and configuration file compatibility (e.g., `Directory.Build.props`) per Plan §Annexes / Actions recommandées avant exécution

### [✓] TASK-002: Atomic framework and package upgrade with compilation fixes *(Completed: 2026-02-22 18:37)*
**References**: Plan §Stratégie de migration, Plan §Plans par projet, Plan §Référence de mise à jour des packages NuGet, Plan §Catalogue des ruptures attendues

- [✓] (1) Convert non-SDK projects to SDK-style and update `TargetFramework` for all projects listed in the plan (convert `SearchComics.csproj` to SDK-style with `<TargetFramework>net10.0</TargetFramework>`; update `DownloadComics.csproj` to `net10.0-windows` and ensure `<UseWindowsDesktop>true</UseWindowsDesktop>` as required) per Plan §Plans par projet
- [✓] (2) Update all package references per Plan §Référence de mise à jour des packages NuGet (apply evaluated compatible versions for .NET 10)  
- [✓] (3) Restore all dependencies (e.g., `dotnet restore`) per Plan §Stratégie de migration
- [✓] (4) Build the solution and fix all compilation errors caused by framework/package upgrades, addressing breaking changes listed in Plan §Catalogue des ruptures attendues
- [✓] (5) Rebuild solution to verify fixes; solution builds with 0 errors (**Verify**)
- [✓] (6) Commit changes with message: "TASK-002: Atomic upgrade to .NET 10.0"

### [✓] TASK-003: Run full test suite and validate upgrade *(Completed: 2026-02-22 18:39)*
**References**: Plan §Stratégie de tests et validation, Plan §Plans par projet, Plan §Catalogue des ruptures attendues

- [✓] (1) Run all automated test projects referenced in the plan (unit, integration, UI tests if automated) per Plan §Stratégie de tests et validation
- [✓] (2) Fix any test failures (reference Plan §Catalogue des ruptures attendues for common issues)
- [✓] (3) Re-run tests after fixes
- [✓] (4) All tests pass with 0 failures (**Verify**)
- [✓] (5) Commit test fixes with message: "TASK-003: Complete testing and validation"
