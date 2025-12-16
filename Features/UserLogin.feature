Feature: User login and dashboard access
  In order to use the application
  As a registered user
  I want to log in and see my dashboard

  Background:
    Given the application is running

  @happy_path @smoke
  Scenario: Successful login with valid credentials
    Given I am on the login page
    When I enter a valid username and password
    And I click the login button
    Then I should be redirected to the dashboard
    And I should see my user greeting

  @negative
  Scenario: Login fails with invalid password
    Given I am on the login page
    When I enter a valid username and an invalid password
    And I click the login button
    Then I should see an error message saying "Invalid username or password"

  @security
  Scenario: User is logged out after closing the browser
    Given I am logged in as a valid user
    When I close the browser
    And I open the browser again
    And I navigate to the dashboard page
    Then I should be asked to log in again
