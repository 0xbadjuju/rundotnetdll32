# RunDotNetDll32 - Execute a .net dll method from the command line

### Execute a static method
*rundotnetdll32.exe assembly.dll,class,method arguments*

* #### Execute Implant.RunCMD method <br>
  * rundotnetdll32.exe WheresMyImplant.dll,Implant,RunCMD whoami

### List the contents of and assembly
*rundotnetdll32.exe assembly.dll list <namespaces|classes|methods> <namespace> <class>*
  
* ### Listing namespaces
  * rundotnetdll32.exe WheresMyImplant.dll list namespaces
* ### Listing classes
  * **List all classes in all namespaces**
    * rundotnetdll32.exe WheresMyImplant.dll list classes
  * **List all classes in namespace Implant**
    * rundotnetdll32.exe WheresMyImplant.dll list classes Implant
* ### Listing methods
  * **List all methods in all namespaces and classes**
    * rundotnetdll32.exe WheresMyImplant.dll list methods
  * **List all methods in namespace Implant**
    * rundotnetdll32.exe WheresMyImplant.dll list methods Implant
  * **List all methods in namespace Implant and in class EmpireStager**
    * rundotnetdll32.exe WheresMyImplant.dll list methods Implant EmpireStager
