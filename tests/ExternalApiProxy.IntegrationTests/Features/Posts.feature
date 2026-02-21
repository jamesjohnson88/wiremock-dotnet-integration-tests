Feature: Posts
    As a consumer of the ExternalApiProxy API
    I want to retrieve posts via the proxy
    So that the proxy correctly calls the upstream JSONPlaceholder service

    Scenario: Get a post by id returns the post details
        Given the upstream service will return a post with id 1
        When I request the post with id 1
        Then the response status should be 200
        And the response body should contain a post with id 1

    Scenario: Get a post with a non-existent id returns 404
        Given the upstream service will return 404 for post with id 999
        When I request the post with id 999
        Then the response status should be 404
