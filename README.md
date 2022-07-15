# SystemLibrary Common Episerver

## Requirements
- &gt;= .NET 6
- &gt;= Episerver 12


## Latest Version
- 3.1.0.1
- Updated latest dependencies
- Added unit tests for a few of the extension methods
- AppBuilderOptions renamed to EpiserverAppBuilderOptions (Breaking Change)
- Renamed ServiceCollectionOptions to EpiserverServiceCollectionOptions (Breaking change)
- EpiserverServiceCollectionOptions moved to new namespace (Breaking Change)

## Description
A common library for all .NET &gt;= 6 episerver applications - various reusable extensions and classes
			
Selling points:
  * Extensions for XhtmlString, ContentReference, ContentArea, ...
  * Initialize your application through CommonEpiserverInitialization(options); and services.CommonEpiserverServices&lt;CurrentUser&gt;(options);
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