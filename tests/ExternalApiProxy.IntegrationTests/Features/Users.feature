Feature: Users
    As a consumer of the ExternalApiProxy API
    I want to retrieve users via the proxy
    So that the proxy correctly calls the upstream JSONPlaceholder service

    Scenario: Get a user by id returns the user details
        Given the upstream service will return a user with id 1
        When I request the user with id 1
        Then the response status should be 200
        And the response body should contain a user with id 1

    Scenario: Get a user with a non-existent id returns 404
        Given the upstream service will return 404 for user with id 999
        When I request the user with id 999
        Then the response status should be 404
