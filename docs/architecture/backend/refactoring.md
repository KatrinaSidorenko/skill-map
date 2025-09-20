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
https://mapperly.riok.app/docs/intro/ 
Decided to use Riok.Mapperly
Added to Bussines library 

## Validation
https://docs.fluentvalidation.net/en/latest/start.html#
Was added validation to bussiness layer with error codes

### Error Codes
- SE - System Error 
- UIE - User Input Error

## ToDo
[X] Add mapper
[X] Add validation
[X] Rewrite exceptions on codes