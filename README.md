#Capstone Project 03 For computer science capstone class at FCHS


~~It's a incredibly shortsighted, poorly made, idiotic API~~
It's the main API, SQL statements, configuration files, etc, for a 'Fun Fact API' that allows users to get random fun facts, either completey random or by genre. Technically, adding more databases for more fun facts shouldn't be difficult (as it's only a couple else if statements, and a new service-model-repository-sql are all needed),
I would've made it easier for users to add more DBs for funfacts, but that's not a requirement for the project and honestly, I'm kinda burnt out and also running out of time. 

Project is entirely written in C#, and uses packages like Dapper.Contrib, Xunit, Moq, and Aspnet. It uses Docker for containerization, Bruno for testing endpoints, and Swagger for generating documentation. I did try to expirement with several other languages while making this project, like Python, TypeScript, Rust, and Ruby, but decided 'why?' and gave 
up on those prospects.

Most of the actual code is in .Core. I tried to keep everything organized as best as possible. 
Testing is found in .Test, and .Auth is mostly unused. .Utilities just features helper stuff that the .Core uses.
All the Bruno calls are in BrunoCollection, and all MySQL statements/queries/whatever-they're-called are in DatabaseScripts.

I don't feel like writing documentation, as I'm already extremely tired on working on this project, and I've already met most of the requirements- a couple points off is fine. But, Swagger should suffice. 
