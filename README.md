## Implemented Functionality (Summary)

### Architecture & Design Patterns

- **Page Object**: All UI interactions are implemented via page objects under the `EhuTests.NUnit.Pages` namespace (e.g. `HomePage`, `SearchResultsPage`). Test classes (`EhuUiTests`) and BDD steps (`EhuUserJourneySteps`) do not work with raw locators directly, but call strongly typed page object methods.
- **Singleton**: `DriverManager` implements a singleton WebDriver lifecycle used across all tests, including BDD steps. This avoids duplication of driver setup/teardown logic and keeps browser management centralized.
- **Factory / Builder (where appropriate)**: Browser options are created through a builder/factory style API (`OptionsBuilder.ForChrome()...`) so that browser configuration is composable and easy to extend.

### Logging & Reporting

- **Serilog integration**: The solution uses Serilog as the central logging library, configured in `Core\TestLog.cs`.
  - Logs are enriched with process id, thread id and environment user name.
  - Output targets:
    - Console (human-friendly format for local runs).
    - Rolling log files under `logs/` (JSON-like format for easier analysis).
- **Per-test logging**:
  - `Tests\BaseTest.cs` logs lifecycle events for every NUnit test (start, end, result, errors, screenshots on failure).
  - BDD step definitions add scenario-specific logs:
    - `StepDefinitions\UserLoginSteps.cs` logs every significant browser action for the login journey into `logs/bdd-tests-.log`.
    - `StepDefinitions\EhuUserJourneySteps.cs` logs navigation, language switch and search behaviour into `logs/bdd-ehu-journey-.log`.
- **Reporting**:
  - ExtentReports is integrated via `Core\ExtentManager.cs` and used from `BaseTest`.
  - Each test run produces an HTML report in the `reports/` folder with per-test status and details.
  - `Core\ReportAggregator.cs` writes an additional `summary.json` with aggregated statistics (total tests, passed, failed, skipped) for easy post-processing.
  - On failures, screenshot capture is implemented and attached to ExtentReports.

### Assertions with Shouldly

- The project references **Shouldly** and uses it in UI and BDD tests to make assertions more readable and expressive:
  - `EhuUiTests` uses calls like `currentUrl.ShouldContain("/about")` and `Driver.Url.ShouldStartWith(RuntimeConfig.Settings.Urls.BaseLt);`.
  - `EhuUserJourneySteps` uses `ShouldStartWith` and `ShouldContain` for URL and search-results verification.
- NUnit assertions are kept only where framework-specific behaviour is required (e.g. `Assert.Fail` for hard failures in infrastructure code), but all domain-level checks are moved to Shouldly as requested.

### BDD (Reqnroll) End-to-End Tests

- **Tooling**: The project references `Reqnroll` and includes all `.feature` files under `Features/` so that Reqnroll can generate and run BDD scenarios.

- **Feature files (Gherkin)**:
  - `Features\UserLogin.feature` describes a complete login journey:
    - Successful login with valid credentials (`@happy_path @smoke`).
    - Login failure with an invalid password (`@negative`).
    - Session invalidation after closing/reopening the browser (`@security`).
  - `Features\EhuUserJourney.feature` describes a realistic user journey on the EHU site:
    - Navigation from home page to About page (`@navigation @smoke`).
    - Switching site language to Lithuanian (`@localization`).
    - Searching for information using a **Scenario Outline** with examples (`@search`).

- **Step definitions (Reqnroll bindings)**:
  - `StepDefinitions\UserLoginSteps.cs`:
    - Implements Given/When/Then steps for the login scenarios using Selenium WebDriver (ChromeDriver).
    - Includes `BeforeScenario` / `AfterScenario` hooks to manage browser lifecycle.
    - Uses structured logging and meaningful assertion messages to simplify debugging.
  - `StepDefinitions\EhuUserJourneySteps.cs`:
    - Reuses existing architecture (Page Objects, `RuntimeConfig`, `DriverManager`) in BDD steps.
    - Implements steps like `Given I am on the EHU home page`, `When I open the About page from the header`, `When I switch the language to Lithuanian`, and `When I search for "<term>"`.
    - Uses Shouldly for assertions and Serilog for detailed scenario logging.
    - Shares state between steps via `ScenarioContext` (e.g. passing `HomePage` instance and search term) to avoid duplication and keep steps modular.

### How to Run the Tests

- **Build**:
  - Run `dotnet build` in the `EhuTests.NUnit` project directory, or use Visual Studio's *Rebuild Solution*.
- **Run all tests (including BDD)**:
  - From command line: `dotnet test`.
  - From Visual Studio: open *Test Explorer* and run all tests in the `EhuTests.NUnit` project. BDD scenarios from `.feature` files appear as individual tests.
- After test execution, inspect:
  - `logs/` for detailed text logs (including BDD-specific logs).
  - `reports/` for the ExtentReports HTML report and `summary.json` summary.

This setup demonstrates that all parts of the assignment are implemented: design patterns (Page Object, Singleton, Builder/Factory), structured logging with Serilog, expressive assertions with Shouldly, and Behaviour-Driven Development end-to-end tests implemented via Reqnroll with clear, reusable scenarios and detailed failure reporting.