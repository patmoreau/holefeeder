---
mode: agent
name: add-api-endpoint
description: >
  Add a new REST ennpoint to Holefeeder backend.
---

# Skill: Add a new REST endpoint to Holefeeder backend

Use this skill to add a new REST endpoint to the Holefeeder backend. The endpoint should follow the existing structure and conventions of the codebase.

---

## Variables

| Variable            | Example                                                |
| ------------------- | ------------------------------------------------------ |
| `$FEATURE`          | `transactions`                                         |
| `$VERB`             | `create` / `read` / `update` / `delete`                |
| `$ENDPOINT`         | `/transactions`                                        |
| `$HANDLER_FUNCTION` | `handleCreateTransaction`                              |
| `$REQUEST_METHOD`   | `POST` / `GET` / `PUT` / `DELETE`                      |
| `$REQUEST_PAYLOAD`  | `{ "amount": 100, "description": "Grocery shopping" }` |

---

## Step 1 - Define the new endpoint

`Features/$FEATURE/Endpoints.ts`

```csharp

```

**Rules**

- Return `Result<T>`, never throw exceptions.
