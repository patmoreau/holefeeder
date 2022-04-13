@IntegrationTests

Feature: StoreItem
    Background:
        Given the following items
          | Id                                   | Code    | Data    | UserId                               |
          | B6D0941D-58BE-44B1-94F5-3B95E64024A4 | code #1 | data #1 | B80B9954-3EE0-4BB0-80DA-FA202744323E |

    Scenario: Unauthorized
        Given I am not authorized
        When I try to fetch StoreItem
        Then I should not be authorized to access the endpoint

    Scenario: Authorized
        Given I am authorized
        When I try to fetch StoreItem
        Then I should be authorized to access the endpoint
       
    Scenario: HappyPath
        Given I am authorized
        When I try to fetch StoreItem with id 'B6D0941D-58BE-44B1-94F5-3B95E64024A4'
        Then I expect a '200' status code
        And I get my expected item

    Scenario: Request Not Found
        Given I am authorized
        When I try to fetch StoreItem with an empty id
        Then I expect a '422' status code
        And I get an problem details with error message saying 'One or more validation errors occurred.'