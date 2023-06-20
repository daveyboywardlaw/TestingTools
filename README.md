# TestingTools

This repository contains various useful pieces of C# code that can be used for testing. They will need some modifications to suit your needs but hopefully you will find them useful.

Below is an overview of each tool and any additional information you may need to use it.

## Accessibility Tools

### What does it do?
It runs accessibility test using Selenium Axe across a website of your choice using the sitemap. Details on Selenium Axe can be found here [Selenim Axe](https://github.com/TroyWalshProf/SeleniumAxeDotnet)

### Additional Information
In the code it will run accessibilty tests on markdown.org, update some namespaces (based on the sitemap) and publish accessibility reports to C:\AccessibilityTests. So you will need to/may want to update the following lines in the *AccessibilityTest.cs* file:

```var baseUrl = "http://www.mkdocs.org";```

```var documentLinks = xDoc.XPathSelectElements("/sm:urlset/sm:url/sm:loc", nsMgr);```

```var baseOutputDirectory = @"C:\AccessibilityReport";```
