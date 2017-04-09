# FYP-Net
Final year project using the .NET stack. 

Project consists of:
* Web application which is a CMS/LMS (Content/Learning Management System) for data configuration
* XMPP messaging server using GCM (Google Cloud Messaging) to facilitate notification of changes for contextual clients which are related to the changes being made

Technologies used are:

* .NET Core MVC & Web API
* EntityFramework 7
* XMPP to communicate with Google Cloud (XMPP is deprecated as of [last year](https://cloud.google.com/appengine/docs/deprecations/xmpp))
* EF6 - for .NET Framework 4.6.1 as EF7 migration cdoe cannot be resued due to framework mismatches.
* XUnit/Moq - for unit testing
