# SystemLibrary Common Episerver

## Requirements
- &gt;= .NET 5
- &gt;= Episerver 12

## Latest Version
- Initial version

## Description
A common library for all .NET &gt;= 5 episerver applications - various reusable extensions and classes
			
Selling points:
  * Extensions for XhtmlString, ContentReference, ContentArea, ...
  * Initialize your application through two methods for Episerver 12, in &gt;= .NET 5: app.CommonEpiserverInitialization(); and services.CommonEpiserverServices&lt;CurrentUser&gt;(options);
  * A log builder which builds a string with all needed information about current error and request, which you control where to log to (slack, cloud watch, sentry, firebase, ...)
  * Cache class that creates a unique key based on current user roles, with option add additional predicates, and also a flag to cache or not for CmsUsers
  * Contains a 'CurrentUser' that you can either inherit or just inject yourself in any service/controller

## Docs			
Documentation with samples:
https://systemlibrary.github.io/systemlibrary-common-episerver/

## Nuget
https://www.nuget.org/packages/SystemLibrary.Common.Episerver/

## Suggestions and feedback
support@systemlibrary.com

## Lisence
- It's free forever, copy paste as you'd like