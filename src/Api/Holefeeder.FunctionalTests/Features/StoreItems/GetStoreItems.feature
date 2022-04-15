@IntegrationTests

Feature: StoreItems
    Background:
        Given the following items
          | Id                                   | Code    | Data    | UserId                               |
          | b6d0941d-58be-44b1-94f5-3b95e64024a4 | code #1 | data #1 | b80b9954-3ee0-4bb0-80da-fa202744323e |
          | f9b6836c-3a32-49fd-991c-2785aa74e8e6 | code #2 | data #2 | bf2f6750-6ada-4c1e-aa9f-ac7652b0ead7 |
          | 754baec1-d586-4ddc-b980-e0c1bbc951bd | code #3 | data #3 | b80b9954-3ee0-4bb0-80da-fa202744323e |

    Scenario: HappyPath
        Given I am authorized
        When I try to GetStoreItems using query params with offset '' and limit '' and sorts '-code' and filters ''
        Then I expect a '200' status code
        And I get my expected items
          | Id                                   | Code    | Data    |
          | 754baec1-d586-4ddc-b980-e0c1bbc951bd | code #3 | data #3 |
          | b6d0941d-58be-44b1-94f5-3b95e64024a4 | code #1 | data #1 |

    Scenario: Unauthorized
        Given I am not authorized
        When I try to GetStoreItems
        Then I should not be authorized to access the endpoint

    Scenario: Authorized
        Given I am authorized
        When I try to GetStoreItems
        Then I should be authorized to access the endpoint

    Scenario: Invalid Request
        Given I am authorized
        When I try to GetStoreItems using query params with offset '0' and limit '-1' and sorts 'date' and filters 'code:eq:settings'
        Then I expect a '422' status code
        And I get an problem details with error message saying 'One or more validation errors occurred.'