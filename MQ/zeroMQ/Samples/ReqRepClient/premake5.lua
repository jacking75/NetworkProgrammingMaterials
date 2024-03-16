-- A solution contains projects, and defines the available configurations
  workspace "ReqRepClient"	
     configurations { "Debug", "Release" }
	 platforms { "Win64" }
	 
  -- A project defines one build target
  project "ReqRepClient"	
    kind "ConsoleApp"
    language "C++"
    files { "**.h", "**.hpp", "**.cpp" }
		
	includedirs { 
        "D:/00_Dev/C++/thirdparty/libzmq/include"
      }
	 
	--flags {"StaticRuntime" } /MTD, /MD
		
	filter "configurations:Debug"
	do
		defines { "DEBUG", "_DEBUG" }
		flags { "Symbols" }
		
		libdirs { 
			"D:/00_Dev/C++/thirdparty/libzmq/bin/x64/Debug/v141/static"
		  }
		links { 
			"ws2_32.lib", "IPHLPAPI.lib", "libzmq.lib"
		}
	end

	filter "configurations:Release"
	do
		defines { "NDEBUG" }
		optimize "On"
		
		libdirs { 
			"D:/00_Dev/C++/thirdparty/libzmq/bin/x64/Release/v141/static"
		  }
		links { 
			"ws2_32.lib", "IPHLPAPI.lib", "libzmq.lib"
		}
	end

    filter { "platforms:Win64" }
		architecture "x64"
	--filter {"platforms:Win64", "configurations:Debug" }
	--	targetdir("Win64_Debug")
	--filter {"platforms:Win64", "configurations:Release" }
	--	targetdir("Win64_Release")
		    
    characterset "Unicode"

	filter {} -- filter clear