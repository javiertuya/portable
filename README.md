![Status](https://github.com/javiertuya/portable/actions/workflows/test.yml/badge.svg)
[![Maven Central](https://img.shields.io/maven-central/v/io.github.javiertuya/portable-java)](https://central.sonatype.com/artifact/io.github.javiertuya/portable-java)
[![Nuget](https://img.shields.io/nuget/v/PortableCs)](https://www.nuget.org/packages/PortableCs/)

# Portable

A compact library with utility methods that are Java/C# compatible.
Use these utilities if generating portable applications that with converted .NET C# projects from Java.

Available on Java and .NET: 

- On Java: include the `portable-java` dependency as indicated in the 
  [Maven Central Repository](https://central.sonatype.com/artifact/io.github.javiertuya/portable-java)
- On .NET: include the `PortableCs` package in you project as indicated in 
  [NuGet](https://www.nuget.org/packages/PortableCs)

Contains different implementations in the `java` and `net` folders,
but the tests are shared (converted from java to .net using Sharpen).