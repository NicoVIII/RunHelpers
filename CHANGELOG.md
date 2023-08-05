# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Changed
- Move project in argument list to the front like in `dotnet publish --help` suggested
- RunHelpers now targets .NET 6 instead of .NET 5

## [0.5.0] - 2022-05-28
### Added
- DotNet template for toolRestore and restore in one step named restoreWithTools
- DotNet template for basic packing
- DotNet template for self contained publishing
- README to nuget package
- Add "parallel" operation to allow parallel execution of stuff
- Basic cmd shortcut to easily create own shortcuts

### Changed
- Replaced `Template` module with RequiredQualifiedAccess by `Templates` namespace
- Restructure structure to enable parallelization feature
- Remove exitcode from error type

## [0.4.1] - 2021-11-16
### Fixed
- Loosen FSharp.Core version constraints (from ~> 5.0 to >= 5.0 which allows e.g. 6.0)

## [0.4.0] - 2021-10-30
### Added
- Helpers to build watch commands

## [0.3.0] - 2021-10-30
### Changed
- Split template for tool restore and project restore

## [0.2.0] - 2021-10-30
### Changed
- Rename FakeExtension stuff to avoid nameclashes with FAKE
- Restructure stuff a bit

## [0.1.0] - 2021-10-27
Initial release

[Unreleased]: https://github.com/NicoVIII/RunHelpers/compare/v0.5.0...HEAD
[0.5.0]: https://github.com/NicoVIII/RunHelpers/compare/v0.4.1..v0.5.0
[0.4.1]: https://github.com/NicoVIII/RunHelpers/compare/v0.4.0..v0.4.1
[0.4.0]: https://github.com/NicoVIII/RunHelpers/compare/v0.3.0..v0.4.0
[0.3.0]: https://github.com/NicoVIII/RunHelpers/compare/v0.2.0..v0.3.0
[0.2.0]: https://github.com/NicoVIII/RunHelpers/compare/v0.1.0..v0.2.0
[0.1.0]: https://github.com/NicoVIII/RunHelpers/releases/v0.1.0
