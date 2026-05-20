# 1.1. Software Engineering

## SW Challenge 1: Spec-Driven Development Basics

| Level | Skills | Required Resources | Time |
| --- | --- | --- | --- |
| Fundamentals | - Spec-Driven Development (SDD)<br>- requirements writing<br>- user stories<br>- acceptance criteria<br>- Markdown documentation | free LLM chat for spec review | 2 - 3 days |

**Challenge: Personal Task List**

The Personal Task List app should allow you to create, edit, delete, and mark tasks as completed within a task list.

Produce a complete specification package that contains at least:

- a vision document
- a list of user stories with Gherkin-style acceptance criteria
- a data model diagram
- an API contract written in OpenAPI

Then, build a minimal REST API in your preferred language that implements exactly what the specification says, nothing more (no authentication required). The goal is not the code itself, but the discipline of writing the spec first and following it strictly.

You can use an LLM chat (whichever you prefer) to help you complete this activity, ensuring you also include a Markdown (.md) file with the prompts used. We are not looking for you to use code assistants to build this activity for now; please focus on understanding how spec-driven development works before starting.

**Deliverables**

A repository containing at least the following:

- a /docs folder containing vision.md, user-stories.md, data-model.md, and openapi.yaml
- API code that exposes exactly the endpoints and commands described in the specification, including unit tests
- a README file that explain how to run the project, how to run tests and how to validate each user story
