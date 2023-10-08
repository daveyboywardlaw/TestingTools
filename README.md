# TestingTools

This repository contains various useful pieces of C# code that can be used for testing. They will need some modifications to suit your needs but hopefully you will find them useful.

Below is an overview of each tool and any additional information you may need to use it.

## Accessibility Tools

### What does it do?
It runs an accessibility test using Selenium Axe across a website of your choice using the sitemap. Details on Selenium Axe can be found here [Selenim Axe](https://github.com/TroyWalshProf/SeleniumAxeDotnet) . The code generates reports for each page and also prints out each issue found, along with its severity and how it can be resolved. 

### Additional Information
In the code it will run accessibilty tests on markdown.org, update some namespaces (based on the sitemap) and publish accessibility reports to **_C:\AccessibilityReport_**. So you will need to/may want to update the following lines in the *AccessibilityTest.cs* file:

```var baseUrl = "http://www.mkdocs.org";```

```var documentLinks = xDoc.XPathSelectElements("/sm:urlset/sm:url/sm:loc", nsMgr);```

```var baseOutputDirectory = @"C:\AccessibilityReport";```


## Build Tools

### What does it do?
It looks at your last 10 DevOps builds and outputs the times that each test took as well as aggregating the time it takes on average for each test takes to run. This is outputted in 2 files, one called AggregatedIndividualTests.txt and one called IndividualTest.txt and these are outputted in a directory called C:\TEMP. 

### Additional Information
You need to parameterise certain elements of the code and these are defined in the parameters.xml file. These elements are:

* OrgURL - The organisation URL which contains the builds
  
* ProjectName - The name of the project under which the builds are run
  
* UserName - Username that can log in and view the builds

* PATToken - PAT Token to give the program access
