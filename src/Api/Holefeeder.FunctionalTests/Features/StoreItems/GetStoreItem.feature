@IntegrationTests

Feature: StoreItem
    Background:
        Given the following items
          | Id                                   | Code    | Data    | UserId                               |
          | b6d0941d-58be-44b1-94f5-3b95e64024a4 | code #1 | data #1 | b80b9954-3ee0-4bb0-80da-fa202744323e |

    Scenario: HappyPath
        Given I am authorized
        When I try to GetStoreItem with id 'B6D0941D-58BE-44B1-94F5-3B95E64024A4'
        Then I expect a '200' status code
        And I get my expected item
          | Id                                   | Code    | Data    |
          | b6d0941d-58be-44b1-94f5-3b95e64024a4 | code #1 | data #1 |

    Scenario: Unauthorized
        Given I am not authorized
        When I try to GetStoreItem
        Then I should not be authorized to access the endpoint

    Scenario: Authorized
        Given I am authorized
        When I try to GetStoreItem
        Then I should be authorized to access the endpoint

    Scenario: Request Not Found
        Given I am authorized
        When I try to GetStoreItem with an empty id
        Then I expect a '422' status code
        And I get an problem details with error message saying 'One or more validation errors occurred.'