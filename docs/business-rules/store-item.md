---
path: docs/business-rules/store-item.md
domain: store-item
last-reviewed: 2026-05
---

# Store Item Rules

## Concepts

A store item is a key-value pair that allows clients to persist arbitrary state on the server. Each item has a unique code (the key) and a data payload (the value). Store items are scoped to the
owning user.

---

## Create Store Item

**Happy path:**

- Code and data are provided
- The code is unique for this user
- A new store item is created with the given code and data

**Errors:**

| Condition                         | Error                 |
|-----------------------------------|-----------------------|
| Code is empty or whitespace       | rejected              |
| Data is empty                     | rejected              |
| Code already exists for this user | `Code already exists` |

---

## Modify Store Item

Updates the data payload of an existing store item.

**Happy path:**

- Store item ID and data are provided
- Store item exists and belongs to the current user
- The data field is updated

**Errors:**

| Condition                                            | Error                 |
|------------------------------------------------------|-----------------------|
| Store item ID is empty                               | rejected              |
| Data is empty                                        | rejected              |
| Store item does not exist or belongs to another user | `StoreItem not found` |

---

## Invariants

- A store item's code is immutable after creation
- Codes must be unique per user
- All operations are scoped to the owning user — cross-user access is rejected
