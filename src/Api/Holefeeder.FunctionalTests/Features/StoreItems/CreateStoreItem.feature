@IntegrationTests

Feature: CreateStoreItem
    Background:
        Given the following items
          | Id                                   | Code    | Data    | UserId                               |
          | b6d0941d-58be-44b1-94f5-3b95e64024a4 | code #1 | data #1 | b80b9954-3ee0-4bb0-80da-fa202744323e |

    Scenario: HappyPath
        Given I am authorized
        When I try to CreateStoreItem with code 'new code' and data ' new data'
        Then I expect a '201' status code
        And I get the route of the new resource in the header

    Scenario: Unauthorized
        Given I am not authorized
        When I try to CreateStoreItem
        Then I should not be authorized to access the endpoint

    Scenario: Authorized
        Given I am authorized
        When I try to CreateStoreItem
        Then I should be authorized to access the endpoint
