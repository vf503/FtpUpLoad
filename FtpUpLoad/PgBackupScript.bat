@ECHO OFF
@setlocal enableextensions
SET PGPASSWORD=chenchen
SET PGPATH=C:\"Program Files"\PostgreSQL\10\bin\pg_dump
SET SVPATH=D:\ServerApp\DBBackup\
SET PRJDB=VideoProcessing
SET DBUSR=postgres
SET DBROLE=postgres
SET DBDUMP=%PRJDB%_%DATE:~0,4%%DATE:~5,2%%DATE:~8,2%%TIME:~0,2%%TIME:~3,2%%TIME:~6,2%.tar
@ECHO OFF
%PGPATH% -U postgres -Ft -b %PRJDB% > %SVPATH%%DBDUMP%
echo Backup Taken Complete %SVPATH%%DBDUMP%