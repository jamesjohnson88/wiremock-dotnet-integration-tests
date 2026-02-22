Feature: Todos
    As a consumer of the ExternalApiProxy API
    I want to retrieve todo items via the proxy
    So that the proxy correctly calls the upstream JSONPlaceholder service

    Scenario: Get a todo by id returns the todo details
        Given the upstream service will return a todo with id 1
        When I request the todo with id 1
        Then the response status should be 200
        And the response body should contain a todo with id 1

    Scenario: Get a todo with a non-existent id returns 404
        Given the upstream service will return 404 for todo with id 999
        When I request the todo with id 999
        Then the response status should be 404
