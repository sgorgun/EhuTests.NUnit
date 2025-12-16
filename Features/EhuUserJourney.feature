Feature: EHU site user journey
  In order to learn about EHU
  As a prospective student
  I want to browse the site and find relevant information

  Background:
    Given I am on the EHU home page

  @navigation @smoke
  Scenario: User opens the About page
    When I open the About page from the header
    Then the About page should be displayed

  @localization
  Scenario: User switches site language to Lithuanian
    When I switch the language to Lithuanian
    Then the Lithuanian home page should be displayed

  @search
  Scenario Outline: User searches for information
    When I search for "<term>"
    Then the search results should contain "<expectedKeyword>"

    Examples:
      | term              | expectedKeyword |
      | business studies  | business        |
      | communication art | communication   |
