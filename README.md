# geometry-api-cs


|   |Windows|
|:-:|:-:|
|**Debug**|[![Build Status](http://geometry-build.cloudapp.net/app/rest/builds/buildType:id:GEOMETRY_API_CS_BUILD/statusIcon)](http://geometry-build.cloudapp.net/project.html?projectId=GeometryApiCs&guest=1)|
|**Release**|[![Build Status](http://geometry-build.cloudapp.net/app/rest/builds/buildType:id:GEOMETRY_API_CS_BUILD/statusIcon)](http://geometry-build.cloudapp.net/project.html?projectId=GeometryApiCs&guest=1)
|**Coverage Report**|[![Coverage status](https://img.shields.io/badge/coverage-report-blue.svg)](http://geometry-build.cloudapp.net/project.html?projectId=GeometryApiCs&guest=1)|

The following three projects should all exist in the same directory:
[https://github.com/davidraleigh/JSON-java](https://github.com/davidraleigh/JSON-java)
[https://github.com/davidraleigh/geometry-api-cs](https://github.com/davidraleigh/geometry-api-cs)
[https://github.com/davidraleigh/geometry-api-java](https://github.com/davidraleigh/geometry-api-java)

the bash script should be executed from within the geometry-api-cs directory
```bash
$ ./generateCSharp.sh
```

Below is the original version of the conversion script
```bash
java -jar sharpencore-0.0.1-SNAPSHOT-jar-with-dependencies.jar ~/Downloads/jackson-core-master/src/ -cp ~/Downloads/geometry-api-java-master/DepFiles/unittest/junit-4.8.2.jar -junitConversion @sharpen-all-options
```


https://en.wikipedia.org/wiki/Comparison_of_C_Sharp_and_Java

http://web.archive.org/web/20131202022757/http://www.pauldb.me/post/14916717048/a-guide-to-sharpen-a-great-tool-for-converting-java
