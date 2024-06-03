# Customer Service API and site

Simple customer service API and site.

Split from [Entity framework studies](https://github.com/wallymathieu/entity-framework-studies). The test code is stripped down for readability and focus.

In order to use the database locally with docker compose you need to migrate it.

```sh
docker compose up -d # start the database
dotnet tool restore # restore dotnet ef cli tool
dotnet ef database update --project src/Web # migrate the database
```
