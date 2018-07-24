# RunDotNetDll32 - Execute a .net dll method from the command line

### Execute a static method with Assembly.LoadFile
*rundotnetdll32.exe assembly.dll,class,method arguments*

### Execute a static method with Type.GetType
*rundotnetdll32.exe -t assembly.dll,class,method arguments*

* #### Execute Implant.RunCMD method <br>
  * rundotnetdll32.exe -t WheresMyImplant.dll,Implant,RunCMD whoami

### Listing the contents of and assembly

* #### Listing namespaces in an assembly
  * rundotnetdll32.exe -l WheresMyImplant.dll
  
* #### Listing classes in a namespace
  * rundotnetdll32.exe -l WheresMyImplant.dll -n WheresMyImplant
    
* #### Listing methods in a class
  * rundotnetdll32.exe -l WheresMyImplant.dll -n WheresMyImplant -c Implant
    
* #### Listing parameters for a method
  * rundotnetdll32.exe -l WheresMyImplant.dll -n WheresMyImplant -c Implant -m RunPowerShell
