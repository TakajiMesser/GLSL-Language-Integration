@echo off

del "%LOCALAPPDATA%\Microsoft\VisualStudio\15.0_d930e0d8Exp\ComponentModelCache\Microsoft.VisualStudio.Default.cache" 2> nul
rmdir /S /Q "%LOCALAPPDATA%\Microsoft\VisualStudio\15.0_d930e0d8Exp\ComponentModelCache" 2> nul

reg delete HKCU\Software\Microsoft\VisualStudio\15.0_d930e0d8Expp\FontAndColors\Cache\{75A05685-00A8-4DED-BAE5-E7A50BFA929A} /f 