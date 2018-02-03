# RunDotNetDll32 - Execute a .net dll method from the command line

## Execute a static method
*rundotnetdll32.exe assembly.dll,class,method arguments*

* ### Execute Implant.RunCMD method <br>
  * rundotnetdll32.exe WheresMyImplant.dll,Implant,RunCMD whoami

## List the contents of and assembly
*rundotnetdll32.exe list assembly.dll <namespaces|classes|methods> <namespace> <class>*
  
* ### List all namespaces
  * rundotnetdll32.exe WheresMyImplant.dll namespaces
* ### List all classes in all namespaces
  * rundotnetdll32.exe WheresMyImplant.dll classes
* ### List all classes in namespace Implant
  * rundotnetdll32.exe WheresMyImplant.dll classes Implant
* ### List all methods in all namespaces and classes
  * rundotnetdll32.exe WheresMyImplant.dll methods
* ### List all methods in namespace Implant
  * rundotnetdll32.exe WheresMyImplant.dll methods Implant
* ### List all methods in namespace Implant and Class EmpireStager
  * rundotnetdll32.exe WheresMyImplant.dll methods Implant EmpireStager
