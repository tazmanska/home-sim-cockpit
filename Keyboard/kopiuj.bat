cd %1

copy "bin\Debug\*.dll" "..\DomowyPanelApp\bin\Debug\modules\"
copy "bin\Release\*.dll" "..\DomowyPanelApp\bin\Release\modules\"

copy "Keyboard.html" "..\DomowyPanelApp\bin\Release\modules\"
copy "Keyboard.html" "..\DomowyPanelApp\bin\Debug\modules\"

copy "Keyboard.xml" "..\DomowyPanelApp\bin\Release\modules\"
copy "Keyboard.xml" "..\DomowyPanelApp\bin\Debug\modules\"