# .NET 10 Upgrade Design

## Goal

Retarget SynVideoDownloader from .NET Core 3.1 to .NET 10 and restore a clean build without broad, unrelated changes.

## Approach

Set the project target framework to `net10.0`. Replace the obsolete `Microsoft.Edge.SeleniumTools` integration with the supported Edge driver/options APIs from `Selenium.WebDriver`, updating Selenium dependencies as required. Update other package references only when required for compatibility or a successful .NET 10 build.

Preserve the downloader's existing supported sites and user-facing behavior. Keep changes to the checked-in Edge driver and runtime setup bounded to compatibility requirements discovered during implementation.

## Validation

Restore and build the solution targeting .NET 10, then run any relevant existing tests. Report any runtime or driver limitations that cannot be validated by the build.
