cd %1

copy "bin\Debug\*.dll" "..\DomowyPanelApp\bin\Debug\modules\"
copy "bin\Release\*.dll" "..\DomowyPanelApp\bin\Release\modules\"

copy "LCDOnLPT.xml" "..\DomowyPanelApp\bin\Release\modules\"
copy "LCDOnLPT.xml" "..\DomowyPanelApp\bin\Debug\modules\"