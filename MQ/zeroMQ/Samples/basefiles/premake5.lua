-- A solution contains projects, and defines the available configurations
workspace "FirstZeroMq2"
	configurations { "Debug", "Release" }
	platforms { "Win64" }
	 
	 -- A project defines one build target
	 project "FirstZeroMq2"	
		kind "ConsoleApp"
		language "C++"
		files { "**.h", "**.cpp" }
		--configurations { "Debug", "Release" }
		--platforms { "Win64" }
		
		includedirs { 
			"D:/00_Dev/C++/thirdparty/libzmq/include"
		  }
		 
			
		filter "configurations:Debug"
		do
			defines { "DEBUG", "_DEBUG" }
			--flags { "Symbols" }
			
			libdirs { 
				"D:/00_Dev/C++/thirdparty/libzmq/bin/x64/Debug/v141/static"
			  }
			links { -- -lopengl32 -lglu32 -lglut32 に相当
				"libzmq.lib"
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
				"libzmq.lib"
			}
		end

		filter { "platforms:Win64" }
			architecture "x64"
		filter {"platforms:Win64", "configurations:Debug" }
			targetdir("/Win64_Debug")
		filter {"platforms:Win64", "configurations:Release" }
			targetdir("/Win64_Release")
				
		characterset "Unicode"

		filter {} -- filter clear