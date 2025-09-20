Links
[About](#about)
[Refactoring comments](#refactoring-comments)
[ToDo](#todo)

## About
Database for core service is PostgreSQl + ORM (EF Core)

## Refactoring comments
- very scared about this part
    .AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
}); - it will disapper with new roadmap contract

## Mapper
Decided to use Riok.Mapperly
Added to Bussines library 

## Validation
Was added validation to bussiness layer with error codes

## ToDo
[X] Add mapper
[X] Add validation
[X] Rewrite exceptions on codes