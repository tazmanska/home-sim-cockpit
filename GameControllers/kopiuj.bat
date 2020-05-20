cd %1

copy "bin\Debug\*.dll" "..\DomowyPanelApp\bin\Debug\modules\"
copy "bin\Release\*.dll" "..\DomowyPanelApp\bin\Release\modules\"

copy "GameControllers.xml" "..\DomowyPanelApp\bin\Release\modules\"
copy "GameControllers.xml" "..\DomowyPanelApp\bin\Debug\modules\"