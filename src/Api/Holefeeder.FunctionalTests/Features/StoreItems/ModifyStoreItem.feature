@IntegrationTests

Feature: ModifyStoreItem
    Background:
        Given the following items
          | Id                                   | Code    | Data    | UserId                               |
          | b6d0941d-58be-44b1-94f5-3b95e64024a4 | code #1 | data #1 | b80b9954-3ee0-4bb0-80da-fa202744323e |

    Scenario: HappyPath
        Given I am authorized
        When I try to ModifyStoreItem with id 'b6d0941d-58be-44b1-94f5-3b95e64024a4' and data 'modified data'
        Then I expect a '204' status code

    Scenario: Unauthorized
        Given I am not authorized
        When I try to ModifyStoreItem
        Then I should not be authorized to access the endpoint

    Scenario: Authorized
        Given I am authorized
        When I try to ModifyStoreItem
        Then I should be authorized to access the endpoint