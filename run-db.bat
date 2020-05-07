SET CURRENT_PATH=%~dp0
mongod --dbpath="%CURRENT_PATH:~0,-1%"